using System.Diagnostics;
using System.Runtime.InteropServices;
using Littera.Services;

namespace Littera.UI;

internal sealed partial class FolderPanel
{
    /// <summary>Botão direito numa linha: abrir, mostrar no Explorador e copiar o caminho.</summary>
    private void ShowRowMenu(FolderEntry entry, Point location)
    {
        ContextMenuStrip menu = Menus.Create(this);
        menu.ShowCheckMargin = false;
        if (!entry.IsDirectory)
            menu.Items.Add("Abrir", null, (_, _) => FileRequested?.Invoke(entry.Path));
        menu.Items.Add("Mostrar no Explorador", null, (_, _) => Reveal(entry.Path));
        menu.Items.Add("Copiar caminho", null, (_, _) => CopyPath(entry.Path));
        menu.Show(this, location);
    }

    private static void Reveal(string path) =>
        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"") { UseShellExecute = true })?.Dispose();

    private static void CopyPath(string path)
    {
        try
        {
            Clipboard.SetText(path);
        }
        catch (ExternalException)
        {
            // Área de transferência ocupada por outro programa: o usuário tenta de novo.
        }
    }
}
