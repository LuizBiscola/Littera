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
        if (!hasDraft && !hasFile)
            return null;

        var document = new OpenDocument(saved.Path.Length > 0 ? Path.GetFullPath(saved.Path) : null, saved.UntitledNumber) { Restore = saved };
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

    private void SaveSession()
    {
        var session = new SessionState();
        foreach (OpenDocument document in _documents.Documents.Where(d => d.Path is not null))
        {
            if (document == _documents.Active)
                session.ActiveIndex = session.Documents.Count;
            session.Documents.Add(SessionEntry(document));
        }

        SettingsStore.SaveSession(session);
    }

    /// <summary>Aba nunca vista mantém a posição que veio da sessão anterior.</summary>
    private static SessionDocument SessionEntry(OpenDocument document) => document.Editor is { } editor && document.IsLoaded
        ? new SessionDocument
        {
            Path = document.Path!,
            CaretPosition = editor.CurrentPosition,
            AnchorPosition = editor.AnchorPosition,
            FirstVisibleLine = editor.FirstVisibleLine,
            Language = document.LanguageChosen ? document.Language.Name : null,
        }
        : document.Restore ?? new SessionDocument { Path = document.Path! };
}
