using Notas.Settings;
using Notas.UI.Controls;

namespace Notas.UI.Settings;

/// <summary>Página "Arquivos": rascunhos automáticos e formato de arquivos novos.</summary>
internal sealed class FilesPage : SettingsPage
{
    private static readonly string[] Encodings = ["Utf8", "Utf8Bom", "Ansi"];
    private static readonly string[] LineEndings = ["CrLf", "Lf"];

    public override string Name => "Arquivos";

    public override char Icon => ''; // "Folder"

    public override string Subtitle => "Rascunhos e formato de arquivos novos.";

    public override IEnumerable<Control> Build(AppSettings draft)
    {
        FilesSettings files = draft.Files;
        yield return Section("RASCUNHOS");
        yield return Toggle("Guardar alterações ao fechar", "Fecha sem perguntar: o que não foi salvo volta na próxima abertura.",
            files.KeepDraftsOnClose, v => files.KeepDraftsOnClose = v);

        var interval = new Stepper { Minimum = 1, Maximum = 60, Value = files.AutosaveSeconds };
        interval.ValueChanged += () => files.AutosaveSeconds = (int)interval.Value;
        yield return new SettingRow("Salvar rascunho após (segundos)", "Tempo sem digitar até guardar as alterações.", interval);

        yield return Section("ARQUIVOS NOVOS");
        var encoding = new Segmented("UTF-8", "UTF-8 BOM", "ANSI") { SelectedIndex = Math.Max(0, Array.IndexOf(Encodings, files.NewFileEncoding)) };
        encoding.SelectedIndexChanged += () => files.NewFileEncoding = Encodings[encoding.SelectedIndex];
        yield return new SettingRow("Encoding", null, encoding);

        var lineEnding = new Segmented("CRLF", "LF") { SelectedIndex = Math.Max(0, Array.IndexOf(LineEndings, files.NewFileLineEnding)) };
        lineEnding.SelectedIndexChanged += () => files.NewFileLineEnding = LineEndings[lineEnding.SelectedIndex];
        yield return new SettingRow("Quebra de linha", "CRLF é o padrão do Windows; LF, do Linux e do Git.", lineEnding);
    }

    public override void Reset(AppSettings draft) => SettingsCopy.Into(new FilesSettings(), draft.Files);
}
