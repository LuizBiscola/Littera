using Notas.Editor;
using Notas.Services;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private const string FileFilter =
        "Todos os arquivos (*.*)|*.*|SQL (*.sql)|*.sql|Texto (*.txt;*.md;*.log)|*.txt;*.md;*.log|JSON e XML (*.json;*.xml)|*.json;*.xml";

    private void ShowOpenDialog()
    {
        using var dialog = new OpenFileDialog { Filter = FileFilter, Multiselect = true, Title = "Abrir" };
        if (_documents.Active?.Path is { } current)
            dialog.InitialDirectory = Path.GetDirectoryName(current);

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            foreach (string file in dialog.FileNames)
                OpenFile(file);
        }
    }

    private void OpenFile(string path)
    {
        if (!File.Exists(path))
        {
            _settings.RecentFiles.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
            MessageBox.Show(this, $"Arquivo não encontrado:\n{path}", Product.Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Uma aba "Sem título" vazia e intocada dá lugar ao arquivo, como em outros editores.
        OpenDocument? blank = _documents.Active is { Path: null, IsDirty: false } active
            && active.Editor?.TextLength == 0 ? active : null;

        _documents.Open(path);
        RecentFiles.Add(_settings.RecentFiles, Path.GetFullPath(path));
        if (blank is not null)
            _documents.Close(blank);
    }

    private void EnableFileDrop()
    {
        foreach (Control area in new Control[] { _editorHost, _openFiles, _status, TitleBar })
            FileDrop.Attach(area, OpenFile);
        foreach (OpenDocument document in _documents.Documents.Where(d => d.Editor is not null))
            EditorDrop.Attach(document.Editor!, OpenFile);
        _documents.EditorCreated += editor => EditorDrop.Attach(editor, OpenFile);
    }
}
