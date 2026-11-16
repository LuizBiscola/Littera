using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Notas.Settings;

/// <summary>Lê e grava settings.json e session.json em %APPDATA%\Notas.</summary>
internal static class SettingsStore
{
    /// <summary>NOTAS_HOME troca a pasta (medições e testes rodam sem tocar na sessão real do usuário).</summary>
    public static string Folder { get; } = Environment.GetEnvironmentVariable("NOTAS_HOME") is { Length: > 0 } home
        ? home
        : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Notas");

    public static string SettingsPath => Path.Combine(Folder, "settings.json");

    /// <summary>Releitura depois de editar o arquivo à mão: com erro, nada muda e a mensagem diz onde.</summary>
    public static AppSettings? TryReloadSettings(out string? error)
    {
        try
        {
            error = null;
            return JsonSerializer.Deserialize(File.ReadAllText(SettingsPath), NotasJsonContext.Default.AppSettings);
        }
        catch (JsonException ex)
        {
            error = ex.Message;
            return null;
        }
    }

    public static AppSettings LoadSettings() =>
        Load("settings.json", NotasJsonContext.Default.AppSettings) ?? new AppSettings();

    public static void SaveSettings(AppSettings settings) =>
        Save("settings.json", settings, NotasJsonContext.Default.AppSettings);

    public static SessionState LoadSession() =>
        Load("session.json", NotasJsonContext.Default.SessionState) ?? new SessionState();

    public static void SaveSession(SessionState session) =>
        Save("session.json", session, NotasJsonContext.Default.SessionState);

    private static T? Load<T>(string fileName, JsonTypeInfo<T> type) where T : class
    {
        string path = Path.Combine(Folder, fileName);
        try
        {
            using FileStream stream = File.OpenRead(path);
            return JsonSerializer.Deserialize(stream, type);
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            return null;
        }
        catch (JsonException)
        {
            // Arquivo editado à mão com erro: guarda uma cópia e segue com o padrão.
            File.Copy(path, path + ".invalido", overwrite: true);
            return null;
        }
    }

    private static void Save<T>(string fileName, T value, JsonTypeInfo<T> type)
    {
        Directory.CreateDirectory(Folder);
        string path = Path.Combine(Folder, fileName);
        string temp = path + ".tmp";
        using (FileStream stream = File.Create(temp))
            JsonSerializer.Serialize(stream, value, type);

        File.Move(temp, path, overwrite: true); // troca atômica: nunca deixa o arquivo pela metade
    }
}
