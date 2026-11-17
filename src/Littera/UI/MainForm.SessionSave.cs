using Notas.Editor;
using Notas.Services;
using Notas.Settings;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Arquivos abertos e rascunhos (com o formato original); rascunhos que sobraram são apagados.</summary>
    private void SaveSession()
    {
        var session = new SessionState
        {
            Folder = _folderPanel?.Folder,
            FolderHidden = !_folderShown,
            ExpandedFolders = [.. _folderPanel?.ExpandedFolders ?? []],
        };
        foreach (OpenDocument document in _documents.Documents.Where(KeepInSession))
        {
            if (document == _documents.Active)
                session.ActiveIndex = session.Documents.Count;
            SessionDocument entry = SessionEntry(document);
            entry.Pinned = document.Pinned;
            session.Documents.Add(entry);
        }

        SettingsStore.SaveSession(session);
        DraftStore.DeleteAllExcept(session.Documents.Select(d => d.Draft).OfType<string>());
    }

    /// <summary>Fixadas sempre voltam; com alterações não salvas também (nada se perde); as outras, se "Reabrir sessão" estiver ligado.</summary>
    private bool KeepInSession(OpenDocument document) =>
        document.Pinned || document.DraftId is not null || (_settings.Window.ReopenSession && document.Path is not null);

    /// <summary>Aba nunca vista mantém o que veio da sessão anterior.</summary>
    private static SessionDocument SessionEntry(OpenDocument document) => document.Editor is { } editor && document.IsLoaded
        ? new SessionDocument
        {
            Path = document.Path ?? "",
            CaretPosition = editor.CurrentPosition,
            AnchorPosition = editor.AnchorPosition,
            FirstVisibleLine = editor.FirstVisibleLine,
            Language = document.LanguageChosen ? document.Language.Name : null,
            Draft = document.DraftId,
            UntitledNumber = document.UntitledNumber,
            Encoding = document.Format.Encoding.ToString(),
            LineEnding = document.Format.LineEnding.ToString(),
            Folded = FoldedHeaders(document, editor),
        }
        : document.Restore ?? new SessionDocument { Path = document.Path ?? "" };
}
