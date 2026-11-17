using Notas.Editor;
using Notas.Settings;
using Notas.UI.Settings;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private static SettingsPage[] CreateSettingsPages() =>
        [new GeneralPage(), new EditorPage(), new LanguagesPage(), new FilesPage(), new ShortcutsPage()];

    private void ShowSettings()
    {
        _ = Shortcuts; // a página de atalhos lista o settings.json já completo
        using var dialog = new SettingsDialog(_settings, CreateSettingsPages());
        DialogResult result = dialog.ShowDialog(this);
        if (result == DialogResult.Retry)
        {
            SettingsStore.SaveSettings(_settings);
            OpenFile(SettingsStore.SettingsPath); // "Editar no settings.json"
            return;
        }

        if (result != DialogResult.OK)
            return;

        ApplySettings(dialog.Draft);
        SettingsStore.SaveSettings(_settings);
    }

    /// <summary>Reaplica fonte, cores, margens e realce em todas as abas já abertas.</summary>
    private void ApplyEditorSettings()
    {
        foreach (OpenDocument document in _documents.Documents)
        {
            if (document.Editor is not { } editor)
                continue;
            EditorFactory.ApplySettings(editor, _settings.Editor);
            document.Language.Apply(editor);
        }

        _blocks = null;
        UpdateStatus();
    }
}
