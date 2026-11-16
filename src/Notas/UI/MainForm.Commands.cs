using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Comandos: nome estável (a chave no settings.json), nome na paleta, atalho padrão e ação.</summary>
    private (string Name, string Label, string Keys, Action Run)[] CommandTable() =>
    [
        ("paletaDeComandos", "Paleta de comandos", "Ctrl+Shift+P", ShowCommandPalette),
        ("novo", "Novo arquivo", "Ctrl+N", () => _documents.New()),
        ("abrir", "Abrir arquivo…", "Ctrl+O", ShowOpenDialog),
        ("salvar", "Salvar", "Ctrl+S", () => _ = SaveActiveAsync(askPath: false)),
        ("salvarComo", "Salvar como…", "Ctrl+Shift+S", () => _ = SaveActiveAsync(askPath: true)),
        ("fecharAba", "Fechar aba", "Ctrl+W", CloseActive),
        ("proximaAba", "Próxima aba", "Ctrl+Tab", () => CycleTabs(+1)),
        ("abaAnterior", "Aba anterior", "Ctrl+Shift+Tab", () => CycleTabs(-1)),
        ("painelAbas", "Mostrar ou ocultar o painel de abas", "Ctrl+B", ToggleOpenFiles),
        ("abrirPasta", "Abrir pasta…", "Ctrl+Shift+O", ShowOpenFolderDialog),
        ("painelPasta", "Mostrar ou ocultar o painel de pasta", "Ctrl+Shift+E", ToggleFolderPanel),
        ("irParaArquivo", "Ir para arquivo da pasta", "Ctrl+P", GoToFile),
        ("modoFoco", "Modo foco", "F11", ToggleFocusMode),
        ("buscar", "Buscar", "Ctrl+F", () => FindPanel.Open(replace: false)),
        ("substituir", "Substituir", "Ctrl+H", () => FindPanel.Open(replace: true)),
        ("proximaOcorrencia", "Próxima ocorrência", "F3", () => FindPanel.Step(+1)),
        ("ocorrenciaAnterior", "Ocorrência anterior", "Shift+F3", () => FindPanel.Step(-1)),
        ("irParaLinha", "Ir para linha…", "Ctrl+G", ShowGoToLine),
        ("formatarSql", "Formatar SQL", "Ctrl+Shift+F", FormatSql),
        ("proximaAlteracao", "Próxima alteração", "Alt+F5", () => GoToChange(+1)),
        ("alteracaoAnterior", "Alteração anterior", "Shift+Alt+F5", () => GoToChange(-1)),
        ("duplicarLinha", "Duplicar linha ou seleção", "Ctrl+D", () => InEditor(e => e.ExecuteCmd(Command.SelectionDuplicate))),
        ("moverLinhaAcima", "Mover linha para cima", "Alt+Up", () => InEditor(e => e.ExecuteCmd(Command.MoveSelectedLinesUp))),
        ("moverLinhaAbaixo", "Mover linha para baixo", "Alt+Down", () => InEditor(e => e.ExecuteCmd(Command.MoveSelectedLinesDown))),
        ("comentarLinha", "Comentar ou descomentar linha", "Ctrl+/; Ctrl+Divide", () => InEditor(ToggleLineComment)),
        ("recolherBloco", "Recolher trecho", "Ctrl+Shift+[", CollapseCurrentFold),
        ("expandirBloco", "Expandir trecho", "Ctrl+Shift+]", ExpandCurrentFold),
        ("recolherTudo", "Recolher tudo", "", () => SetAllFolds(expand: false)),
        ("expandirTudo", "Expandir tudo", "", () => SetAllFolds(expand: true)),
        ("configuracoes", "Configurações", "Ctrl+Comma", ShowSettings),
        ("aumentarZoom", "Aumentar zoom", "Ctrl+Plus; Ctrl+Add", () => Zoom(+1)),
        ("diminuirZoom", "Diminuir zoom", "Ctrl+Minus; Ctrl+Subtract", () => Zoom(-1)),
        ("zoomPadrao", "Zoom padrão", "Ctrl+0; Ctrl+NumPad0", () => Zoom(0)),
        ("quebraDeLinha", "Quebra de linha automática", "Alt+Z", ToggleWordWrap),
        ("numerosDeLinha", "Mostrar ou ocultar números de linha", "Ctrl+Shift+L", ToggleLineNumbers),
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
