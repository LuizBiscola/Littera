using Littera.Editor;
using Littera.Editor.Changes;
using Littera.Editor.Search;

namespace Littera.UI;

internal sealed partial class MainForm
{
    /// <summary>
    /// Ao terminar de carregar, o texto vira a referência das barras. Rascunho restaurado (alterações de antes
    /// de fechar o Littera) compara com o arquivo em disco, então essas alterações também aparecem em laranja.
    /// </summary>
    private async void CaptureBaseline(OpenDocument document)
    {
        if (document.Editor is not { } editor || editor.TextLength > ChangeBaseline.MaxChars)
            return;
        if (!document.HasUnsavedDraft)
        {
            document.Baseline = new ChangeBaseline(ChangeBaseline.Lines(editor.Text));
            return;
        }

        if (document.Path is not { } path || await Task.Run(() => FileSearch.ReadText(path)) is not { } disk)
            return; // rascunho sem título compara com o vazio
        document.Baseline = new ChangeBaseline(ChangeBaseline.Lines(disk));
        if (document == _documents.Active)
        {
            InvalidateChanges();
            ScheduleHighlights(textChanged: false);
        }
    }
}
