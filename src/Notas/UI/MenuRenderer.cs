using Notas.Theme;
using Notas.UI.Chrome;

namespace Notas.UI;

/// <summary>
/// Menus do protótipo ("Menu da aba"): fundo branco, item em destaque azul-claro arredondado com texto azul,
/// atalho em cinza claro, ícone desenhado na cor do estado e separador recuado. Cantos e sombra: Windows 11.
/// </summary>
internal sealed partial class MenuRenderer(int dpi) : ToolStripProfessionalRenderer(new MenuColors())
{
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
            Shapes.FillRounded(e.Graphics, Palette.AccentSoft, new Rectangle(Px(2), 0, e.Item.Width - Px(4), e.Item.Height), Px(6));
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        bool shortcut = e.Item is ToolStripMenuItem item && e.Text == item.ShortcutKeyDisplayString;
        e.TextColor = !e.Item.Enabled || shortcut ? Palette.TextFaint : StateColor(e.Item, Palette.Text);
        base.OnRenderItemText(e);
    }

    /// <summary>Ícone guardado em Tag (glifo da Segoe Fluent): cinza, azul em destaque, vermelho em ação perigosa.</summary>
    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
        if (e.Item.Tag is not char glyph)
        {
            base.OnRenderItemImage(e);
            return;
        }

        Color color = e.Item.Enabled ? StateColor(e.Item, Palette.TextMuted) : Palette.TextFaint;
        Icons.Draw(e.Graphics, glyph, e.ImageRectangle, color, 12, dpi); // 14 px no protótipo
    }
}
