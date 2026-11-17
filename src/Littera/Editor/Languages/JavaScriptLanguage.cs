namespace Notas.Editor.Languages;

/// <summary>JavaScript e TypeScript no mesmo lexer "cpp" (template strings ligadas na base).</summary>
internal sealed class JavaScriptLanguage : CLikeLanguage
{
    public override string Name => "JavaScript";

    public override string[] Extensions => [".js", ".mjs", ".cjs", ".jsx", ".ts", ".tsx"];

    protected override string Keywords =>
        "abstract any as async await boolean break case catch class const continue debugger declare default delete " +
        "do else enum export extends false finally for from function get if implements import in instanceof " +
        "interface keyof let namespace never new null number of private protected public readonly return set " +
        "static string super switch symbol this throw true try type typeof undefined unknown var void while with yield";

    protected override string Types =>
        "Array Boolean Date Error JSON Map Math Number Object Promise RegExp Set String Symbol console document window";
}
