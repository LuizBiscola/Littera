namespace Notas.Editor;

internal sealed partial class DocumentManager
{
    public OpenDocument New()
    {
        int number = _documents.Where(d => d.Path is null).Select(d => d.UntitledNumber).DefaultIfEmpty(0).Max() + 1;
        var document = new OpenDocument(null, number) { IsLoaded = true };
        _documents.Add(document);
        Activate(document);
        return document;
    }

    /// <summary>Já aberto: só ativa. Senão cria a aba e carrega em blocos.</summary>
    public OpenDocument Open(string path)
    {
        OpenDocument document = Find(path) ?? AddUnloaded(path);
        Activate(document);
        return document;
    }

    /// <summary>Entra na lista sem carregar (sessão restaurada: lê o arquivo só quando a aba for vista).</summary>
    public OpenDocument AddUnloaded(string path)
    {
        var document = new OpenDocument(Path.GetFullPath(path));
        _documents.Add(document);
        Changed?.Invoke();
        return document;
    }

    /// <summary>Fecha sem perguntar (quem chama já tratou "salvar alterações?"). Nunca fica sem aba.</summary>
    public void Close(OpenDocument document)
    {
        int index = _documents.IndexOf(document);
        _documents.RemoveAt(index);
        document.LoadCancellation?.Cancel();

        if (document == Active)
        {
            Active = null;
            if (_documents.Count == 0)
                New();
            else
                Activate(_documents[Math.Min(index, _documents.Count - 1)]);
        }

        document.Editor?.Dispose();
        Changed?.Invoke();
    }

    private OpenDocument? Find(string path) =>
        _documents.Find(d => string.Equals(d.Path, Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase));
}
