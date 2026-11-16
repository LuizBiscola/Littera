using Notas.Services;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private Dictionary<Keys, Action>? _shortcuts;

    private Dictionary<Keys, Action> Shortcuts => _shortcuts ??= BuildShortcuts();

    /// <summary>Atalhos do settings.json; comando que não estiver lá ganha o padrão e passa a aparecer no arquivo.</summary>
    private Dictionary<Keys, Action> BuildShortcuts()
    {
        var map = new Dictionary<Keys, Action>();
        foreach ((string name, string defaults, Action run) in CommandTable())
        {
            if (!_settings.Shortcuts.TryGetValue(name, out string? keys))
                _settings.Shortcuts[name] = keys = defaults;
            foreach (Keys key in KeyText.ParseAll(keys))
                map[key] = run;
        }

        return map;
    }

    /// <summary>Roda antes do editor ver a tecla: o Scintilla não engole Ctrl+Tab e companhia.</summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape && HandleEscape())
            return true;

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
