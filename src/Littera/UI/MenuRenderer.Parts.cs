using Littera.Theme;

namespace Littera.UI;

internal sealed partial class MenuRenderer
{
    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        int y = e.Item.Height / 2;
        using var line = new Pen(Palette.Border);
        e.Graphics.DrawLine(line, Px(11) - e.Item.Bounds.X, y, MenuWidth(e.Item) - Px(11) - e.Item.Bounds.X, y);
    }

    /// <summary>Vermelho fica vermelho; o resto vira azul com o mouse em cima; item azul (Fixar aba) é sempre azul.</summary>
    private static Color StateColor(ToolStripItem item, Color normal) =>
        item.ForeColor == Palette.Danger ? Palette.Danger
        : item.Selected || item.ForeColor == Palette.Accent ? Palette.Accent
        : normal;

    /// <summary>
    /// Medidas do protótipo contadas da borda do menu (o item começa no x 0 e vai até quase a borda direita):
    /// destaque recuado 5 px, ícone a 15 px, texto a 15 px (39 com coluna de ícone) e atalho terminando a 15 px da direita.
    /// </summary>
    private static int MenuWidth(ToolStripItem item) => item.Owner?.Width ?? item.Width;

    private Rectangle Highlight(ToolStripItem item) =>
        new(Px(5) - item.Bounds.X, 0, MenuWidth(item) - Px(10), item.Height);

    private Rectangle IconBox(ToolStripItem item) =>
        new(Px(15) - item.Bounds.X, (item.Height - Px(14)) / 2, Px(14), Px(14));

    /// <summary>2 px a menos: o texto do GDI já vem com essa folga antes da primeira letra e depois da última.</summary>
    private int TextLeft(ToolStripItem item) =>
        Px(item.Owner is ToolStripDropDownMenu { ShowImageMargin: true } or ToolStripDropDownMenu { ShowCheckMargin: true } ? 37 : 13)
        - item.Bounds.X;

    private int ShortcutRight(ToolStripItem item) => MenuWidth(item) - Px(13) - item.Bounds.X;
}
