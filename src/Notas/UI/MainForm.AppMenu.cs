namespace Notas.UI;

internal sealed partial class MainForm
{
    private const int WM_SYSCOMMAND = 0x0112;
    private const int SC_KEYMENU = 0xF100;

    /// <summary>Menu do app (o protótipo esconde a barra "Arquivo/Editar"): abre pela marca ou com Alt.</summary>
    private void ShowAppMenu(Rectangle anchor)
    {
        ContextMenuStrip menu = Menus.Create(this);
        menu.ShowCheckMargin = false;
        Add(menu.Items, "Novo", "Ctrl+N", () => _documents.New());
        Add(menu.Items, "Abrir…", "Ctrl+O", ShowOpenDialog);
        menu.Items.Add(RecentMenu(menu));
        menu.Items.Add(new ToolStripSeparator());
        Add(menu.Items, "Salvar", "Ctrl+S", () => _ = SaveActiveAsync(askPath: false));
        Add(menu.Items, "Salvar como…", "Ctrl+Shift+S", () => _ = SaveActiveAsync(askPath: true));
        Add(menu.Items, "Fechar aba", "Ctrl+W", () => Shortcuts[Keys.Control | Keys.W]());
        menu.Items.Add(new ToolStripSeparator());
        Add(menu.Items, "Formatar SQL", "Ctrl+Shift+F", FormatSql);
        Add(menu.Items, "Modo foco", "F11", ToggleFocusMode);
        menu.Items.Add(new ToolStripSeparator());
        Add(menu.Items, "Configurações…", "Ctrl+,", ShowSettings);
        Add(menu.Items, "Sair", "Alt+F4", Close);
        menu.Show(new Point(anchor.Left, anchor.Bottom + 4));
    }

    private ToolStripMenuItem RecentMenu(ContextMenuStrip owner)
    {
        var recent = new ToolStripMenuItem("Abrir recente") { Enabled = _settings.RecentFiles.Count > 0 };
        recent.DropDown.Renderer = owner.Renderer;
        foreach (string path in _settings.RecentFiles)
            Add(recent.DropDownItems, Path.GetFileName(path), Path.GetDirectoryName(path), () => OpenFile(path)); // pasta à direita, em cinza
        return recent;
    }

    private static void Add(ToolStripItemCollection items, string text, string? hint, Action action) =>
        items.Add(new ToolStripMenuItem(text, null, (_, _) => action()) { ShortcutKeyDisplayString = hint });

    /// <summary>Alt solto sozinho: o Windows pediria o menu do sistema; abrimos o nosso.</summary>
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_SYSCOMMAND && ((int)m.WParam & 0xFFF0) == SC_KEYMENU && m.LParam == IntPtr.Zero)
        {
            TitleBar.OpenMenu();
            return;
        }

        base.WndProc(ref m);
    }
}
