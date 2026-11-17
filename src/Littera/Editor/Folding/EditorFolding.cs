using Littera.Theme;
using ScintillaNET;

namespace Littera.Editor.Folding;

/// <summary>Recolher e expandir trechos: níveis de dobra, linhas escondidas e cabeçalhos recolhidos.</summary>
internal static partial class EditorFolding
{
    private const int SciSetFoldLevel = 2222;
    private const int SciGetFoldLevel = 2223;
    private const int SciGetLastChild = 2224;

    public static void Setup(Scintilla editor)
    {
        // Se um trecho recolhido deixa de existir (texto editado), as linhas dele voltam a aparecer.
        editor.AutomaticFold = AutomaticFold.Show | AutomaticFold.Change;
        editor.SetFoldMarginColor(true, Palette.Background);
        editor.SetFoldMarginHighlightColor(true, Palette.Background);
        EolAnnotations.Show(editor, EOLAnnotationVisible.Stadium);
    }

    /// <summary>Níveis calculados pelo Littera (SQL e texto); só escreve as linhas que mudaram.</summary>
    public static void ApplyLevels(Scintilla editor, int[] levels)
    {
        int count = Math.Min(levels.Length, editor.Lines.Count);
        for (int line = 0; line < count; line++)
        {
            if ((int)editor.DirectMessage(SciGetFoldLevel, line) != levels[line])
                editor.DirectMessage(SciSetFoldLevel, line, levels[line]);
        }
    }

    public static bool IsHeader(Scintilla editor, int line) =>
        line >= 0 && line < editor.Lines.Count && ((int)editor.DirectMessage(SciGetFoldLevel, line) & FoldLevels.HeaderFlag) != 0;

    /// <summary>Última linha que some quando o cabeçalho é recolhido.</summary>
    public static int LastChild(Scintilla editor, int header) => (int)editor.DirectMessage(SciGetLastChild, header, -1);

    /// <summary>Cabeçalhos recolhidos, de cima para baixo.</summary>
    public static List<int> Contracted(Scintilla editor)
    {
        var headers = new List<int>();
        int count = editor.Lines.Count;
        for (int line = count > 0 ? editor.Lines[0].ContractedFoldNext : -1; line >= 0;
             line = line + 1 < count ? editor.Lines[line + 1].ContractedFoldNext : -1)
            headers.Add(line);
        return headers;
    }

    /// <summary>Texto das linhas que o cabeçalho esconde.</summary>
    public static string[] HiddenLines(Scintilla editor, int header)
    {
        int last = LastChild(editor, header);
        if (last <= header)
            return [];
        int start = editor.Lines[header + 1].Position, end = editor.Lines[last].EndPosition;
        return editor.GetTextRange(start, end - start).Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd('\n').Split('\n');
    }
}
