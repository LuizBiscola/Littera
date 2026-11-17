using System.Text.Json;
using System.Text.Json.Serialization;

namespace Notas.Settings;

/// <summary>Serialização gerada em tempo de compilação: sem reflexão na inicialização.</summary>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true)]
[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(SessionState))]
internal sealed partial class NotasJsonContext : JsonSerializerContext;
