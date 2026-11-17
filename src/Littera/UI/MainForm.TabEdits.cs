using Notas.Editor;
using Notas.Services;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Cópia do texto atual numa aba "Sem título", com a mesma linguagem.</summary>
    private void DuplicateTab(OpenDocument source)
    {
        string text = source.Editor is { } editor && source.IsLoaded ? editor.Text
            : (source.SourcePath ?? source.Path) is { } path && File.Exists(path) ? File.ReadAllText(path) : "";
        OpenDocument copy = _documents.New();
        copy.Editor!.Text = text;
        _documents.SetLanguage(copy, source.Language);
    }

    /// <summary>Volta ao texto salvo (pergunta antes; dá para desfazer com Ctrl+Z).</summary>
    private void DiscardChanges(OpenDocument document)
    {
        if (!document.IsDirty || MessageBox.Show(this, $"Descartar as alterações em \"{document.Title}\"? O texto volta a ser o do arquivo salvo.",
                Product.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;

        DiscardDraft(document);
        document.SourcePath = null; // aba ainda não carregada passa a ler o arquivo, não o rascunho
        if (document.Editor is { } editor && document.IsLoaded)
        {
            string saved = document.Path is { } path && File.Exists(path) ? File.ReadAllText(path, document.Format.Encoding.ToEncoding()) : "";
            editor.Text = saved;
            editor.SetSavePoint();
        }

        _openFiles.Invalidate();
    }
}
