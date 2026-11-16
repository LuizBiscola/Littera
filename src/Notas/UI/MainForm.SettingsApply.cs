using Notas.Editor;
using Notas.Settings;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Copia para as configurações vivas (que editores e documentos já referenciam) e reaplica.</summary>
    private void ApplySettings(AppSettings source)
    {
        SettingsCopy.Into(source.Editor, _settings.Editor);
        SettingsCopy.Into(source.SqlFormat, _settings.SqlFormat);
        SettingsCopy.Into(source.Files, _settings.Files);
        _settings.Shortcuts = source.Shortcuts;
        _shortcuts = null;
        if (source.Window.ShowOpenFiles != _settings.Window.ShowOpenFiles)
            ToggleOpenFiles();
        _documents.NewFileFormat = NewFileFormat(_settings.Files);
        ApplyEditorSettings();
    }

    /// <summary>settings.json salvo dentro do Notas: vale na hora (atalhos inclusive).</summary>
    private void OnDocumentSaved(OpenDocument document)
    {
        if (document.Path is { } saved && _folderPanel?.Contains(saved) == true)
            _folderPanel.Reload(); // "Salvar como" pode ter criado um arquivo novo na pasta
        _changedLines = null; // salvar troca as barras laranja por verdes
        ScheduleHighlights(textChanged: false);

        if (!string.Equals(document.Path, Path.GetFullPath(SettingsStore.SettingsPath), StringComparison.OrdinalIgnoreCase))
            return;

        if (SettingsStore.TryReloadSettings(out string? error) is { } fresh)
            ApplySettings(fresh);
        else
            MessageBox.Show(this, $"O settings.json tem um erro e não foi aplicado:\n{error}", "Notas",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
