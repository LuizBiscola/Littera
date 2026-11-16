using System.Drawing.Text;
using Notas.Settings;
using Notas.Theme;
using Notas.UI.Controls;

namespace Notas.UI.Settings;

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

    }

    private static ComboBox FontPicker(EditorSettings editor)
    {
        var picker = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDown,
            FlatStyle = FlatStyle.Flat,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = AutoCompleteSource.ListItems,
            Width = 220,
        };
        picker.Font = UiFonts.Get(13, picker.DeviceDpi);
        using (var installed = new InstalledFontCollection())
            picker.Items.AddRange([.. installed.Families.Select(f => f.Name)]);
        picker.Text = editor.FontFamily;
        picker.TextChanged += (_, _) => editor.FontFamily = picker.Text;
        return picker;
    }
}
