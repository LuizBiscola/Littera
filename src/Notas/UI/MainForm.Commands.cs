namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Comandos com nome estável (a chave no settings.json) e o atalho padrão.</summary>
    private (string Name, string Keys, Action Run)[] CommandTable() =>
    [
        ("novo", "Ctrl+N", () => _documents.New()),
        ("abrir", "Ctrl+O", ShowOpenDialog),
        ("salvar", "Ctrl+S", () => _ = SaveActiveAsync(askPath: false)),
        ("salvarComo", "Ctrl+Shift+S", () => _ = SaveActiveAsync(askPath: true)),
        ("fecharAba", "Ctrl+W", CloseActive),
        ("proximaAba", "Ctrl+Tab", () => CycleTabs(+1)),
        ("abaAnterior", "Ctrl+Shift+Tab", () => CycleTabs(-1)),
        ("painelAbas", "Ctrl+B", ToggleOpenFiles),
        ("abrirPasta", "Ctrl+Shift+O", ShowOpenFolderDialog),
        ("painelPasta", "Ctrl+Shift+E", ToggleFolderPanel),
        ("irParaArquivo", "Ctrl+P", GoToFile),
        ("modoFoco", "F11", ToggleFocusMode),
        ("buscar", "Ctrl+F", () => FindPanel.Open(replace: false)),
        ("substituir", "Ctrl+H", () => FindPanel.Open(replace: true)),
        ("proximaOcorrencia", "F3", () => FindPanel.Step(+1)),
        ("ocorrenciaAnterior", "Shift+F3", () => FindPanel.Step(-1)),
        ("irParaLinha", "Ctrl+G", ShowGoToLine),
        ("formatarSql", "Ctrl+Shift+F", FormatSql),
        ("configuracoes", "Ctrl+Comma", ShowSettings),
        ("aumentarZoom", "Ctrl+Plus; Ctrl+Add", () => Zoom(+1)),
        ("diminuirZoom", "Ctrl+Minus; Ctrl+Subtract", () => Zoom(-1)),
        ("zoomPadrao", "Ctrl+0; Ctrl+NumPad0", () => Zoom(0)),
        ("quebraDeLinha", "Alt+Z", ToggleWordWrap),
        ("numerosDeLinha", "Ctrl+Shift+L", ToggleLineNumbers),
    ];

    private void CloseActive()
    {
        if (_documents.Active is { } active)
            _ = RequestCloseAsync(active);
    }

    /// <summary>Texto do atalho de um comando para mostrar no menu (o primeiro, se houver vários).</summary>
    private string? ShortcutText(string command) =>
        _settings.Shortcuts.TryGetValue(command, out string? keys) ? keys.Split(';')[0].Trim() : null;
}
