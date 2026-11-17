using Littera.Settings;
using Littera.UI.Controls;

namespace Littera.UI.Settings;

/// <summary>Página "Linguagens": o formatador de SQL (Ctrl+Shift+F).</summary>
internal sealed class LanguagesPage : SettingsPage
{
    private static readonly string[] Cases = ["upper", "lower", "keep"];

    public override string Name => "Linguagens";

    public override char Icon => ''; // "Code"

    public override string Subtitle => "Como o Formatar SQL (Ctrl+Shift+F) arruma o código.";

    public override IEnumerable<Control> Build(AppSettings draft)
    {
        SqlFormatSettings sql = draft.SqlFormat;
        yield return Section("FORMATAR SQL");

        var keywordCase = new Segmented("MAIÚSCULAS", "minúsculas", "Manter") { SelectedIndex = Math.Max(0, Array.IndexOf(Cases, sql.KeywordCase)) };
        keywordCase.SelectedIndexChanged += () => sql.KeywordCase = Cases[keywordCase.SelectedIndex];
        yield return new SettingRow("Palavras-chave", "SELECT, COUNT, INT…", keywordCase);

        yield return Toggle("Uma coluna por linha", "Colunas do SELECT alinhadas abaixo da primeira.",
            sql.OneColumnPerLine, v => sql.OneColumnPerLine = v);
        yield return Toggle("AND e OR em linhas próprias", "Condições do WHERE e do HAVING alinhadas à direita da cláusula.",
            sql.BreakConditions, v => sql.BreakConditions = v);
    }

    public override void Reset(AppSettings draft) => SettingsCopy.Into(new SqlFormatSettings(), draft.SqlFormat);
}
