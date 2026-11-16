namespace Notas.UI;

internal sealed partial class ChangePreview
{
    /// <summary>"Linhas 17–19"; remoção pura mostra onde as linhas estavam.</summary>
    private string Title => _hunk.AfterCount switch
    {
        0 => $"Linha {_hunk.AfterStart + 1}",
        1 => $"Linha {_hunk.AfterStart + 1}",
        _ => $"Linhas {_hunk.AfterStart + 1}–{_hunk.AfterStart + _hunk.AfterCount}",
    };

    private string Subtitle => $"não salvas · {Count(_removed.Length, "removida", "removidas")}, {Count(_added.Length, "nova", "novas")}";

    private static string Count(int n, string one, string many) => $"{n} {(n == 1 ? one : many)}";
}
