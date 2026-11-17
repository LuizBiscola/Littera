namespace Littera.Editor.Languages;

/// <summary>Linguagens disponíveis e a escolha automática pela extensão do arquivo.</summary>
internal static class LanguageRegistry
{
    public static Language PlainText { get; } = new PlainTextLanguage();

    public static IReadOnlyList<Language> All { get; } =
    [
        new SqlLanguage(),
        new JsonLanguage(),
        new XmlLanguage(),
        new CSharpLanguage(),
        new JavaScriptLanguage(),
        new PythonLanguage(),
        new MarkdownLanguage(),
        PlainText,
    ];

    public static Language ForPath(string? path)
    {
        string extension = Path.GetExtension(path ?? "").ToLowerInvariant();
        return All.FirstOrDefault(l => l.Extensions.Contains(extension)) ?? PlainText;
    }

    public static Language? ByName(string? name) => All.FirstOrDefault(l => l.Name == name);
}
