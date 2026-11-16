using Notas.Settings;
using Notas.Theme;

namespace Notas.UI.Settings;

/// <summary>Janela de configurações do protótipo: menu lateral, página à direita e rodapé com salvar/cancelar.</summary>
internal sealed partial class SettingsDialog : Form
{
    private readonly IReadOnlyList<SettingsPage> _pages;
    private readonly SettingsHeader _header;
    private readonly Panel _content = new() { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Palette.Background };
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

    private void ShowPage(int index)
    {
        _page = index;
        _header.SetPage(_pages[index]);
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
    }
}
