namespace Littera.Editor;

internal sealed partial class DocumentManager
{
    /// <summary>As fixadas ficam sempre no começo da lista, então esta é também a posição da primeira aba comum.</summary>
    public int PinnedCount => _documents.Count(d => d.Pinned);

    /// <summary>Fixar leva a aba para o fim das Fixadas; desafixar, para o topo das Abertas.</summary>
    public void SetPinned(OpenDocument document, bool pinned)
    {
        if (document.Pinned == pinned || !_documents.Remove(document))
            return;
        document.Pinned = pinned;
        _documents.Insert(PinnedCount, document); // fim das fixadas = começo das abertas
        Changed?.Invoke();
    }

    /// <summary>Arrastar: posição dentro da seção de destino; soltar na outra seção fixa ou desafixa.</summary>
    public void Move(OpenDocument document, bool pinned, int index)
    {
        if (!_documents.Remove(document))
            return;
        document.Pinned = pinned;
        int pinnedCount = PinnedCount;
        int start = pinned ? 0 : pinnedCount;
        int size = pinned ? pinnedCount : _documents.Count - pinnedCount;
        _documents.Insert(start + Math.Clamp(index, 0, size), document);
        Changed?.Invoke();
    }
}
