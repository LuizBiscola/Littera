using Notas.Theme;

namespace Notas.UI.Find;

internal sealed partial class FindPanel
{
    private Rectangle _findBox;

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        int y = this.Px(8), toggle = this.Px(22), icon = this.Px(24);
        _findBox = new Rectangle(this.Px(32), y, this.Px(274), this.Px(30));
        PlaceText(_find, _findBox, this.Px(84));

        _regex.Bounds = new Rectangle(_findBox.Right - this.Px(26), y + this.Px(4), toggle, toggle);
        _wholeWord.Bounds = new Rectangle(_regex.Bounds.X - this.Px(24), _regex.Bounds.Y, toggle, toggle);
        _matchCase.Bounds = new Rectangle(_wholeWord.Bounds.X - this.Px(24), _regex.Bounds.Y, toggle, toggle);

        _close.Bounds = new Rectangle(Width - this.Px(32), y + this.Px(3), icon, icon);
        _next.Bounds = new Rectangle(_close.Bounds.X - this.Px(28), _close.Bounds.Y, icon, icon);
        _previous.Bounds = new Rectangle(_next.Bounds.X - this.Px(26), _close.Bounds.Y, icon, icon);
        LayoutRows();
    }

    /// <summary>Linhas de baixo (substituir e escopo) e a altura final.</summary>
    partial void LayoutRows();

    private void PlaceText(TextBox box, Rectangle frame, int reservedRight)
    {
        box.Font = UiFonts.Get(13, DeviceDpi);
        int height = box.PreferredHeight;
        box.SetBounds(frame.X + this.Px(8), frame.Y + (frame.Height - height) / 2, frame.Width - reservedRight, height);
    }

    /// <summary>Canto superior direito da área do editor, como no protótipo.</summary>
    private void PlaceInParent()
    {
        if (Parent is null)
            return;
        Width = this.Px(460);
        Location = new Point(Parent.ClientSize.Width - Width - this.Px(24), this.Px(10));
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (Parent is not null)
            Parent.Resize += (_, _) => PlaceInParent();
        PlaceInParent();
    }
}
