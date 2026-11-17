using Notas.Editor;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Voltou para o Notas (de um Explorer, por exemplo): arquivo apagado ou movido fica com o nome riscado na aba.</summary>
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        CheckMissingFiles();
    }

    private void CheckMissingFiles()
    {
        bool changed = false;
        foreach (OpenDocument document in _documents.Documents)
        {
            bool missing = document.Path is { } path && !document.IsLoading && !File.Exists(path);
            changed |= missing != document.Missing;
            document.Missing = missing;
        }

        if (changed)
            _openFiles.Invalidate();
    }
}
