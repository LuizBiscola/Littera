using Notas.Settings;
using Notas.UI.Controls;

namespace Notas.UI.Settings;

/// <summary>Página "Atalhos": mostra os atalhos; a edição é no settings.json (aberto numa aba pelo botão).</summary>
internal sealed partial class ShortcutsPage : SettingsPage
{
    public override string Name => "Atalhos";

    public override char Icon => ''; // "Keyboard"

    public override string Subtitle => "Troque no settings.json: \"comando\": \"Ctrl+Shift+F\" (vários separados por ;).";

    public override IEnumerable<Control> Build(AppSettings draft)
    {
        var edit = new FlatButton("Editar no settings.json", FlatButtonKind.Secondary);
        edit.Click += (_, _) => edit.FindForm()!.DialogResult = DialogResult.Retry; // quem abriu abre o arquivo numa aba
        yield return new SettingRow("Arquivo de configurações", "Salve o arquivo e os atalhos novos já valem.", edit);

        yield return Section("COMANDOS");
        yield return new ShortcutList(draft.Shortcuts);
    }

    public override void Reset(AppSettings draft) => draft.Shortcuts.Clear(); // na próxima abertura voltam os padrões
}
