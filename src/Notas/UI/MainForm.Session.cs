using Notas.Editor;
using Notas.Editor.Languages;
using Notas.Settings;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Abas da última sessão entram sem carregar; só a ativa lê o arquivo agora.</summary>
    private OpenDocument? RestoreSession(SessionState session)
    {
        OpenDocument? active = null;
        for (int i = 0; i < session.Documents.Count; i++)
        {
            SessionDocument saved = session.Documents[i];
            if (!File.Exists(saved.Path))
                continue;

            OpenDocument document = _documents.AddUnloaded(saved.Path);
            document.Restore = saved;
            if (LanguageRegistry.ByName(saved.Language) is { } language)
            {
                document.Language = language;
                document.LanguageChosen = true;
            }

            if (i == session.ActiveIndex)
                active = document;
        }

        return active ?? _documents.Documents.FirstOrDefault();
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
