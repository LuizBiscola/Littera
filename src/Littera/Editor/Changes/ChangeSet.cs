namespace Notas.Editor.Changes;

/// <summary>
/// Linhas alteradas, pela comparação de verdade (não pelo histórico de edição): digitar e apagar não deixa marca,
/// e espaços contam. Unsaved = diferente do disco; Saved = já salva, mas diferente de quando o arquivo abriu.
/// </summary>
internal sealed partial record ChangeSet(List<DiffHunk> Unsaved, SortedSet<int> UnsavedLines, SortedSet<int> SavedLines)
{
    /// <summary>null quando há alterações demais para comparar.</summary>
    public static ChangeSet? Compute(string[] original, string[] saved, string[] current)
    {
        if (LineDiff.Hunks(saved, current) is not { } unsaved)
            return null;

        var unsavedLines = new SortedSet<int>();
        foreach (DiffHunk hunk in unsaved)
        {
            for (int line = hunk.AfterStart; line < hunk.AfterStart + Math.Max(1, hunk.AfterCount); line++)
                unsavedLines.Add(Math.Min(line, current.Length - 1)); // só remoção: marca a linha onde o texto sumiu
        }

        var savedLines = new SortedSet<int>();
        if (!ReferenceEquals(original, saved) && LineDiff.Hunks(original, saved) is { } history)
        {
            foreach (DiffHunk hunk in history)
            {
                for (int line = hunk.AfterStart; line < hunk.AfterStart + hunk.AfterCount; line++)
                {
                    if (ToCurrent(line, unsaved) is int now && !unsavedLines.Contains(now))
                        savedLines.Add(now);
                }
            }
        }

        return new ChangeSet(unsaved, unsavedLines, savedLines);
    }
}
