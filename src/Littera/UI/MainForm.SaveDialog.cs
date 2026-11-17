using Littera.Editor;

namespace Littera.UI;

internal sealed partial class MainForm
{
    /// <summary>
    /// "Salvar como": arquivo novo sai como "Sem título 1.txt", e um nome digitado sem extensão ganha .txt.
    /// Se a linguagem da aba foi escolhida (ex.: SQL), vale a extensão dela, para o realce voltar ao reabrir.
    /// </summary>
    private static SaveFileDialog SaveDialogFor(OpenDocument document)
    {
        string extension = document.Path is { } path ? Path.GetExtension(path).TrimStart('.') : DefaultExtension(document);
        var dialog = new SaveFileDialog
        {
            Filter = FileFilter,
            Title = "Salvar como",
            AddExtension = true,
            DefaultExt = extension,
            FilterIndex = extension.ToLowerInvariant() switch
            {
                "sql" => 2,
                "txt" or "md" or "log" => 3,
                "json" or "xml" => 4,
                _ => 1,
            },
            FileName = document.Path is null ? $"{document.Title}.{extension}" : document.Title,
        };
        if (document.Path is not null)
            dialog.InitialDirectory = Path.GetDirectoryName(document.Path);
        return dialog;
    }

    private static string DefaultExtension(OpenDocument document) =>
        document.LanguageChosen && document.Language.Extensions.FirstOrDefault() is { } extension ? extension.TrimStart('.') : "txt";
}
