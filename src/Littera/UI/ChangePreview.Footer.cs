using Notas.Theme;

namespace Notas.UI;

internal sealed partial class ChangePreview
{
    /// <summary>"Comparado com a última versão salva" e o atalho para a próxima alteração.</summary>
    private void PaintFooter(Graphics g, int dpi)
    {
        var footer = Rectangle.FromLTRB(this.Px(12), Height - this.Px(FooterHeight), Width - this.Px(12), Height);
        Font small = UiFonts.Get(11, dpi);
        TextRenderer.DrawText(g, "Comparado com a última versão salva", small, footer, Palette.TextFaint, Line);
        if (string.IsNullOrEmpty(Shortcut))
            return;

        const string Hint = "próxima alteração";
        int hintWidth = TextRenderer.MeasureText(g, Hint, small, Size.Empty, TextFormatFlags.NoPadding).Width;
        TextRenderer.DrawText(g, Hint, small, Rectangle.FromLTRB(footer.Right - hintWidth, footer.Y, footer.Right, footer.Bottom), Palette.TextFaint, Line);
        Font keys = UiFonts.Get(10.5f, dpi);
        int width = TextRenderer.MeasureText(g, Shortcut, keys, Size.Empty, TextFormatFlags.NoPadding).Width + this.Px(10);
        var cap = new Rectangle(footer.Right - hintWidth - this.Px(6) - width, footer.Y + (footer.Height - this.Px(16)) / 2, width, this.Px(16));
        Shapes.FillRounded(g, Palette.Border, cap, this.Px(3));
        cap.Inflate(-1, -1);
        Shapes.FillRounded(g, Palette.Background, cap, this.Px(3));
        TextRenderer.DrawText(g, Shortcut, keys, cap, Palette.TextMuted, Line | TextFormatFlags.HorizontalCenter);
    }
}
