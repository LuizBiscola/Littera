namespace Littera.Services;

/// <summary>Erros inesperados: grava em %LOCALAPPDATA%\Littera\erros.log e avisa sem derrubar o editor.</summary>
internal static class ErrorReport
{
    public static string LogPath { get; } = Path.Combine(DataFolders.Local, "erros.log");

    public static void Show(Exception error, IWin32Window? owner = null)
    {
        Log(error);
        MessageBox.Show(owner, $"{error.Message}\n\nDetalhes em {LogPath}", $"{Product.Name} — erro inesperado",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void Log(Exception error)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {error}{Environment.NewLine}{Environment.NewLine}");
        }
        catch (IOException)
        {
            // Sem como registrar: o aviso na tela ainda aparece.
        }
    }
}
