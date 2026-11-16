using Notas.Editor;
using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    /// <summary>Soltou: muda de lugar e, se mudou de seção, fixa ou desafixa (com a confirmação azul ao fixar).</summary>
    private bool EndDrag()
    {
        OpenDocument? dragged = _drag;
        (bool Pinned, int Index)? drop = _drop;
        _pressed = _drag = null;
        _drop = null;
        if (dragged is null)
            return false;

        if (drop is { } target)
        {
            bool pinning = target.Pinned && !dragged.Pinned;
            _documents.Move(dragged, target.Pinned, target.Index);
            if (pinning)
                Flash(dragged);
        }

        Invalidate();
        return true;
    }

    /// <summary>Acima do cabeçalho de Abertos é a seção Fixadas (quando existe); a posição conta as abas acima do mouse.</summary>
    private (bool Pinned, int Index) DropAt(Point p)
    {
        bool pinned = _documents.PinnedCount > 0 && p.Y < OpenHeaderTop;
        int index = 0;
        foreach (TabSlot slot in Slots())
        {
            if (slot.Pinned == pinned && slot.Document != _drag && slot.Bounds.Top + (slot.Bounds.Height / 2) < p.Y)
                index++;
        }

        return (pinned, index);
    }
}
