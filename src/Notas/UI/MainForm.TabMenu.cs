using Notas.Editor;
using Notas.Theme;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Clique direito na aba (protótipo "Menu da aba"). Em aba fixada, "Fechar" funciona por ser escolha explícita.</summary>
    private void ShowTabMenu(OpenDocument document, Point screen)
    {
        ContextMenuStrip menu = Menus.Create(this);
        menu.ShowCheckMargin = false;
        menu.ShowImageMargin = true;
        menu.ImageScalingSize = new Size(this.Px(16), this.Px(16));
        bool onDisk = document.Path is not null;
        AddTabItem(menu, document.Pinned ? Icons.Unpin : Icons.Pin, document.Pinned ? "Desafixar aba" : "Fixar aba", "fixarAba",
            () => _openFiles.TogglePin(document), accent: true);
        menu.Items.Add(new ToolStripSeparator());
        AddTabItem(menu, Icons.Dismiss, "Fechar", "fecharAba", () => _ = RequestCloseAsync(document));
        AddTabItem(menu, Icons.DismissThin, "Fechar outras", "fecharOutras", () => _ = CloseOthersAsync(document));
        AddTabItem(menu, Icons.CheckMark, "Fechar abas salvas", "fecharSalvas", CloseSavedTabs);
        menu.Items.Add(new ToolStripSeparator());
        AddTabItem(menu, Icons.Clipboard, "Copiar caminho", "copiarCaminho", () => CopyPath(document), onDisk);
        AddTabItem(menu, Icons.Folder, "Mostrar na pasta", "mostrarNaPasta", () => RevealInFolder(document), onDisk && !document.Missing);
        AddTabItem(menu, Icons.OpenOutside, "Abrir no Explorer", null, () => OpenInExplorer(document), onDisk && !document.Missing);
        menu.Items.Add(new ToolStripSeparator());
        AddTabItem(menu, Icons.Edit, "Renomear arquivo…", "renomearArquivo", () => RenameTab(document), onDisk && !document.Missing);
        AddTabItem(menu, Icons.Copy, "Duplicar em nova aba", "duplicarAba", () => DuplicateTab(document));
        menu.Items.Add(new ToolStripSeparator());
        AddTabItem(menu, Icons.Undo, "Descartar alterações", "descartarAlteracoes", () => DiscardChanges(document), document.IsDirty, danger: true);
        menu.Show(screen);
    }

    private void AddTabItem(ContextMenuStrip menu, char glyph, string text, string? command, Action action,
        bool enabled = true, bool accent = false, bool danger = false)
    {
        Color color = accent ? Palette.Accent : danger ? Palette.Danger : Palette.Text;
        Color icon = accent || danger ? color : Palette.TextMuted;
        menu.Items.Add(new ToolStripMenuItem(text, Menus.Glyph(glyph, enabled ? icon : Palette.TextFaint, this.Px(16), DeviceDpi), (_, _) => action())
        {
            Enabled = enabled,
            ForeColor = color,
            ShortcutKeyDisplayString = command is null ? null : ShortcutText(command),
        });
    }
}
