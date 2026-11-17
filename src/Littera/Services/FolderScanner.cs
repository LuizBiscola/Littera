namespace Littera.Services;

/// <summary>Item da árvore do painel "PASTA".</summary>
internal sealed record FolderEntry(string Path, string Name, bool IsDirectory);

/// <summary>Lê pastas para o painel: pastas antes de arquivos, ordem alfabética, sem lixo de build e controle de versão.</summary>
internal static class FolderScanner
{
    private static readonly HashSet<string> Ignored = new(StringComparer.OrdinalIgnoreCase)
    {
        ".git", ".vs", ".idea", ".vscode", "bin", "obj", "node_modules", "packages", "artifacts", "__pycache__",
    };

    public static List<FolderEntry> Children(string folder)
    {
        var entries = new List<FolderEntry>();
        try
        {
            foreach (var info in new DirectoryInfo(folder).EnumerateFileSystemInfos())
            {
                bool directory = info is DirectoryInfo;
                if (Ignored.Contains(info.Name) || info.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;
                entries.Add(new FolderEntry(info.FullName, info.Name, directory));
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Pasta sumiu ou sem permissão: mostra vazia.
        }

        return [.. entries.OrderByDescending(e => e.IsDirectory).ThenBy(e => e.Name, StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>Arquivos de toda a árvore cujo nome contém o filtro (para no limite, pastas enormes não travam).</summary>
    public static List<FolderEntry> Search(string folder, string filter, CancellationToken cancel = default, int limit = 500)
    {
        var found = new List<FolderEntry>();
        var pending = new Stack<string>([folder]);
        while (pending.Count > 0 && found.Count < limit && !cancel.IsCancellationRequested)
        {
            foreach (FolderEntry entry in Children(pending.Pop()))
            {
                if (entry.IsDirectory)
                    pending.Push(entry.Path);
                else if (entry.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) && found.Count < limit)
                    found.Add(entry);
            }
        }

        return [.. found.OrderBy(e => e.Path, StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>Arquivos e subpastas da árvore toda para o rodapé; Partial quando parou no limite.</summary>
    public static (int Files, int Folders, bool Partial) Count(string folder, CancellationToken cancel = default, int limit = 20_000)
    {
        int files = 0, folders = 0;
        var pending = new Stack<string>([folder]);
        while (pending.Count > 0 && files + folders < limit && !cancel.IsCancellationRequested)
        {
            foreach (FolderEntry entry in Children(pending.Pop()))
            {
                if (entry.IsDirectory)
                {
                    folders++;
                    pending.Push(entry.Path);
                }
                else
                {
                    files++;
                }
            }
        }

        return (files, folders, pending.Count > 0);
    }
}
