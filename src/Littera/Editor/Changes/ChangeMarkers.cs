using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Changes;

/// <summary>
/// Barras do protótipo "Alterações nas linhas": laranja = diferente do disco, verde = salva mas diferente de quando abriu.
/// Com o mouse em cima de um trecho laranja, a barra engrossa (marcador de destaque por baixo dela).
/// </summary>
internal static class ChangeMarkers
{
    public const int Hover = 14;
    public const int Unsaved = 15;
    public const int Saved = 16;
    public const uint Mask = (1u << Hover) | (1u << Unsaved) | (1u << Saved);

    public static void Setup(Scintilla editor)
    {
        Paint(editor.Markers[Unsaved], MarkerSymbol.Bar, Palette.ChangeUnsaved);
        Paint(editor.Markers[Saved], MarkerSymbol.Bar, Palette.ChangeSaved);
        Paint(editor.Markers[Hover], MarkerSymbol.FullRect, Palette.ChangeUnsaved);
    }

    /// <summary>Troca as barras pelas da comparação nova (null = sem marcação).</summary>
    public static void Apply(Scintilla editor, ChangeSet? changes)
    {
        editor.MarkerDeleteAll(Unsaved);
        editor.MarkerDeleteAll(Saved);
        editor.MarkerDeleteAll(Hover);
        if (changes is null)
            return;
        int count = editor.Lines.Count;
        foreach (int line in changes.UnsavedLines.Where(l => l < count))
            editor.Lines[line].MarkerAdd(Unsaved);
        foreach (int line in changes.SavedLines.Where(l => l < count))
            editor.Lines[line].MarkerAdd(Saved);
    }

    /// <summary>Barra grossa nas linhas do trecho sob o mouse; null tira o destaque.</summary>
    public static void SetHover(Scintilla editor, DiffHunk? hunk)
    {
        editor.MarkerDeleteAll(Hover);
        if (hunk is not { } h)
            return;
        int first = Math.Min(h.AfterStart, editor.Lines.Count - 1);
        for (int line = first; line < first + Math.Max(1, h.AfterCount) && line < editor.Lines.Count; line++)
            editor.Lines[line].MarkerAdd(Hover);
    }

    private static void Paint(Marker marker, MarkerSymbol symbol, Color color)
    {
        marker.Symbol = symbol;
        marker.SetBackColor(color);
        marker.SetForeColor(color);
    }
}
