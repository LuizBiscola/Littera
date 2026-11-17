using Littera.Editor;

namespace Littera.UI;

internal sealed partial class MainForm
{
    /// <summary>Fecha as outras abas comuns (pergunta antes das alteradas); fixadas ficam.</summary>
    private async Task CloseOthersAsync(OpenDocument keep)
    {
        foreach (OpenDocument document in _documents.Documents.Where(d => d != keep && !d.Pinned).ToList())
        {
            if (!await ConfirmSaveAsync(document))
                return;
            _documents.Close(document);
        }

        _documents.Activate(keep);
    }

    /// <summary>Fecha as abas comuns sem alteração pendente; fixadas e alteradas ficam.</summary>
    private void CloseSavedTabs()
    {
        foreach (OpenDocument document in _documents.Documents.Where(d => !d.Pinned && !d.IsDirty && !d.IsLoading).ToList())
            _documents.Close(document);
    }

    /// <summary>Ctrl+W: aba fixada não fecha pelo atalho, só avisa (o "Fechar" do menu fecha).</summary>
    private void CloseActive()
    {
        if (_documents.Active is not { } active)
            return;
        if (active.Pinned)
            _status.ShowMessage("Aba fixada — desafixe para fechar");
        else
            _ = RequestCloseAsync(active);
    }
}
