using Littera.Editor;
using Littera.Theme;

namespace Littera.UI;

/// <summary>Uma aba na tela: retângulo e seção.</summary>
internal readonly record struct TabSlot(OpenDocument Document, Rectangle Bounds, bool Pinned);

internal sealed partial class OpenFilesPanel
{
    private int _pinnedScroll, _openScroll;

    /// <summary>Linhas das fixadas; acima de 40% da altura do painel, a seção ganha rolagem própria.</summary>
    private Rectangle PinnedArea
    {
        get
        {
            int count = _documents.PinnedCount;
            if (count == 0)
                return Rectangle.Empty;
            int max = Math.Max(this.Px(ItemStep), (int)((Height - this.Px(FooterHeight)) * 0.4));
            return new Rectangle(0, this.Px(PinnedHeaderHeight), Width, Math.Min(count * this.Px(ItemStep), max));
        }
    }

    private int OpenHeaderTop => _documents.PinnedCount == 0 ? 0 : PinnedArea.Bottom + this.Px(SeparatorHeight);

    private int OpenHeaderBottom => OpenHeaderTop + this.Px(_documents.PinnedCount == 0 ? SoloHeaderHeight : OpenHeaderHeight);

    private Rectangle OpenArea => Rectangle.FromLTRB(0, OpenHeaderBottom, Width, Height - this.Px(FooterHeight));

    private Rectangle NewButtonBounds => new(Width - this.Px(30), OpenHeaderTop + this.Px(14), this.Px(20), this.Px(20));

    private Rectangle CloseBounds(Rectangle row) => new(row.Right - this.Px(34), row.Top + this.Px(6), this.Px(20), this.Px(20));

    /// <summary>Alfinete: no lugar do × numa fixada; à esquerda do × numa aba comum (só com o mouse em cima).</summary>
    private Rectangle PinBounds(TabSlot slot)
    {
        Rectangle close = CloseBounds(slot.Bounds);
        return slot.Pinned ? close : new Rectangle(close.Left - this.Px(22), close.Top, close.Width, close.Height);
    }

    private void ClampScroll()
    {
        Rectangle pinned = PinnedArea, open = OpenArea;
        _pinnedScroll = Math.Clamp(_pinnedScroll, 0, Math.Max(0, _documents.PinnedCount * this.Px(ItemStep) - pinned.Height));
        int openCount = _documents.Documents.Count - _documents.PinnedCount;
        _openScroll = Math.Clamp(_openScroll, 0, Math.Max(0, openCount * this.Px(ItemStep) + this.Px(4) - open.Height));
    }
}
