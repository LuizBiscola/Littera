using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Languages;

internal sealed class JsonLanguage : Language
{
    public override string Name => "JSON";

    public override string? Lexer => "json";

    public override string[] Extensions => [".json", ".jsonc", ".geojson", ".har"];

    protected override void Configure(Scintilla editor)
    {
        editor.SetKeywords(0, "true false null");
        editor.SetProperty("lexer.json.allow.comments", "1");

        Paint(editor, SyntaxColors.Number, 1);
        Paint(editor, SyntaxColors.String, 2, 3);
        Paint(editor, SyntaxColors.Keyword, 4);         // nome da propriedade
        Paint(editor, SyntaxColors.Function, 5);        // sequência de escape
        PaintItalic(editor, SyntaxColors.Comment, 6, 7);
        Paint(editor, SyntaxColors.Variable, 11);       // true, false, null
    }
}
