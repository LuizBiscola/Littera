using System.Diagnostics;
using System.Runtime.InteropServices;
using Littera.Editor;

namespace Littera.UI;

internal sealed partial class MainForm
{
    private static void CopyPath(OpenDocument document)
    {
        try
        {
            if (document.Path is { } path)
                Clipboard.SetText(path);
        }
        catch (ExternalException)
        {
            // Área de transferência ocupada por outro programa: o usuário tenta de novo.
        }
    }

    private static void OpenInExplorer(OpenDocument document)
    {
        if (document.Path is { } path)
            Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"") { UseShellExecute = true })?.Dispose();
    }

    /// <summary>"Mostrar na pasta": arquivo fora da pasta aberta abre a pasta dele no painel.</summary>
    private void RevealInFolder(OpenDocument document)
    {
        if (document.Path is not { } path)
            return;
        if (_folderPanel?.Contains(path) == true)
            SetFolderShown(true);
        else
            OpenFolder(Path.GetDirectoryName(path)!);
        _folderPanel!.ShowFile(path);
    }

    /// <summary>Executa um comando de aba sobre a aba ativa (atalhos e paleta).</summary>
    private void OnActive(Action<OpenDocument> action)
    {
        if (_documents.Active is { } active)
            action(active);
    }
}
