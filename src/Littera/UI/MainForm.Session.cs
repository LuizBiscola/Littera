using Notas.Editor;
using Notas.Editor.Languages;
using Notas.Services;
using Notas.Settings;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Abas da última sessão entram sem carregar; só a ativa lê o arquivo (ou o rascunho) agora.</summary>
    private OpenDocument? RestoreSession(SessionState session)
    {
        if (session.Folder is { } folder && Directory.Exists(folder))
        {
            OpenFolder(folder, session.ExpandedFolders);
            SetFolderShown(!session.FolderHidden);
        }

        OpenDocument? active = null;
        for (int i = 0; i < session.Documents.Count; i++)
        {
            if (Restored(session.Documents[i]) is not { } document)
                continue;

            _documents.AddUnloaded(document);
            if (i == session.ActiveIndex)
                active = document;
        }

        return active ?? _documents.Documents.FirstOrDefault();
    }

    /// <summary>O arquivo, ou o rascunho com as alterações que não foram salvas (inclusive "Sem título").</summary>
    private static OpenDocument? Restored(SessionDocument saved)
    {
        bool hasDraft = saved.Draft is { } id && File.Exists(DraftStore.PathFor(id));
        bool hasFile = saved.Path.Length > 0 && File.Exists(saved.Path);
        if (!hasDraft && !hasFile && !saved.Pinned)
            return null; // fixada continua na lista mesmo sem arquivo, com o nome riscado

        var document = new OpenDocument(saved.Path.Length > 0 ? Path.GetFullPath(saved.Path) : null, saved.UntitledNumber)
        {
            Restore = saved,
            Pinned = saved.Pinned,
            Missing = saved.Path.Length > 0 && !hasFile && !hasDraft,
            IsLoaded = saved.Path.Length == 0 && !hasDraft, // "Sem título" fixada e vazia
        };
        if (hasDraft)
        {
            document.DraftId = saved.Draft;
            document.SourcePath = DraftStore.PathFor(saved.Draft!);
            document.HasUnsavedDraft = true;
        }

        if (LanguageRegistry.ByName(saved.Language) is { } language)
        {
            document.Language = language;
            document.LanguageChosen = true;
        }

        return document;
    }
}
