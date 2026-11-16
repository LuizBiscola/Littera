namespace Notas.Settings;

/// <summary>Conteúdo de %APPDATA%\Notas\settings.json.</summary>
internal sealed class AppSettings
{
    public EditorSettings Editor { get; set; } = new();

    public WindowSettings Window { get; set; } = new();

    public SqlFormatSettings SqlFormat { get; set; } = new();

    public FilesSettings Files { get; set; } = new();

    /// <summary>Atalhos por comando ("formatarSql": "Ctrl+Shift+F"); vários separados por ";". Faltando, vale o padrão.</summary>
    public Dictionary<string, string> Shortcuts { get; set; } = [];

    public List<string> RecentFiles { get; set; } = [];
}

internal sealed class EditorSettings
{
    public string FontFamily { get; set; } = "Cascadia Code";

    /// <summary>Tamanho em pontos (9,75 pt = 13 px a 100%, como no protótipo).</summary>
    public float FontSize { get; set; } = 9.75f;

    /// <summary>Altura da linha como múltiplo do tamanho da fonte (1,7 ≈ 22 px a 13 px).</summary>
    public float LineSpacing { get; set; } = 1.7f;

    public int IndentSize { get; set; } = 4;

    public bool UseTabs { get; set; }

    /// <summary>Linhas verticais em cada nível de recuo.</summary>
    public bool ShowIndentGuides { get; set; } = true;

    public bool WordWrap { get; set; }

    public bool ShowLineNumbers { get; set; } = true;

    public bool HighlightCurrentLine { get; set; } = true;

    public bool HighlightCurrentBlock { get; set; } = true;

    public bool HighlightOccurrences { get; set; } = true;

    /// <summary>Barras laranja (não salva) e verde (salva) nas linhas alteradas.</summary>
    public bool ShowChanges { get; set; } = true;

    /// <summary>"gdi" (padrão, mais leve) ou "directwrite" (ligaduras, +40 MB).</summary>
    public string Renderer { get; set; } = "gdi";
}

internal sealed class WindowSettings
{
    // Campos soltos em vez de Rectangle: o JSON fica limpo e não depende de Location/Size.
    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public bool Maximized { get; set; }

    public bool ShowOpenFiles { get; set; } = true;
}
