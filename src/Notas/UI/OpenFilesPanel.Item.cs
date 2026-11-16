using System.Drawing.Drawing2D;
using Notas.Editor;
using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    private const TextFormatFlags LeftText = TextFormatFlags.Left | TextFormatFlags.VerticalCenter
        | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;

    private void PaintItem(Graphics g, int index, int dpi)
    {
        OpenDocument document = _documents.Documents[index];
        Rectangle item = ItemBounds(index);
        bool active = document == _documents.Active;
        bool hover = index == _hoverIndex;
        if (active || hover)
            Shapes.FillRounded(g, active ? Palette.ItemActive : Palette.ItemHover, item, this.Px(6));

        var icon = new Rectangle(item.X + this.Px(9), item.Y + this.Px(8), this.Px(16), this.Px(16));
        Icons.Draw(g, Icons.Document, icon, active ? Palette.IconActive : Palette.IconInactive, 13, dpi);

        Rectangle close = CloseBounds(item);
        int textRight = close.Left - this.Px(document.IsDirty ? 21 : 4);
        Color color = document.IsLoading ? Palette.TextFaint : active ? Palette.Text : Palette.TextSecondary;
        TextRenderer.DrawText(g, document.Title, UiFonts.Get(13, dpi),
            Rectangle.FromLTRB(item.X + this.Px(35), item.Y, textRight, item.Bottom), color, LeftText);

        if (document.IsDirty)
        {
            using var dirty = new SolidBrush(Palette.Dirty);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillEllipse(dirty, close.Left - this.Px(17), item.Y + this.Px(12), this.Px(7), this.Px(7));
            g.SmoothingMode = SmoothingMode.Default;
        }

        if (hover && _hoverClose)
            Shapes.FillRounded(g, Palette.Border, close, this.Px(4));
        Icons.Draw(g, Icons.Close, close, active || hover ? Palette.TextMuted : Palette.CloseInactive, 9, dpi);
    }
}
