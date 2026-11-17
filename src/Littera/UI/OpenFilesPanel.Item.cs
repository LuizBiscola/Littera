using System.Drawing.Drawing2D;
using Littera.Editor;
using Littera.Theme;

namespace Littera.UI;

internal enum TabButton
{
    None,
    Pin,
    Close,
}

internal sealed partial class OpenFilesPanel
{
    private const TextFormatFlags LeftText = TextFormatFlags.Left | TextFormatFlags.VerticalCenter
        | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;

    /// <summary>Estados da especificação: repouso, mouse em cima, ativa, não salva, fixada, arquivo sumido.</summary>
    private void PaintItem(Graphics g, TabSlot slot, int dpi)
    {
        OpenDocument document = slot.Document;
        Rectangle item = slot.Bounds;
        bool active = document == _documents.Active;
        bool hover = document == _hoverDocument && _drag is null;
        if (active || hover)
            Shapes.FillRounded(g, active ? Palette.ItemActive : Palette.ItemHover, item, this.Px(6));
        PaintFlash(g, document, item);

        var icon = new Rectangle(item.X + this.Px(10), item.Y + this.Px(9), this.Px(15), this.Px(15));
        TabIcon(document).Draw(g, icon, document.Missing ? Palette.MissingIcon : active ? Palette.IconActive : Palette.IconInactive);

        int left = PaintButtons(g, slot, hover, dpi);
        if (document.IsDirty)
        {
            using var dirty = new SolidBrush(Palette.Dirty);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillEllipse(dirty, left - this.Px(15), item.Y + this.Px(13), this.Px(7), this.Px(7));
            g.SmoothingMode = SmoothingMode.Default;
            left -= this.Px(19);
        }

        Color color = document.Missing ? Palette.MissingText : document.IsLoading ? Palette.TextFaint : active ? Palette.Text : Palette.TextSecondary;
        Font font = UiFonts.Get(13, dpi, document.Missing ? FontStyle.Strikeout : FontStyle.Regular);
        TextRenderer.DrawText(g, document.Title, font, Rectangle.FromLTRB(item.X + this.Px(33), item.Y, left - this.Px(4), item.Bottom), color, LeftText);
    }

    /// <summary>Estados têm prioridade sobre a extensão: arquivo sumido, depois "Sem título".</summary>
    private static LineIcon TabIcon(OpenDocument document) =>
        document.Missing ? FileIcons.Missing : document.Path is null ? FileIcons.Untitled : FileKinds.For(document.Path);
}
