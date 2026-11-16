using Notas.Editor.Changes;
using Notas.Editor.Folding;
using Notas.Settings;
using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>
/// Calha do protótipo, da esquerda para a direita: barra de alterações, números alinhados à direita,
/// setas de dobra, faixa de 3 px do bloco atual e 16 px antes do texto.
/// </summary>
internal static class EditorMargins
{
    public const int Changes = 0;
    public const int Numbers = 1;
    public const int Fold = 2;
    public const int BlockBar = 3;

    public static void Apply(Scintilla editor, EditorSettings settings)
    {
        Margin changes = editor.Margins[Changes];
        changes.Type = MarginType.Symbol;
        changes.Mask = ChangeMarkers.AllMask;
        changes.Sensitive = true; // clique na barra abre a prévia da alteração
        changes.Cursor = MarginCursor.Arrow;
        changes.Width = settings.ShowChanges ? Px(editor, 8) : 0;

        Margin fold = editor.Margins[Fold];
        fold.Type = MarginType.Symbol;
        fold.Mask = Marker.MaskFolders;
        fold.Sensitive = true;
        fold.Cursor = MarginCursor.Arrow;
        fold.Width = Px(editor, 20);
        FoldMarkers.Define(editor);

        Margin bar = editor.Margins[BlockBar];
        bar.Type = MarginType.Symbol;
        bar.Width = Px(editor, 3);

        editor.Margins[Numbers].Type = MarginType.Number;
        editor.Margins[Numbers].Mask = 0;
        editor.Margins.Left = Px(editor, 16);
        UpdateNumberWidth(editor, settings.ShowLineNumbers);
    }

    /// <summary>Largura dos números conforme a quantidade de dígitos (mínimo 3); chamado quando o total de linhas muda.</summary>
    public static void UpdateNumberWidth(Scintilla editor, bool visible)
    {
        int digits = Math.Max(3, editor.Lines.Count.ToString().Length);
        int width = visible ? editor.TextWidth(Style.LineNumber, new string('9', digits)) + Px(editor, 18) : 0;
        if (editor.Margins[Numbers].Width != width)
            editor.Margins[Numbers].Width = width;
    }

    private static int Px(Scintilla editor, int px) => (int)Math.Round(px * editor.DeviceDpi / 96.0);
}
