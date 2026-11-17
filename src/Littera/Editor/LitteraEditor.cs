using ScintillaNET;

namespace Littera.Editor;

/// <summary>O editor do Scintilla com um acréscimo: cursor de mão onde há algo clicável na calha (barras de alteração).</summary>
internal sealed class LitteraEditor : Scintilla
{
    private const int WM_SETCURSOR = 0x0020;

    /// <summary>Ponto (em coordenadas do editor) onde o cursor vira mão; o Scintilla não oferece mão nas margens.</summary>
    public Func<Point, bool>? HandCursorAt { get; set; }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_SETCURSOR && HandCursorAt?.Invoke(PointToClient(Cursor.Position)) == true)
        {
            Cursor.Current = Cursors.Hand;
            m.Result = 1;
            return;
        }

        base.WndProc(ref m);
    }
}
