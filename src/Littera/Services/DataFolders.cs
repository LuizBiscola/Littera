namespace Littera.Services;

/// <summary>
/// Pastas de dados: %APPDATA%\Littera (configurações, sessão, rascunhos) e %LOCALAPPDATA%\Littera (DLLs nativas,
/// erros.log). Antes da troca de nome eram ...\Notas: na primeira abertura como Littera a pasta antiga é renomeada.
/// </summary>
internal static class DataFolders
{
    private const string OldName = "Notas";

    private static string? _roaming;
    private static string? _local;

    // Calculadas no primeiro uso (não na carga da classe): os testes usam Prepare sem mexer nas pastas reais.
    public static string Roaming => _roaming ??= Prepare(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

    public static string Local => _local ??= Prepare(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

    /// <summary>Se a pasta antiga está em uso (o Notas antigo aberto), continua nela desta vez e tenta de novo depois.</summary>
    internal static string Prepare(string parent)
    {
        string current = Path.Combine(parent, Product.Name);
        string old = Path.Combine(parent, OldName);
        if (Directory.Exists(current) || !Directory.Exists(old))
            return current;
        try
        {
            Directory.Move(old, current);
            return current;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return old;
        }
    }
}
