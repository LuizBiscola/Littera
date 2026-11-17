namespace Notas.Theme;

/// <summary>
/// Ícones de arquivo e pasta do protótipo ("Especificação — Ícones de arquivo"): caminhos copiados do Lucide
/// (lucide-static 0.544.0, licença ISC; aviso em THIRD-PARTY-NOTICES.txt). Nome do Lucide ao lado de cada um.
/// </summary>
internal static class FileIcons
{
    private const string Page = "M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z";
    private const string Corner = "M14 2v4a2 2 0 0 0 2 2h4";

    public static readonly LineIcon File = new(Page, Corner); // file
    public static readonly LineIcon Text = new(Page, Corner, "M10 9H8", "M16 13H8", "M16 17H8"); // file-text
    public static readonly LineIcon Code = new(Page, Corner, "M10 12.5 8 15l2 2.5", "m14 12.5 2 2.5-2 2.5"); // file-code
    public static readonly LineIcon Database = new( // database
        "M3 5a9 3 0 1 0 18 0a9 3 0 1 0-18 0", "M3 5V19A9 3 0 0 0 21 19V5", "M3 12A9 3 0 0 0 21 12");
    public static readonly LineIcon Braces = new( // braces
        "M8 3H7a2 2 0 0 0-2 2v5a2 2 0 0 1-2 2 2 2 0 0 1 2 2v5c0 1.1.9 2 2 2h1",
        "M16 21h1a2 2 0 0 0 2-2v-5c0-1.1.9-2 2-2a2 2 0 0 1-2-2V5a2 2 0 0 0-2-2h-1");
    public static readonly LineIcon Markup = new("m18 16 4-4-4-4", "m6 8-4 4 4 4", "m14.5 4-5 16"); // code-xml
    public static readonly LineIcon Sheet = new( // sheet
        "M5 3h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Z", "M3 9h18", "M3 15h18", "M9 9v12", "M15 9v12");
    public static readonly LineIcon Settings = new( // file-cog
        Corner, "M4.677 21.5a2 2 0 0 0 1.313.5H18a2 2 0 0 0 2-2V7l-5-5H6a2 2 0 0 0-2 2v2.5", "M3 14a3 3 0 1 0 6 0a3 3 0 1 0-6 0",
        "m2.305 15.53.923-.382", "m3.228 12.852-.924-.383", "m4.852 11.228-.383-.923", "m4.852 16.772-.383.924",
        "m7.148 11.228.383-.923", "m7.53 17.696-.382-.924", "m8.772 12.852.923-.383", "m8.772 15.148.923.383");
    public static readonly LineIcon Document = new(Page, Corner, "M9 13v-1h6v1", "M12 12v6", "M11 18h2"); // file-type
    public static readonly LineIcon Log = new("M15 12h-5", "M15 8h-5", "M19 17V5a2 2 0 0 0-2-2H4", // scroll-text
        "M8 21h12a2 2 0 0 0 2-2v-1a1 1 0 0 0-1-1H11a1 1 0 0 0-1 1v1a2 2 0 1 1-4 0V5a2 2 0 1 0-4 0v2a1 1 0 0 0 1 1h3");

    public static readonly LineIcon Untitled = new( // file-pen-line
        "m18 5-2.414-2.414A2 2 0 0 0 14.172 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2",
        "M21.378 12.626a1 1 0 0 0-3.004-3.004l-4.01 4.012a2 2 0 0 0-.506.854l-.837 2.87a.5.5 0 0 0 .62.62l2.87-.837a2 2 0 0 0 .854-.506z",
        "M8 18h1");
    public static readonly LineIcon Missing = new(Page, Corner, "m14.5 12.5-5 5", "m9.5 12.5 5 5"); // file-x

    public static readonly LineIcon Folder = new( // folder
        "M20 20a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2h-7.9a2 2 0 0 1-1.69-.9L9.6 3.9A2 2 0 0 0 7.93 3H4a2 2 0 0 0-2 2v13a2 2 0 0 0 2 2Z");
    public static readonly LineIcon FolderOpen = new( // folder-open
        "m6 14 1.5-2.9A2 2 0 0 1 9.24 10H20a2 2 0 0 1 1.94 2.5l-1.54 6a2 2 0 0 1-1.95 1.5H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h3.9a2 2 0 0 1 1.69.9l.81 1.2a2 2 0 0 0 1.67.9H18a2 2 0 0 1 2 2v2");
}
