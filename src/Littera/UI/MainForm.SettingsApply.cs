using Littera.Editor;
using Littera.Settings;

namespace Littera.UI;

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
        _settings.Window.ReopenSession = source.Window.ReopenSession;
        if (source.Window.ShowOpenFiles != _settings.Window.ShowOpenFiles)
            ToggleOpenFiles();
        _documents.NewFileFormat = NewFileFormat(_settings.Files);
        ApplyEditorSettings();
    }

    /// <summary>settings.json salvo dentro do Littera: vale na hora (atalhos inclusive).</summary>
    private void OnDocumentSaved(OpenDocument document)
    {
        document.Missing = false; // salvar recria o arquivo que tinha sumido
        if (document.Path is { } saved && _folderPanel?.Contains(saved) == true)
            _folderPanel.Reload(); // "Salvar como" pode ter criado um arquivo novo na pasta
        InvalidateChanges(); // salvar troca as barras laranja por verdes
        ScheduleHighlights(textChanged: false);

        if (!string.Equals(document.Path, Path.GetFullPath(SettingsStore.SettingsPath), StringComparison.OrdinalIgnoreCase))
            return;

        if (SettingsStore.TryReloadSettings(out string? error) is { } fresh)
            ApplySettings(fresh);
        else
            MessageBox.Show(this, $"O settings.json tem um erro e não foi aplicado:\n{error}", Product.Name,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
