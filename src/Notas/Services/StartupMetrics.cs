using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Notas.Services;

/// <summary>
/// Métricas de desempenho, ativadas só quando a variável NOTAS_METRICS aponta para um arquivo.
/// Cada linha gravada é "chave=valor"; o tools\measure.ps1 lê esse arquivo.
/// </summary>
internal static class StartupMetrics
{
    private static readonly StringBuilder Pending = new();
    private static string? _file;
    private static long _processCreatedUtc;

    public static bool Enabled => _file is not null;

    public static void Init()
    {
        string? path = Environment.GetEnvironmentVariable("NOTAS_METRICS");
        if (string.IsNullOrWhiteSpace(path))
            return;

        _file = path;
        GetProcessTimes(GetCurrentProcess(), out _processCreatedUtc, out _, out _, out _);
    }

    /// <summary>Milissegundos desde a criação do processo (relógio do kernel, inclui o carregamento do runtime).</summary>
    public static double SinceProcessStart() =>
        (DateTime.UtcNow.ToFileTimeUtc() - _processCreatedUtc) / 10_000.0;

    /// <summary>Registra o tempo de uma etapa; só vai para o disco no próximo <see cref="Write"/>.</summary>
    public static void Mark(string key)
    {
        if (_file is not null)
            Append(key, SinceProcessStart());
    }

    public static void Write(string key, double value)
    {
        if (_file is null)
            return;

        Append(key, value);
        lock (Pending)
        {
            File.AppendAllText(_file, Pending.ToString());
            Pending.Clear();
        }
    }

    private static void Append(string key, double value)
    {
        lock (Pending)
            Pending.Append(key).Append('=').Append(value.ToString("F1", CultureInfo.InvariantCulture)).AppendLine();
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetProcessTimes(IntPtr process, out long creation, out long exit, out long kernel, out long user);
}
