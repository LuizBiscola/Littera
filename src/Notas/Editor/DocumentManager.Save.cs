using Notas.Services;
using ScintillaNET;

namespace Notas.Editor;

internal sealed partial class DocumentManager
{
    /// <summary>Salvo (talvez com outro nome): a janela atualiza recentes e linguagem.</summary>
    public event Action<OpenDocument>? Saved;

    /// <summary>Salva no caminho do documento ou em <paramref name="path"/> ("Salvar como").</summary>
    public async Task SaveAsync(OpenDocument document, string? path = null)
    {
        string target = Path.GetFullPath(path ?? document.Path ?? throw new InvalidOperationException("Documento sem caminho."));
        Scintilla editor = document.Editor!;
        editor.ReadOnly = true; // o texto não pode mudar entre a cópia e o ponto de salvamento
        try
        {
            await TextFileWriter.SaveAsync(target, editor.Text, document.Format.Encoding);
            document.Path = target;
            document.FormatChanged = false;
            editor.SetSavePoint();
        }
        finally
        {
            editor.ReadOnly = false;
        }

        Saved?.Invoke(document);
        Changed?.Invoke();
    }

    /// <summary>Converte as quebras do texto inteiro (dá para desfazer) e usa a nova nas linhas digitadas.</summary>
    public void SetLineEnding(OpenDocument document, LineEnding ending)
    {
        document.Editor!.ConvertEols(ending.ToScintilla());
        document.Editor.EolMode = ending.ToScintilla();
        document.Format = document.Format with { LineEnding = ending };
        Changed?.Invoke();
    }

    /// <summary>O texto não muda: o encoding vale a partir do próximo salvamento.</summary>
    public void SetEncoding(OpenDocument document, TextEncodingKind encoding)
    {
        if (document.Format.Encoding == encoding)
            return;
        document.Format = document.Format with { Encoding = encoding };
        document.FormatChanged = true;
        Changed?.Invoke();
    }
}
