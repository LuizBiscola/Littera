using Littera.Editor;
using Littera.Services;
using Littera.Theme;

namespace Littera.UI;

internal sealed partial class FolderPanel
{
    private void PaintRow(Graphics g, int index, int dpi)
    {
        Rectangle row = RowBounds(index);
        if (row.Bottom < TreeArea.Top || row.Top > TreeArea.Bottom)
            return;

        (FolderEntry entry, int depth) = (_rows[index].Entry, _rows[index].Depth);
        OpenDocument? open = entry.IsDirectory ? null : _documents.Documents.FirstOrDefault(d => SamePath(d.Path, entry.Path));
        bool active = open is not null && open == _documents.Active;
        if (active || index == _hoverIndex || index == _keyIndex)
            Shapes.FillRounded(g, active ? Palette.AccentSoft : index == _keyIndex ? Palette.ItemActive : Palette.ItemHover, row, this.Px(5));

        int x = row.X + this.Px(16 * depth);
        Font font = UiFonts.Get(12.5f, dpi);
        var text = Rectangle.FromLTRB(x + this.Px(entry.IsDirectory ? 46 : 45), row.Y, row.Right - this.Px(20), row.Bottom);
        if (entry.IsDirectory)
        {
            bool expanded = _expanded.Contains(entry.Path);
            Icons.Draw(g, expanded ? Icons.ChevronDownSmall : Icons.ChevronRightSmall,
                new Rectangle(x + this.Px(6), row.Y + this.Px(8), this.Px(13), this.Px(13)), Palette.TextFaint, 9, dpi);
            (expanded ? FileIcons.FolderOpen : FileIcons.Folder).Draw(g,
                new Rectangle(x + this.Px(25), row.Y + this.Px(7), this.Px(15), this.Px(15)), Palette.FolderIcon);
            TextRenderer.DrawText(g, entry.Name, font, text, Palette.Text, Line);
            return;
        }

        FileKinds.For(entry.Path).Draw(g, new Rectangle(x + this.Px(25), row.Y + this.Px(7), this.Px(14), this.Px(14)),
            active ? Palette.Accent : Palette.TextFaint);
        TextRenderer.DrawText(g, entry.Name, font, text, active ? Palette.Accent : Palette.TextSecondary, Line);
        if (Filtering)
            PaintLocation(g, entry, font, text, dpi);
        if (open is not null)
            PaintDot(g, row, open.IsDirty ? Palette.Dirty : active ? Palette.Accent : Palette.OpenInTab, open.IsDirty ? 7 : 5);
    }
}
