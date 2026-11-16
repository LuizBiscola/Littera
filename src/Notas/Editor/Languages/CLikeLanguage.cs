using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Languages;

/// <summary>Linguagens no lexer "cpp" da Lexilla (C#, JavaScript): só mudam as palavras-chave.</summary>
internal abstract class CLikeLanguage : Language
{
    public override string? Lexer => "cpp";

    public override bool LexerFolds => true;

    protected abstract string Keywords { get; }

    protected virtual string Types => "";

    protected override void Configure(Scintilla editor)
    {
        editor.SetKeywords(0, Keywords);
        editor.SetKeywords(1, Types);
        editor.SetProperty("lexer.cpp.track.preprocessor", "0"); // não acinzenta blocos #if
        editor.SetProperty("lexer.cpp.backquoted.strings", "1"); // `template` do JavaScript

        PaintItalic(editor, SyntaxColors.Comment, 1, 2, 3, 15, 17);
        Paint(editor, SyntaxColors.Number, 4);
        Paint(editor, SyntaxColors.Keyword, 5);
        Paint(editor, SyntaxColors.String, 6, 7, 12, 13, 14, 20, 21, 22);
        Paint(editor, SyntaxColors.Type, 9, 16, 19);              // pré-processador, tipos, classes globais
        Paint(editor, SyntaxColors.Function, 27);                 // sequências de escape
    }
}

internal sealed class CSharpLanguage : CLikeLanguage
{
    public override string Name => "C#";

    public override string[] Extensions => [".cs", ".csx"];

    protected override string Keywords =>
        "abstract add alias and as async await base bool break by byte case catch char checked class const continue " +
        "decimal default delegate descending do double dynamic else enum equals event explicit extern false file " +
        "finally fixed float for foreach from get global goto group if implicit in init int interface internal into " +
        "is join let lock long managed nameof namespace new nint not notnull nuint null object on operator or orderby " +
        "out override params partial private protected public readonly record ref remove required return sbyte " +
        "scoped sealed select set short sizeof stackalloc static string struct switch this throw true try typeof " +
        "uint ulong unchecked unmanaged unsafe ushort using value var virtual void volatile when where while with yield";
}
