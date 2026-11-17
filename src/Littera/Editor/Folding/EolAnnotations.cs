using System.Runtime.InteropServices;
using System.Text;
using ScintillaNET;

namespace Notas.Editor.Folding;

/// <summary>
/// Anotações de fim de linha do Scintilla 5 (o Scintilla5.NET não as expõe): texto com estilo próprio
/// desenhado depois do fim da linha. É onde aparece o resumo de um trecho recolhido.
/// </summary>
internal static class EolAnnotations
{
    private const int SciSetText = 2740;
    private const int SciSetStyle = 2742;
    private const int SciClearAll = 2744;
    private const int SciSetVisible = 2745;

    public static void Set(Scintilla editor, int line, string text, int style)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text + "\0");
        GCHandle pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            editor.DirectMessage(SciSetText, line, pin.AddrOfPinnedObject());
        }
        finally
        {
            pin.Free();
        }

        editor.DirectMessage(SciSetStyle, line, style);
    }

    public static void ClearAll(Scintilla editor) => editor.DirectMessage(SciClearAll);

    public static void Show(Scintilla editor, EOLAnnotationVisible visible) => editor.DirectMessage(SciSetVisible, (int)visible);
}
