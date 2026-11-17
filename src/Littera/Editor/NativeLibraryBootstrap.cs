using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ScintillaNET;
using Architecture = System.Runtime.InteropServices.Architecture;

namespace Notas.Editor;

/// <summary>
/// Garante que Scintilla.dll e Lexilla.dll sejam encontradas.
/// Em build normal elas ficam em runtimes\win-x64\native ao lado do exe.
/// No publish single-file elas vão embutidas no exe e são extraídas uma vez para
/// %LOCALAPPDATA%\Littera\native\&lt;versão&gt;, porque o Scintilla.NET só carrega DLLs do disco.
/// </summary>
internal static class NativeLibraryBootstrap
{
    private static readonly string[] Libraries = ["Scintilla.dll", "Lexilla.dll"];

    public static void Configure()
    {
        string rid = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X86 => "win-x86",
            Architecture.Arm64 => "win-arm64",
            _ => "win-x64",
        };

        string besideExe = Path.Combine(AppContext.BaseDirectory, "runtimes", rid, "native");
        if (Libraries.All(lib => File.Exists(Path.Combine(besideExe, lib))))
            return;

        Assembly assembly = typeof(NativeLibraryBootstrap).Assembly;
        if (assembly.GetManifestResourceInfo("native." + Libraries[0]) is null)
            return; // Sem DLLs embutidas: deixa o Scintilla.NET seguir a busca padrão.

        // As DLLs só mudam quando o pacote Scintilla5.NET muda: a pasta é por versão dele, não do app.
        string version = typeof(ScintillaNativeLibrary).Assembly.GetName().Version?.ToString() ?? "0";
        string nativeRoot = Path.Combine(Notas.Services.DataFolders.Local, "native");
        string target = Path.Combine(nativeRoot, $"{version}-{rid}");

        bool extracted = false;
        foreach (string lib in Libraries)
        {
            using Stream embedded = assembly.GetManifestResourceStream("native." + lib)
                ?? throw new InvalidOperationException($"Recurso ausente: native.{lib}");

            string destination = Path.Combine(target, lib);
            if (SameContent(embedded, destination))
                continue;

            embedded.Position = 0;
            Extract(embedded, destination);
            extracted = true;
        }

        if (extracted)
            DeleteOtherVersions(nativeRoot, target);

        // A cópia em disco foi comparada byte a byte com a embutida no exe; a verificação
        // Authenticode do Scintilla.NET custaria ~55 ms a cada abertura sem proteger nada a mais.
        ScintillaNativeLibrary.VerifyScintillaDll = false;
        ScintillaNativeLibrary.VerifyLexillaDll = false;
        ScintillaNativeLibrary.SatelliteDirectory = target;
    }

    /// <summary>
    /// Carrega Scintilla.dll/Lexilla.dll numa thread de fundo enquanto a UI inicializa.
    /// Quem criar o controle depois só espera o que faltar (o CLR serializa o construtor estático).
    /// </summary>
    public static void PreloadInBackground() =>
        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            try
            {
                RuntimeHelpers.RunClassConstructor(typeof(Scintilla).TypeHandle);
                Notas.Services.StartupMetrics.Mark("t_preload_done");
            }
            catch (TypeInitializationException)
            {
                // A mesma exceção reaparece na thread de UI ao criar o controle, onde é tratada.
            }
        }, null);

    private static bool SameContent(Stream embedded, string path)
    {
        var info = new FileInfo(path);
        if (!info.Exists || info.Length != embedded.Length)
            return false;

        byte[] a = ArrayPool<byte>.Shared.Rent(1 << 16);
        byte[] b = ArrayPool<byte>.Shared.Rent(1 << 16);
        try
        {
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1, FileOptions.SequentialScan);
            int read;
            while ((read = embedded.ReadAtLeast(a.AsSpan(0, 1 << 16), 1 << 16, throwOnEndOfStream: false)) > 0)
            {
                if (file.ReadAtLeast(b.AsSpan(0, read), read, throwOnEndOfStream: false) != read
                    || !a.AsSpan(0, read).SequenceEqual(b.AsSpan(0, read)))
                    return false;
            }

            return true;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(a);
            ArrayPool<byte>.Shared.Return(b);
        }
    }

    private static void Extract(Stream source, string destination)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

        // Grava em arquivo temporário e renomeia: duas instâncias abrindo ao mesmo tempo não corrompem a DLL.
        string temp = destination + "." + Environment.ProcessId + ".tmp";
        using (var output = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            source.CopyTo(output);

        try
        {
            File.Move(temp, destination, overwrite: true);
        }
        catch (IOException) when (File.Exists(destination))
        {
            File.Delete(temp); // Outra instância já colocou a DLL (e pode estar com ela carregada).
        }
    }

    private static void DeleteOtherVersions(string nativeRoot, string keep)
    {
        foreach (string dir in Directory.EnumerateDirectories(nativeRoot))
        {
            if (string.Equals(dir, keep, StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                Directory.Delete(dir, recursive: true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Versão antiga ainda aberta em outra instância: fica para a próxima limpeza.
            }
        }
    }
}
