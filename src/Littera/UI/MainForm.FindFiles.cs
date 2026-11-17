using Littera.Editor;
using Littera.Services;
using Littera.UI.Find;
using ScintillaNET;

namespace Littera.UI;

/// <summary>Abas e pasta para a busca em vários arquivos.</summary>
internal sealed partial class MainForm : IFindFiles
{
    private const int MaxFolderFiles = 5000;

    bool IFindFiles.HasFolder => _folderPanel?.Folder is not null;

    FindFile? IFindFiles.Active => _documents.Active is { } active ? FileOf(active) : null;

    bool IFindFiles.ActiveInScope(SearchScope scope) =>
        scope != SearchScope.Folder || (_documents.Active?.Path is { } path && _folderPanel?.Contains(path) == true);

    async Task<List<FindFile>> IFindFiles.ListAsync(SearchScope scope)
    {
        if (scope == SearchScope.Tabs)
            return [.. _documents.Documents.Select(FileOf)];
        if (_folderPanel?.Folder is not { } folder)
            return [];

        List<FolderEntry> files = await Task.Run(() => FolderScanner.Search(folder, "", limit: MaxFolderFiles));
        return [.. files.Select(f => new FindFile(f.Path, f.Path))];
    }

    Scintilla? IFindFiles.LoadedEditor(FindFile file) => DocumentOf(file) is { IsLoaded: true } document ? document.Editor : null;

    async Task<Scintilla?> IFindFiles.ShowAsync(FindFile file)
    {
        OpenDocument? document = DocumentOf(file);
        if (document is null && file.Path is { } path && File.Exists(path))
            document = _documents.Open(path);
        if (document is null)
            return null;

        _documents.Activate(document);
        while (document.IsLoading)
            await Task.Delay(15); // carga em blocos: espera sem travar a interface
        return document.IsLoaded && _documents.Documents.Contains(document) ? document.Editor : null;
    }

    private static FindFile FileOf(OpenDocument document) =>
        new(document.Path ?? $"*{document.UntitledNumber}", document.SourcePath ?? document.Path);

    private OpenDocument? DocumentOf(FindFile file) =>
        _documents.Documents.FirstOrDefault(d => string.Equals(FileOf(d).Key, file.Key, StringComparison.OrdinalIgnoreCase));
}
