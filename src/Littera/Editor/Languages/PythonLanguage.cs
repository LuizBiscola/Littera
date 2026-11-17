using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Languages;

internal sealed class PythonLanguage : Language
{
    public override string Name => "Python";

    public override string? Lexer => "python";

    public override string? LineComment => "#";

    public override bool LexerFolds => true;

    public override string[] Extensions => [".py", ".pyw", ".pyi"];

    protected override void Configure(Scintilla editor)
    {
        editor.SetKeywords(0,
            "False None True and as assert async await break case class continue def del elif else except finally " +
            "for from global if import in is lambda match nonlocal not or pass raise return try while with yield");
        editor.SetKeywords(1,
            "abs all any bool bytes dict enumerate filter float format input int isinstance iter len list map max " +
            "min next object open print range repr reversed round self set sorted str sum super tuple type zip");

        PaintItalic(editor, SyntaxColors.Comment, 1, 12);
        Paint(editor, SyntaxColors.Number, 2);
        Paint(editor, SyntaxColors.String, 3, 4, 6, 7, 13, 16, 17, 18, 19);
        Paint(editor, SyntaxColors.Keyword, 5);
        Paint(editor, SyntaxColors.Type, 8, 14);      // nome de classe e funções embutidas
        Paint(editor, SyntaxColors.Function, 9);      // nome na definição def
        Paint(editor, SyntaxColors.Variable, 15);     // @decorador
    }
}
