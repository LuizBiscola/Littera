namespace Littera.Editor;

internal sealed partial class DocumentManager
{
    /// <summary>Renomeia no disco e na aba; a linguagem acompanha a nova extensão, se não foi escolhida à mão.</summary>
    public void Rename(OpenDocument document, string target)
    {
        File.Move(document.Path!, target);
        document.Path = Path.GetFullPath(target);
        document.Missing = false;
        DetectLanguage(document);
        Changed?.Invoke();
    }
}
