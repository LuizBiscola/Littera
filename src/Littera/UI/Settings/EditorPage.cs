using System.Drawing.Text;
using Littera.Settings;
using Littera.Theme;
using Littera.UI.Controls;

namespace Littera.UI.Settings;

/// <summary>Página "Editor" do protótipo: aparência do texto e comportamento ao digitar.</summary>
internal sealed partial class EditorPage : SettingsPage
{
    public override string Name => "Editor";

    public override char Icon => ''; // "Font" na Segoe Fluent

    public override string Subtitle => "Aparência do texto e comportamento ao digitar.";

    public override IEnumerable<Control> Build(AppSettings draft)
    {
        EditorSettings editor = draft.Editor;
        yield return Section("TEXTO");
        yield return new SettingRow("Fonte", "Qualquer fonte instalada; as monoespaçadas alinham melhor o código.", FontPicker(editor));

        // O protótipo mostra o tamanho em pixels (13); guardamos em pontos, que é o que o Scintilla usa.
        var size = new Stepper { Minimum = 8, Maximum = 40, Increment = 1, Value = MathF.Round(editor.FontSize * 96f / 72f) };
        size.ValueChanged += () => editor.FontSize = size.Value * 72f / 96f;
        yield return new SettingRow("Tamanho da fonte", null, size);

        var spacing = new Stepper { Minimum = 1.0f, Maximum = 2.5f, Increment = 0.1f, Format = "0.0", Value = editor.LineSpacing };
        spacing.ValueChanged += () => editor.LineSpacing = spacing.Value;
        yield return new SettingRow("Espaçamento entre linhas", "Altura da linha em relação à fonte (1,7 = protótipo).", spacing);

        foreach (Control row in BehaviorRows(editor))
            yield return row;
    }

    public override void Reset(AppSettings draft) => SettingsCopy.Into(new EditorSettings(), draft.Editor);

    private static FontBox FontPicker(EditorSettings editor)
    {
        string[] fonts;
        using (var installed = new InstalledFontCollection())
            fonts = [.. installed.Families.Select(f => f.Name)];
        var picker = new FontBox(fonts) { Value = editor.FontFamily };
        picker.ValueChanged += () => editor.FontFamily = picker.Value;
        return picker;
    }
}
