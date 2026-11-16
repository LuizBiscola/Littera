using Notas.Editor.Folding;
using Notas.Settings;
using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>Fonte, cores, espaçamento e margens do editor; reaplicado quando as configurações mudam.</summary>
internal static class EditorStyle
{
    /// <summary>Depois disto a linguagem precisa reaplicar as cores dos tokens (StyleClearAll zera tudo).</summary>
    public static void Apply(Scintilla editor, EditorSettings settings)
    {
        string font = UiFonts.FirstInstalled(settings.FontFamily, "Cascadia Code", "Consolas");
        editor.StyleResetDefault();
        Style baseStyle = editor.Styles[Style.Default];
        baseStyle.Font = font;
        baseStyle.SizeF = settings.FontSize;
        baseStyle.ForeColor = Palette.Text;
        baseStyle.BackColor = Palette.Background;
        editor.StyleClearAll();

        Style numbers = editor.Styles[Style.LineNumber];
        numbers.SizeF = settings.FontSize * 12f / 13f; // números 12 px para código de 13 px, como no protótipo
        numbers.ForeColor = Palette.LineNumber;
        numbers.BackColor = Palette.Background;
        Style brace = editor.Styles[Style.BraceLight];
        brace.ForeColor = Palette.Accent;
        brace.BackColor = Palette.Selection;
        brace.Bold = true;
        editor.Styles[Style.BraceBad].ForeColor = Palette.WindowCloseHover; // vermelho: parêntese sem par
        editor.Styles[Style.IndentGuide].ForeColor = Palette.IndentGuide;
        editor.Styles[Style.IndentGuide].BackColor = Palette.Background;
        SummaryStyle(editor.Styles[EditorFolding.SummaryStyle], Palette.TextMuted, Palette.FoldSummary, settings);
        SummaryStyle(editor.Styles[EditorFolding.SummaryActiveStyle], Palette.Accent, Palette.FoldSummaryActive, settings);

        ApplyLineSpacing(editor, settings);
        editor.CaretForeColor = Palette.Text;
        editor.CaretWidth = 2;
        editor.CaretLineBackColor = settings.HighlightCurrentLine ? Palette.AccentSoft : Color.Transparent;
        editor.SelectionBackColor = Palette.Selection;
        editor.SelectionAdditionalBackColor = Palette.Selection;
        editor.AdditionalCaretForeColor = Palette.Accent;
    }

    /// <summary>Resumo de trecho recolhido: fonte da interface, um pouco menor que o código, sobre fundo claro.</summary>
    private static void SummaryStyle(Style style, Color fore, Color back, EditorSettings settings)
    {
        style.Font = UiFonts.Family;
        style.SizeF = settings.FontSize * 11.5f / 13f;
        style.ForeColor = fore;
        style.BackColor = back;
    }

    /// <summary>Espaçamento como múltiplo da fonte: a sobra vira espaço acima e abaixo do texto.</summary>
    private static void ApplyLineSpacing(Scintilla editor, EditorSettings settings)
    {
        editor.ExtraAscent = 0;
        editor.ExtraDescent = 0;
        float fontPx = settings.FontSize * editor.DeviceDpi / 72f;
        int extra = Math.Max(0, (int)Math.Round(fontPx * settings.LineSpacing) - editor.Lines[0].Height);
        editor.ExtraAscent = extra / 2;
        editor.ExtraDescent = extra - extra / 2;
    }
}
