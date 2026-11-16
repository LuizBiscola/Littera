using Notas.Editor;
using Notas.Settings;
using Notas.UI.Settings;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private static SettingsPage[] CreateSettingsPages() => [new EditorPage()];

    private void ShowSettings()
    {
        using var dialog = new SettingsDialog(_settings, CreateSettingsPages());
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        SettingsCopy.Into(dialog.Draft.Editor, _settings.Editor);
        ApplyEditorSettings();
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
