using Notas.Editor;
using Notas.Settings;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private bool _closeConfirmed;

    private async Task RequestCloseAsync(OpenDocument document)
    {
        if (await ConfirmSaveAsync(document))
            _documents.Close(document);
    }

    /// <summary>"Salvar alterações?" para documento alterado; false = o usuário cancelou.</summary>
    private async Task<bool> ConfirmSaveAsync(OpenDocument document)
    {
        if (!document.IsDirty)
            return true;

        _documents.Activate(document);
        DialogResult answer = MessageBox.Show(this, $"Salvar as alterações em \"{document.Title}\"?", "Notas",
            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        return answer switch
        {
            DialogResult.Yes => await SaveAsync(document),
            DialogResult.No => true,
            _ => false,
        };
    }

    /// <summary>FormClosing não espera tarefas: com algo alterado, cancela, pergunta e fecha de novo depois.</summary>
    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        if (e.Cancel || _closeConfirmed || !_documents.Documents.Any(d => d.IsDirty))
        {
            if (!e.Cancel)
                SaveState();
            return;
        }

        e.Cancel = true;
        foreach (OpenDocument document in _documents.Documents.ToList())
        {
            if (!await ConfirmSaveAsync(document))
                return;
        }

        _closeConfirmed = true;
        BeginInvoke(Close);
    }

    /// <summary>Configurações, posição da janela e abas abertas: tudo para a próxima abertura.</summary>
    private void SaveState()
    {
        SaveWindowPlacement();
        SaveSession();
        SettingsStore.SaveSettings(_settings);
    }
}
