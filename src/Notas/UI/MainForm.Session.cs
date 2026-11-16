using Notas.Editor;
using Notas.Editor.Languages;
using Notas.Settings;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Abas da última sessão entram sem carregar; só a ativa lê o arquivo agora.</summary>
    private OpenDocument? RestoreSession()
    {
        SessionState session = SettingsStore.LoadSession();
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

}
