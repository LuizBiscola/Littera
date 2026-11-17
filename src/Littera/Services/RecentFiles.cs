namespace Littera.Services;

/// <summary>Lista de arquivos recentes (guardada no settings.json): mais novo primeiro, sem repetir.</summary>
internal static class RecentFiles
{
    public const int Max = 12;

    public static void Add(List<string> recent, string path)
    {
        recent.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
        recent.Insert(0, path);
        if (recent.Count > Max)
            recent.RemoveRange(Max, recent.Count - Max);
    }
}
