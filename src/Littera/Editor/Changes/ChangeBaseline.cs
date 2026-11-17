namespace Littera.Editor.Changes;

/// <summary>
/// Versões de referência de um documento, por linhas: como estava ao abrir (Original) e como está no disco (Saved).
/// Laranja = editor diferente do disco; verde = disco diferente do que abriu.
/// </summary>
internal sealed class ChangeBaseline(string[] original)
{
    /// <summary>Acima disso não há marcação: guardar e comparar cópias de arquivos enormes não compensa.</summary>
    public const int MaxChars = 2_000_000;

    public string[] Original { get; } = original;

    public string[] Saved { get; private set; } = original;

    public void MarkSaved(string text) => Saved = Lines(text);

    /// <summary>Arquivo novo salvo pela primeira vez: tudo o que foi escrito conta como alterado e salvo.</summary>
    public static ChangeBaseline ForNewFile(string text)
    {
        var baseline = new ChangeBaseline([""]);
        baseline.MarkSaved(text);
        return baseline;
    }

    public static string[] Lines(string text) => text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
}
