using Littera.Theme;
using Littera.UI.Controls;

namespace Littera.UI.Settings;

internal sealed partial class SettingsDialog
{
    private Panel CreateFooter()
    {
        var footer = new Panel { Dock = DockStyle.Bottom, Height = this.Px(60), BackColor = Palette.Background };
        footer.Paint += (_, e) =>
        {
            using var line = new Pen(Palette.Border);
            e.Graphics.DrawLine(line, 0, 0, footer.Width, 0);
        };

        var reset = new FlatButton("Restaurar padrões", FlatButtonKind.Link);
        var cancel = new FlatButton("Cancelar", FlatButtonKind.Secondary);
        var save = new FlatButton("Salvar", FlatButtonKind.Primary);
        reset.Click += (_, _) => ResetPage();
        cancel.Click += (_, _) => Close();
        save.Click += (_, _) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        footer.Controls.AddRange(reset, cancel, save);
        footer.Resize += (_, _) =>
        {
            int top = (footer.Height - save.Height) / 2;
            reset.Location = new Point(this.Px(24), top);
            save.Location = new Point(footer.Width - save.Width - this.Px(20), top);
            cancel.Location = new Point(save.Left - cancel.Width - this.Px(10), top);
        };
        return footer;
    }

    /// <summary>Volta só a página aberta aos valores de fábrica.</summary>
    private void ResetPage()
    {
        _pages[_page].Reset(Draft);
        ShowPage(_page);
    }
}
