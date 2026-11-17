using Littera.Editor;
using Littera.Editor.Folding;
using ScintillaNET;

namespace Littera.UI;

internal sealed partial class MainForm
{
    /// <summary>Para a sessão: os recolhidos agora, ou os da sessão anterior que ainda não puderam ser aplicados.</summary>
    private static List<int>? FoldedHeaders(OpenDocument document, Scintilla editor)
    {
        List<int> folded = document.PendingFolds?.Headers.ToList() ?? EditorFolding.Contracted(editor);
        return folded.Count > 0 ? folded : null;
    }

    /// <summary>Recolhe de novo o que estava recolhido ao fechar; cabeçalho que não existe mais (arquivo mudou lá fora) é ignorado.</summary>
    private void RestoreFolds(Scintilla editor, OpenDocument document)
    {
        if (document.PendingFolds is not { } pending)
            return;
        document.PendingFolds = null;
        foreach (int header in pending.Headers)
        {
            if (EditorFolding.IsHeader(editor, header) && editor.Lines[header].Expanded)
                EditorFolding.Collapse(editor, header);
        }

        editor.FirstVisibleLine = pending.FirstVisibleLine; // conta linhas na tela: só vale depois de recolher
        RefreshFoldSummaries(editor);
    }
}
