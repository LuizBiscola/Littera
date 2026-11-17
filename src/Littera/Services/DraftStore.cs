using System.Text;
using Notas.Settings;

namespace Notas.Services;

/// <summary>
/// Rascunhos das abas com alterações não salvas (inclusive "Sem título"), em &lt;pasta de dados&gt;\rascunhos.
/// Gravados sempre em UTF-8; a sessão guarda o formato original para salvar do jeito certo depois.
/// </summary>
internal static class DraftStore
{
    public static string Folder { get; } = Path.Combine(SettingsStore.Folder, "rascunhos");

    public static string PathFor(string id) => Path.Combine(Folder, id + ".txt");

    /// <summary>Grava por arquivo temporário: um rascunho nunca fica pela metade.</summary>
    public static void Write(string id, string text)
    {
        Directory.CreateDirectory(Folder);
        string path = PathFor(id);
        File.WriteAllText(path + ".tmp", text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        File.Move(path + ".tmp", path, overwrite: true);
    }

    public static void Delete(string? id)
    {
        if (id is not null)
            File.Delete(PathFor(id));
    }

    /// <summary>Apaga rascunhos que nenhuma aba da sessão usa mais.</summary>
    public static void DeleteAllExcept(IEnumerable<string> keep)
    {
        if (!Directory.Exists(Folder))
            return;

        var used = new HashSet<string>(keep.Select(PathFor), StringComparer.OrdinalIgnoreCase);
        foreach (string file in Directory.EnumerateFiles(Folder).Where(f => !used.Contains(f)))
            File.Delete(file);
    }
}
