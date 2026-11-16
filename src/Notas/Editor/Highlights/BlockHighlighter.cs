using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Highlights;

/// <summary>
/// "Destacar bloco atual" do protótipo: fundo azul bem claro nas linhas do bloco, faixa na calha
/// e faixa forte na linha do cursor.
/// </summary>
internal static class BlockHighlighter
{
    // 14 a 16 são das barras de alteração; 25 a 31, da dobra.
    private const int Background = 18;
    private const int Bar = 19;
    private const int CurrentBar = 20;
    private const int MaxMarkedLines = 5000; // bloco gigante: só a faixa da linha atual

    public static void Setup(Scintilla editor)
    {
        editor.Markers[Background].Symbol = MarkerSymbol.Background;
        editor.Markers[Background].SetBackColor(Palette.StatementBackground);
        editor.Markers[Bar].Symbol = MarkerSymbol.FullRect;
        editor.Markers[Bar].SetBackColor(Palette.StatementMarker);
        editor.Markers[CurrentBar].Symbol = MarkerSymbol.FullRect;
        editor.Markers[CurrentBar].SetBackColor(Palette.Accent);
        editor.Margins[EditorMargins.BlockBar].Mask = (1u << Bar) | (1u << CurrentBar);
    }

    /// <summary>Marca o bloco do cursor e devolve o texto da barra de status ("Instrução 2 de 3 · linhas 6–26").</summary>
    public static string? Update(Scintilla editor, List<LineBlock> blocks, string kind)
    {
        Clear(editor);
        int caretLine = editor.CurrentLine;
        editor.Lines[caretLine].MarkerAdd(CurrentBar);

        int index = blocks.FindIndex(b => b.Contains(caretLine));
        if (index < 0)
            return null;

        LineBlock block = blocks[index];
        if (block.LastLine - block.FirstLine < MaxMarkedLines)
        {
            for (int line = block.FirstLine; line <= block.LastLine; line++)
            {
                editor.Lines[line].MarkerAdd(Background);
                editor.Lines[line].MarkerAdd(Bar);
            }
        }

        string lines = block.FirstLine == block.LastLine ? $"linha {block.FirstLine + 1}" : $"linhas {block.FirstLine + 1}–{block.LastLine + 1}";
        return $"{kind} {index + 1} de {blocks.Count} · {lines}";
    }

    public static void Clear(Scintilla editor)
    {
        foreach (int marker in new[] { Background, Bar, CurrentBar })
            editor.MarkerDeleteAll(marker);
    }
}
