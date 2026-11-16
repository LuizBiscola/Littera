using Notas.Theme;

namespace Notas.UI;

internal sealed partial class MenuRenderer
{
    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        int y = e.Item.Height / 2;
        using var line = new Pen(Palette.Border);
        e.Graphics.DrawLine(line, Px(10), y, e.Item.Width - Px(10), y);
    }

    /// <summary>Vermelho fica vermelho; o resto vira azul com o mouse em cima; item azul (Fixar aba) é sempre azul.</summary>
    private static Color StateColor(ToolStripItem item, Color normal) =>
        item.ForeColor == Palette.Danger ? Palette.Danger
        : item.Selected || item.ForeColor == Palette.Accent ? Palette.Accent
        : normal;
}
