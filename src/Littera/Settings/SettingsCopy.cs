using System.Text.Json;

namespace Littera.Settings;

/// <summary>
/// Rascunho da janela de configurações: edita-se uma cópia e, ao salvar, os valores voltam para os objetos
/// originais (que editores e gerenciador de documentos já referenciam, por isso não são trocados).
/// </summary>
internal static class SettingsCopy
{
    public static AppSettings Clone(AppSettings settings) =>
        JsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(settings, LitteraJsonContext.Default.AppSettings),
            LitteraJsonContext.Default.AppSettings)!;

    /// <summary>Copia as propriedades simples (uma seção como EditorSettings) de um objeto para outro.</summary>
    public static void Into<T>(T source, T target) where T : class
    {
        foreach (var property in typeof(T).GetProperties())
        {
            if (property.CanWrite && property.CanRead)
                property.SetValue(target, property.GetValue(source));
        }
    }
}
