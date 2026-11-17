using System.Runtime.InteropServices;

namespace Littera.Services;

/// <summary>Converte atalhos do settings.json ("Ctrl+Shift+F", "F11", "Ctrl+Plus") em teclas do WinForms.</summary>
internal static class KeyText
{
    private static readonly Dictionary<string, Keys> Names = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Plus"] = Keys.Oemplus, ["="] = Keys.Oemplus, ["Minus"] = Keys.OemMinus, ["-"] = Keys.OemMinus,
        ["Comma"] = Keys.Oemcomma, ["Period"] = Keys.OemPeriod, ["Esc"] = Keys.Escape, ["Del"] = Keys.Delete,
        ["Enter"] = Keys.Enter, ["Tab"] = Keys.Tab, ["Space"] = Keys.Space,
    };

    /// <summary>Vários atalhos para o mesmo comando são separados por ";" (ex.: "Ctrl+Plus; Ctrl+Add").</summary>
    public static IEnumerable<Keys> ParseAll(string? text) =>
        (text ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Parse).OfType<Keys>();

    public static Keys? Parse(string text)
    {
        Keys result = Keys.None;
        if (text.EndsWith("++", StringComparison.Ordinal))
            text = text[..^1] + "Plus"; // "Ctrl++"
        string[] parts = text.Split('+', StringSplitOptions.TrimEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i];
            Keys? key = part.ToLowerInvariant() switch
            {
                "ctrl" or "control" => Keys.Control,
                "shift" => Keys.Shift,
                "alt" => Keys.Alt,
                "" => null,
                _ when part.Length == 1 && char.IsDigit(part[0]) => Keys.D0 + (part[0] - '0'),
                _ when Names.TryGetValue(part, out Keys named) => named,
                _ when Enum.TryParse(part, ignoreCase: true, out Keys parsed) => parsed,
                _ when part.Length == 1 => LayoutKey(part[0]),
                _ => null,
            };
            if (key is null)
                return null;
            result |= key.Value;
        }

        return (result & Keys.KeyCode) == Keys.None ? null : result; // só modificadores não é atalho
    }

    /// <summary>Tecla que digita o caractere no layout atual: "[" e "]" ficam em teclas diferentes no ABNT2 e no americano.</summary>
    private static Keys? LayoutKey(char c)
    {
        short scan = VkKeyScanW(c);
        return scan == -1 ? null : (Keys)(scan & 0xFF);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern short VkKeyScanW(char c);
}
