using Littera.Editor;
using Littera.Theme;

namespace Littera.UI;

internal sealed partial class OpenFilesPanel
{
    /// <summary>Todas as abas em ordem (as fixadas primeiro), já com a rolagem de cada seção.</summary>
    private IEnumerable<TabSlot> Slots()
    {
        IReadOnlyList<OpenDocument> documents = _documents.Documents;
        int pinned = _documents.PinnedCount;
        Rectangle pinnedArea = PinnedArea, openArea = OpenArea;
        for (int i = 0; i < documents.Count; i++)
        {
            bool isPinned = i < pinned;
            int top = isPinned ? pinnedArea.Top + i * this.Px(ItemStep) - _pinnedScroll
                : openArea.Top + (i - pinned) * this.Px(ItemStep) - _openScroll;
            yield return new TabSlot(documents[i], new Rectangle(this.Px(8), top, Width - this.Px(16), this.Px(ItemHeight)), isPinned);
        }
    }

    private TabSlot? SlotAt(Point p)
    {
        foreach (TabSlot slot in Slots())
        {
            if (slot.Bounds.Contains(p) && (slot.Pinned ? PinnedArea : OpenArea).Contains(p))
                return slot;
        }

        return null;
    }

    private TabSlot? FindSlot(OpenDocument document)
    {
        foreach (TabSlot slot in Slots())
        {
            if (slot.Document == document)
                return slot;
        }

        return null;
    }
}
