using System.Runtime.CompilerServices;
using Notas.Editor.Highlights;
using ScintillaNET;

namespace Notas.Editor.Folding;

internal static partial class EditorFolding
{
    public const int SummaryStyle = 250;
    public const int SummaryActiveStyle = 251;

    private static readonly ConditionalWeakTable<Scintilla, List<(int Line, int Style)>> Shown = new();

    /// <summary>Recolhe; o cursor que ficaria escondido sobe para o fim do cabeçalho.</summary>
    public static void Collapse(Scintilla editor, int header)
    {
        int last = LastChild(editor, header);
        editor.Lines[header].FoldLine(FoldAction.Contract);
        int caretLine = editor.CurrentLine;
        if (caretLine > header && caretLine <= last)
            editor.GotoPosition(editor.Lines[header].EndPosition - EolLength(editor, header));
    }

    public static void Expand(Scintilla editor, int header) => editor.Lines[header].FoldLine(FoldAction.Expand);

    /// <summary>Resumo no fim de cada cabeçalho recolhido (azul no bloco do cursor). Devolve quantos estão recolhidos.</summary>
    public static int Refresh(Scintilla editor, bool sql, LineBlock? current, bool force)
    {
        List<int> headers = Contracted(editor);
        var wanted = headers.Select(h => (h, current?.Contains(h) == true ? SummaryActiveStyle : SummaryStyle)).ToList();
        List<(int Line, int Style)> shown = Shown.GetOrCreateValue(editor);
        if (!force && shown.SequenceEqual(wanted))
            return wanted.Count;

        EolAnnotations.ClearAll(editor);
        foreach ((int line, int style) in wanted)
            EolAnnotations.Set(editor, line, $" {FoldSummary.For(HiddenLines(editor, line), sql)} ", style);
        shown.Clear();
        shown.AddRange(wanted);
        return wanted.Count;
    }

    private static int EolLength(Scintilla editor, int line)
    {
        string text = editor.Lines[line].Text;
        return text.EndsWith("\r\n", StringComparison.Ordinal) ? 2 : text.EndsWith('\n') || text.EndsWith('\r') ? 1 : 0;
    }
}
