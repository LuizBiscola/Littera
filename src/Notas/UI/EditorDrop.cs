using System.Runtime.InteropServices;
using ScintillaNET;

namespace Notas.UI;

/// <summary>
/// O Scintilla registra seu próprio alvo de arrastar, que só aceita texto. Trocamos pelo do WinForms
/// para aceitar arquivos, e o texto solto é inserido no ponto onde caiu.
/// </summary>
internal static class EditorDrop
{
    public static void Attach(Scintilla editor, Action<string> openFile)
    {
        _ = RevokeDragDrop(editor.Handle);
        FileDrop.Attach(editor, openFile);
        editor.DragDrop += (_, e) =>
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true || e.Data?.GetData(DataFormats.UnicodeText) is not string text)
                return;

            Point client = editor.PointToClient(new Point(e.X, e.Y));
            int position = editor.CharPositionFromPoint(client.X, client.Y);
            if (position >= editor.SelectionStart && position <= editor.SelectionEnd && editor.SelectedText == text)
            {
                e.Effect = DragDropEffects.None; // soltou o trecho em cima dele mesmo
                return;
            }

            // A seleção original fica intacta: num "mover", o próprio Scintilla apaga a origem ao final.
            editor.InsertText(position, text);
        };
    }

    [DllImport("ole32.dll")]
    private static extern int RevokeDragDrop(IntPtr hwnd);
}
