using Notas.Services;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private Dictionary<Keys, Action>? _shortcuts;

    /// <summary>Atalhos da janela; na Fase 2 esta tabela passa a ser lida do settings.json.</summary>
    private Dictionary<Keys, Action> Shortcuts => _shortcuts ??= new()
    {
        [Keys.Control | Keys.N] = () => _documents.New(),
        [Keys.Control | Keys.O] = ShowOpenDialog,
        [Keys.Control | Keys.S] = () => _ = SaveActiveAsync(askPath: false),
        [Keys.Control | Keys.Shift | Keys.S] = () => _ = SaveActiveAsync(askPath: true),
        [Keys.Control | Keys.W] = () => _ = _documents.Active is { } active ? RequestCloseAsync(active) : Task.CompletedTask,
        [Keys.Control | Keys.B] = ToggleOpenFiles,
        [Keys.Control | Keys.F] = () => FindPanel.Open(replace: false),
        [Keys.Control | Keys.H] = () => FindPanel.Open(replace: true),
        [Keys.Control | Keys.G] = ShowGoToLine,
        [Keys.Control | Keys.Shift | Keys.F] = FormatSql,
        [Keys.Control | Keys.Oemcomma] = ShowSettings,
        [Keys.F3] = () => FindPanel.Step(+1),
        [Keys.Shift | Keys.F3] = () => FindPanel.Step(-1),
        [Keys.Control | Keys.Oemplus] = () => Zoom(+1),
        [Keys.Control | Keys.Add] = () => Zoom(+1),
        [Keys.Control | Keys.OemMinus] = () => Zoom(-1),
        [Keys.Control | Keys.Subtract] = () => Zoom(-1),
        [Keys.Control | Keys.D0] = () => Zoom(0),
        [Keys.Control | Keys.NumPad0] = () => Zoom(0),
        [Keys.Alt | Keys.Z] = ToggleWordWrap,
        [Keys.Control | Keys.Shift | Keys.L] = ToggleLineNumbers,
        [Keys.Control | Keys.Tab] = () => CycleTabs(+1),
        [Keys.Control | Keys.Shift | Keys.Tab] = () => CycleTabs(-1),
    };

    /// <summary>Roda antes do editor ver a tecla: o Scintilla não engole Ctrl+Tab e companhia.</summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape && _findPanel?.Visible == true)
        {
            _findPanel.Dismiss();
            return true;
        }

        if (Shortcuts.TryGetValue(keyData, out Action? action))
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ErrorReport.Show(ex, this); // exceções aqui seriam engolidas pelo laço de mensagens
            }

            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private Task SaveActiveAsync(bool askPath) =>
        _documents.Active is { } active ? SaveAsync(active, askPath) : Task.CompletedTask;

    private void CycleTabs(int step)
    {
        int count = _documents.Documents.Count;
        if (count < 2 || _documents.Active is null)
            return;

        int index = _documents.Documents.ToList().IndexOf(_documents.Active);
        _documents.Activate(_documents.Documents[(index + step + count) % count]);
    }
}
