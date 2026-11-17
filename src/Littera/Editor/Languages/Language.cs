using ScintillaNET;

namespace Notas.Editor.Languages;

/// <summary>Uma linguagem com realce: lexer da Lexilla, palavras-chave e cores dos tokens.</summary>
internal abstract class Language
{
    /// <summary>Nome na barra de status e no seletor.</summary>
    public abstract string Name { get; }

    /// <summary>Nome do lexer na Lexilla; null = texto simples, sem realce.</summary>
    public abstract string? Lexer { get; }

    /// <summary>Extensões com ponto, em minúsculas.</summary>
    public abstract string[] Extensions { get; }

    /// <summary>A Lexilla calcula os trechos recolhíveis (chaves, tags, recuo); SQL e texto o Notas calcula.</summary>
    public virtual bool LexerFolds => false;

    /// <summary>Prefixo de comentário de linha (Ctrl+/); null = a linguagem não tem.</summary>
    public virtual string? LineComment => null;

    /// <summary>O lexer é do documento e o StyleClearAll zera as cores: reaplicar ao carregar e ao mudar configurações.</summary>
    public void Apply(Scintilla editor)
    {
        editor.LexerName = Lexer ?? "";
        editor.SetProperty("fold", LexerFolds ? "1" : "0");
        editor.SetProperty("fold.compact", "0");
        Configure(editor);
    }

    protected abstract void Configure(Scintilla editor);

    protected static void Paint(Scintilla editor, Color color, params int[] styles)
    {
        foreach (int style in styles)
            editor.Styles[style].ForeColor = color;
    }

    protected static void PaintItalic(Scintilla editor, Color color, params int[] styles)
    {
        foreach (int style in styles)
        {
            editor.Styles[style].ForeColor = color;
            editor.Styles[style].Italic = true;
        }
    }
}

internal sealed class PlainTextLanguage : Language
{
    public override string Name => "Texto simples";

    public override string? Lexer => null;

    public override string[] Extensions => [".txt", ".log", ".csv"];

    protected override void Configure(Scintilla editor)
    {
    }
}
