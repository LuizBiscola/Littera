using Littera.Editor.Languages;

namespace Littera.Editor;

internal sealed partial class DocumentManager
{
    /// <summary>Escolha manual no seletor da barra de status.</summary>
    public void SetLanguage(OpenDocument document, Language language)
    {
        document.LanguageChosen = true;
        document.Language = language;
        if (document.Editor is not null)
            language.Apply(document.Editor);
        Changed?.Invoke();
    }

    /// <summary>Pela extensão, se o usuário não escolheu outra. Aplicada antes da carga: o texto já chega colorido.</summary>
    private static void DetectLanguage(OpenDocument document)
    {
        if (!document.LanguageChosen)
            document.Language = LanguageRegistry.ForPath(document.Path);
        if (document.Editor is not null)
            document.Language.Apply(document.Editor);
    }
}
