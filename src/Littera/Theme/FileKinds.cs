namespace Notas.Theme;

/// <summary>Ícone pela extensão, sem diferenciar maiúsculas; fora da tabela, o genérico. A linguagem escolhida não muda o ícone.</summary>
internal static class FileKinds
{
    private static readonly Dictionary<string, LineIcon> ByExtension = Build(
        (FileIcons.Text, ".txt .text"),
        (FileIcons.Database, ".sql .ddl .pgsql .psql .plsql"),
        (FileIcons.Code, ".js .ts .cs .py .java .php .go .html .css .sh .ps1 .bat"),
        (FileIcons.Braces, ".json .jsonc"),
        (FileIcons.Markup, ".xml .yaml .yml .toml"),
        (FileIcons.Sheet, ".csv .tsv"),
        (FileIcons.Settings, ".ini .cfg .conf .config .env .properties"),
        (FileIcons.Document, ".md .markdown .rst"),
        (FileIcons.Log, ".log .out .trace"));

    /// <summary>Sem extensão conta como texto simples.</summary>
    public static LineIcon For(string path)
    {
        string extension = Path.GetExtension(path);
        return extension.Length == 0 ? FileIcons.Text : ByExtension.GetValueOrDefault(extension, FileIcons.File);
    }

    private static Dictionary<string, LineIcon> Build(params (LineIcon Icon, string Extensions)[] kinds)
    {
        var map = new Dictionary<string, LineIcon>(StringComparer.OrdinalIgnoreCase);
        foreach ((LineIcon icon, string extensions) in kinds)
            foreach (string extension in extensions.Split(' '))
                map[extension] = icon;
        return map;
    }
}
