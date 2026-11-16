using Notas.Settings;
using Notas.UI.Controls;

namespace Notas.UI.Settings;

internal sealed partial class EditorPage
{
    private static IEnumerable<Control> BehaviorRows(EditorSettings editor)
    {
        var indent = new Segmented("Espaços 2", "Espaços 4", "Tab")
        {
            SelectedIndex = editor.UseTabs ? 2 : editor.IndentSize == 2 ? 0 : 1,
        };
        indent.SelectedIndexChanged += () =>
        {
            editor.UseTabs = indent.SelectedIndex == 2;
            editor.IndentSize = indent.SelectedIndex == 0 ? 2 : 4;
        };
        yield return new SettingRow("Indentação", "Usada ao pressionar Tab.", indent);

        yield return Toggle("Quebra de linha automática", "Linhas longas continuam na linha de baixo.",
            editor.WordWrap, v => editor.WordWrap = v);
        yield return Toggle("Mostrar números de linha", null, editor.ShowLineNumbers, v => editor.ShowLineNumbers = v);

        yield return Section("DESTAQUES");
        yield return Toggle("Destacar linha atual", null, editor.HighlightCurrentLine, v => editor.HighlightCurrentLine = v);
        yield return Toggle("Destacar bloco atual",
            "Marca o trecho onde está o cursor: a instrução inteira em SQL, o parágrafo em texto comum.",
            editor.HighlightCurrentBlock, v => editor.HighlightCurrentBlock = v);
        yield return Toggle("Destacar ocorrências da palavra", "Ao posicionar o cursor, marca a mesma palavra no arquivo.",
            editor.HighlightOccurrences, v => editor.HighlightOccurrences = v);
    }
}
