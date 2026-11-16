using Notas.Theme;
using Notas.UI.Chrome;

namespace Notas.UI;

/// <summary>
/// Menus do protótipo ("Menu da aba"): fundo branco, item em destaque azul-claro arredondado com texto azul,
/// atalho em cinza claro, ícone desenhado na cor do estado e separador recuado. Cantos e sombra: Windows 11.
/// </summary>
internal sealed partial class MenuRenderer(int dpi) : ToolStripProfessionalRenderer(new MenuColors())
{
    private static readonly LineIcon SubmenuArrow = new("m9 18 6-6-6-6"); // chevron-right

    private int Px(int px) => (int)Math.Round(px * dpi / 96.0);

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        using var back = new SolidBrush(Palette.Background);
        e.Graphics.FillRectangle(back, e.AffectedBounds);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        if (Win32.IsWindows11)
            return; // a borda arredondada é do sistema
        using var border = new Pen(Palette.Border);
        e.Graphics.DrawRectangle(border, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        // Sem a coluna cinza dos ícones.
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item.Selected && e.Item.Enabled)
            Shapes.FillRounded(e.Graphics, Palette.AccentSoft, Highlight(e.Item), Px(5));
    }

    /// <summary>Rótulo e atalho (11 px, cinza) reposicionados nas medidas do protótipo.</summary>
    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        bool shortcut = e.Item is ToolStripMenuItem item && e.Text == item.ShortcutKeyDisplayString;
        if (e.Item is ToolStripMenuItem)
            e.TextColor = !e.Item.Enabled || shortcut ? Palette.TextFaint : StateColor(e.Item, Palette.Text);
        if (e.Item is ToolStripMenuItem && e.Item.Owner is ToolStripDropDownMenu) // título (ToolStripLabel) já vem alinhado ao texto
        {
            Rectangle text = e.TextRectangle;
            if (shortcut)
                e.TextFont = UiFonts.Get(11, dpi);
            // O layout centraliza a linha sem contar o Padding do item (texto ficava 5 px acima); o rótulo vai até
            // a coluna do atalho: andou para a direita e não pode cortar.
            int left = shortcut ? text.Left : TextLeft(e.Item);
            e.TextRectangle = new Rectangle(left, (e.Item.Height - text.Height) / 2, ShortcutRight(e.Item) - left, text.Height);
        }

        base.OnRenderItemText(e);
    }

    /// <summary>Submenu: chevron-right do Lucide, centrado na altura e alinhado à coluna dos atalhos.</summary>
    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        if (e.Item is not { Owner: ToolStripDropDownMenu } item)
        {
            base.OnRenderArrow(e);
            return;
        }

        var box = new Rectangle(ShortcutRight(item) - Px(14), (item.Height - Px(14)) / 2, Px(14), Px(14));
        SubmenuArrow.Draw(e.Graphics, box, item.Enabled ? StateColor(item, Palette.TextFaint) : Palette.TextFaint);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) =>
        Icons.Draw(e.Graphics, Icons.CheckMark, IconBox(e.Item), Palette.Accent, 11, dpi);

    /// <summary>Ícone guardado em Tag (glifo da Segoe Fluent): cinza, azul em destaque, vermelho em ação perigosa.</summary>
    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
        if (e.Item.Tag is not char glyph)
        {
            base.OnRenderItemImage(e);
            return;
        }

        Color color = e.Item.Enabled ? StateColor(e.Item, Palette.TextMuted) : Palette.TextFaint;
        Icons.Draw(e.Graphics, glyph, IconBox(e.Item), color, 12, dpi); // 14 px no protótipo
    }
}
