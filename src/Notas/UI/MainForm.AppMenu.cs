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
        Add(menu.Items, "Paleta de comandos…", ShortcutText("paletaDeComandos"), ShowCommandPalette);
        menu.Items.Add(new ToolStripSeparator());
        Add(menu.Items, "Novo", ShortcutText("novo"), () => _documents.New());
        Add(menu.Items, "Abrir…", ShortcutText("abrir"), ShowOpenDialog);
        Add(menu.Items, "Abrir pasta…", ShortcutText("abrirPasta"), ShowOpenFolderDialog);
        menu.Items.Add(RecentMenu(menu));
        menu.Items.Add(new ToolStripSeparator());
        Add(menu.Items, "Salvar", ShortcutText("salvar"), () => _ = SaveActiveAsync(askPath: false));
        Add(menu.Items, "Salvar como…", ShortcutText("salvarComo"), () => _ = SaveActiveAsync(askPath: true));
        Add(menu.Items, "Fechar aba", ShortcutText("fecharAba"), CloseActive);
        menu.Items.Add(new ToolStripSeparator());
        Add(menu.Items, "Formatar SQL", ShortcutText("formatarSql"), FormatSql);
        Add(menu.Items, "Recolher tudo", ShortcutText("recolherTudo"), () => SetAllFolds(expand: false));
        Add(menu.Items, "Expandir tudo", ShortcutText("expandirTudo"), () => SetAllFolds(expand: true));
        Add(menu.Items, "Modo foco", ShortcutText("modoFoco"), ToggleFocusMode);
        menu.Items.Add(new ToolStripSeparator());
        Add(menu.Items, "Configurações…", ShortcutText("configuracoes"), ShowSettings);
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
