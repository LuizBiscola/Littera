using Notas.Settings;
using Notas.Theme;
using Notas.UI.Chrome;
using Notas.UI.Controls;

namespace Notas.UI.Settings;

/// <summary>Janela de configurações do protótipo: menu lateral, página à direita e rodapé com salvar/cancelar.</summary>
internal sealed partial class SettingsDialog : Form
{
    private readonly IReadOnlyList<SettingsPage> _pages;
    private readonly SettingsHeader _header;
    private readonly ScrollPanel _content = new() { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Palette.Background };
    private int _page;

    public SettingsDialog(AppSettings settings, IReadOnlyList<SettingsPage> pages)
    {
        SuspendLayout();
        Draft = SettingsCopy.Clone(settings);
        _pages = pages;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        KeyPreview = true;
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Palette.Background;
        Size = new Size(this.Px(820), this.Px(620));

        // Ordem importa: o WinForms encaixa primeiro o último adicionado (menu à esquerda, depois topo e rodapé).
        _content.Padding = new Padding(this.Px(28), 0, this.Px(28), this.Px(12));
        Controls.Add(_content);
        Controls.Add(CreateFooter());
        Controls.Add(_header = new SettingsHeader(Close));
        var nav = new SettingsNav(pages);
        nav.Selected += ShowPage;
        Controls.Add(nav);
        ResumeLayout(false);
        ShowPage(0);
    }

    /// <summary>Cópia editada; quem abriu aplica se o resultado for OK.</summary>
    public AppSettings Draft { get; }

    /// <summary>A página velha fica parada na tela até a nova estar montada (sem meio-termo em branco ou pela metade).</summary>
    private void ShowPage(int index)
    {
        _page = index;
        _header.SetPage(_pages[index]);
        bool freeze = _content.IsHandleCreated;
        if (freeze)
            Win32.SetRedraw(_content.Handle, false);
        _content.SuspendLayout();
        foreach (Control old in _content.Controls.Cast<Control>().ToList())
            old.Dispose();

        // Dock = Top empilha de baixo para cima: adiciona em ordem inversa.
        foreach (Control row in _pages[index].Build(Draft).Reverse())
        {
            row.Dock = DockStyle.Top;
            _content.Controls.Add(row);
        }

        _content.ResumeLayout();
        _content.AutoScrollPosition = Point.Empty;
        if (!freeze)
            return;
        Win32.SetRedraw(_content.Handle, true);
        Win32.RedrawNow(_content.Handle);
    }

    /// <summary>Sem foco inicial: o primeiro campo (a fonte) rolaria a página e ficaria com o texto selecionado.</summary>
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        ActiveControl = null;
        _content.AutoScrollPosition = Point.Empty;
    }
}
