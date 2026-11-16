using Notas.Settings;
using Notas.Theme;
using Notas.UI.Controls;

namespace Notas.UI.Settings;

/// <summary>Página "Atalhos": mostra os atalhos; a edição é no settings.json (aberto numa aba pelo botão).</summary>
internal sealed class ShortcutsPage : SettingsPage
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
        foreach ((string command, string keys) in draft.Shortcuts.OrderBy(s => s.Key))
            yield return new SettingRow(command, null, new KeyCap(keys));
    }

    public override void Reset(AppSettings draft) => draft.Shortcuts.Clear(); // na próxima abertura voltam os padrões

    /// <summary>Atalho desenhado como tecla: fundo cinza claro e cantos arredondados.</summary>
    private sealed class KeyCap : Control
    {
        public KeyCap(string keys)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            Text = keys;
            Size = new Size(TextRenderer.MeasureText(keys, UiFonts.Get(12, DeviceDpi)).Width + this.Px(16), this.Px(26));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Palette.Background);
            Shapes.FillRounded(e.Graphics, Palette.ItemHover, new Rectangle(0, 0, Width, Height), this.Px(5));
            TextRenderer.DrawText(e.Graphics, Text, UiFonts.Get(12, DeviceDpi), new Rectangle(0, 0, Width, Height), Palette.TextSecondary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }
}
