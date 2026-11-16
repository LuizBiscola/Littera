using Notas.Settings;
using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>
/// Calha do protótipo: números alinhados à direita, 14 px de respiro, faixa de 3 px do bloco atual
/// e 16 px antes do texto.
/// </summary>
internal static class EditorMargins
{
    public const int Numbers = 0;
    public const int Spacer = 1;
    public const int BlockBar = 2;

    public static void Apply(Scintilla editor, EditorSettings settings)
    {
        Margin spacer = editor.Margins[Spacer];
        spacer.Type = MarginType.Color;
        spacer.BackColor = Palette.Background;
        spacer.Mask = 0;
        spacer.Width = Px(editor, settings.ShowLineNumbers ? 14 : 8);

        Margin bar = editor.Margins[BlockBar];
        bar.Type = MarginType.Symbol;
        bar.Width = Px(editor, 3);

        editor.Margins[Numbers].Type = MarginType.Number;
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
