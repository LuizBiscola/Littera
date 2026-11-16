using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Changes;

/// <summary>
/// Barras do protótipo "Alterações nas linhas", a partir do histórico de alterações do próprio Scintilla:
/// laranja = diferente do arquivo em disco (alterada e não salva), verde = alterada e já salva.
/// </summary>
internal static class ChangeMarkers
{
    public const uint UnsavedMask = (1u << Marker.HistoryModified) | (1u << Marker.HistoryRevertedToOrigin)
        | (1u << Marker.HistoryRevertedToModified);

    public const uint AllMask = UnsavedMask | (1u << Marker.HistorySaved);

    private const int SciMarkerGet = 2046;

    public static void Setup(Scintilla editor)
    {
        foreach (int marker in new[] { Marker.HistoryModified, Marker.HistoryRevertedToOrigin, Marker.HistoryRevertedToModified })
            Paint(editor.Markers[marker], Palette.ChangeUnsaved);
        Paint(editor.Markers[Marker.HistorySaved], Palette.ChangeSaved);
    }

    /// <summary>Só dá para ligar com a pilha de desfazer vazia: arquivo recém-carregado ou novo.</summary>
    public static void Enable(Scintilla editor) => editor.ChangeHistory = ChangeHistory.Enabled | ChangeHistory.Markers;

    public static bool IsUnsaved(Scintilla editor, int line) => (Markers(editor, line) & UnsavedMask) != 0;

    /// <summary>
    /// Linhas alteradas em ordem (Unsaved = diferente do disco). Linha a linha: a busca de marcadores
    /// do Scintilla (MARKERNEXT) não enxerga os do histórico, mas a leitura por linha enxerga.
    /// </summary>
    public static List<(int Line, bool Unsaved)> Lines(Scintilla editor)
    {
        var lines = new List<(int Line, bool Unsaved)>();
        int count = editor.Lines.Count;
        for (int line = 0; line < count; line++)
        {
            uint markers = Markers(editor, line);
            if ((markers & AllMask) != 0)
                lines.Add((line, (markers & UnsavedMask) != 0));
        }

        return lines;
    }

    private static uint Markers(Scintilla editor, int line) => (uint)(long)editor.DirectMessage(SciMarkerGet, line);

    private static void Paint(Marker marker, Color color)
    {
        marker.Symbol = MarkerSymbol.Bar;
        marker.SetBackColor(color);
        marker.SetForeColor(color);
    }
}
