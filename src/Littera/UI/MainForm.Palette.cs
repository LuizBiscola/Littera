using Notas.Editor.Languages;
using Notas.Services;
using Notas.UI.Commands;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private readonly List<string> _recentCommands = []; // usados há pouco aparecem primeiro (só nesta sessão)
    private CommandPalette? _palette;

    /// <summary>Ctrl+Shift+P; criada no primeiro uso, como a busca.</summary>
    private void ShowCommandPalette()
    {
        if (_palette is null)
        {
            _palette = new CommandPalette();
            _palette.Chosen += RunFromPalette;
            _editorHost.Controls.Add(_palette);
        }

        _ = Shortcuts; // os atalhos mostrados são os do settings.json
        _palette.Open(PaletteItems());
    }

    private void RunFromPalette(PaletteItem item)
    {
        _palette!.Dismiss();
        _recentCommands.Remove(item.Label);
        _recentCommands.Insert(0, item.Label);
        _boundEditor?.Focus(); // o comando roda como se o atalho fosse apertado no editor
        try
        {
            item.Run();
        }
        catch (Exception ex)
        {
            ErrorReport.Show(ex, this);
        }
    }

    /// <summary>Comandos, troca de linguagem, encoding e quebra de linha do arquivo ativo, e arquivos recentes.</summary>
    private List<PaletteItem> PaletteItems()
    {
        List<PaletteItem> items = [.. CommandTable().Where(c => c.Name != "paletaDeComandos")
            .Select(c => new PaletteItem(c.Label, ShortcutText(c.Name), null, c.Run))];
        if (_documents.Active is { IsLoading: false } document)
        {
            items.AddRange(LanguageRegistry.All.Select(l => new PaletteItem($"Linguagem: {l.Name}", null, null, () => _documents.SetLanguage(document, l))));
            items.AddRange(Enum.GetValues<TextEncodingKind>().Select(k => new PaletteItem($"Salvar com encoding: {(k == TextEncodingKind.Ansi ? "ANSI (Windows-1252)" : k.Label())}",
                null, null, () => _documents.SetEncoding(document, k))));
            items.AddRange(Enum.GetValues<LineEnding>().Select(e => new PaletteItem($"Converter quebras de linha: {e.Label()}", null, null, () => _documents.SetLineEnding(document, e))));
        }

        items.AddRange(_settings.RecentFiles.Select(p => new PaletteItem($"Abrir recente: {Path.GetFileName(p)}", null, Path.GetDirectoryName(p), () => OpenFile(p))));
        return [.. items.OrderBy(i => _recentCommands.IndexOf(i.Label) is >= 0 and var r ? r : int.MaxValue)];
    }
}
