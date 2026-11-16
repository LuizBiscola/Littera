using Notas.Editor;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Zoom vale para todas as abas (Ctrl+roda do mouse já é nativo do Scintilla).</summary>
    private void Zoom(int step)
    {
        int zoom = step == 0 ? 0 : Math.Clamp((_documents.Active?.Editor?.Zoom ?? 0) + step, -10, 20);
        ForEachEditor(editor => editor.Zoom = zoom);
    }

    private void ToggleWordWrap()
    {
        _settings.Editor.WordWrap = !_settings.Editor.WordWrap;
        ForEachEditor(editor => editor.WrapMode = _settings.Editor.WordWrap ? WrapMode.Word : WrapMode.None);
    }

    private void ToggleLineNumbers()
    {
        _settings.Editor.ShowLineNumbers = !_settings.Editor.ShowLineNumbers;
        ForEachEditor(editor => EditorMargins.Apply(editor, _settings.Editor));
    }

    private void ForEachEditor(Action<Scintilla> action)
    {
        foreach (OpenDocument document in _documents.Documents)
        {
            if (document.Editor is { } editor)
                action(editor);
        }
    }
}
