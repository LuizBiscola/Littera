namespace Notas.Editor;

internal sealed partial class DocumentManager
{
    /// <summary>Encoding e quebra de linha de arquivos novos (configurações → Arquivos).</summary>
    public Services.TextFormat NewFileFormat { get; set; } = Services.TextFormat.Default;

    public OpenDocument New()
    {
        int number = _documents.Where(d => d.Path is null).Select(d => d.UntitledNumber).DefaultIfEmpty(0).Max() + 1;
        var document = new OpenDocument(null, number) { IsLoaded = true, Format = NewFileFormat };
        _documents.Add(document);
        Activate(document);
        document.Editor!.EolMode = NewFileFormat.LineEnding.ToScintilla();
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

    /// <summary>Entra na lista já montado (aba restaurada de rascunho), sem carregar.</summary>
    public void AddUnloaded(OpenDocument document)
    {
        _documents.Add(document);
        Changed?.Invoke();
    }

    /// <summary>Fecha sem perguntar (quem chama já tratou "salvar alterações?"). Nunca fica sem aba.</summary>
    public void Close(OpenDocument document)
    {
        int index = _documents.IndexOf(document);
        _documents.RemoveAt(index);
        document.LoadCancellation?.Cancel();
        Services.DraftStore.Delete(document.DraftId); // fechou por vontade própria: o rascunho não serve mais

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
