using System.Runtime.InteropServices;
using Notas.Theme;

namespace Notas.UI.Chrome;

/// <summary>
/// Janela sem a barra de título do Windows, mantendo o que ela fazia: bordas de redimensionar,
/// arrastar, duplo clique, Aero Snap, Snap Layouts (Windows 11), sombra e cantos arredondados.
/// </summary>
internal partial class ChromeForm : Form
{
    public ChromeForm()
    {
        TitleBar = new TitleBar();
        Controls.Add(TitleBar);
    }

    protected TitleBar TitleBar { get; }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Win32.SetBorderColor(Handle, Palette.Border);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        TitleBar.Caption = Text;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        TitleBar.SetMaximized(WindowState == FormWindowState.Maximized);
    }

    /// <summary>
    /// O Windows calcula as bordas padrão (laterais e inferior continuam invisíveis e redimensionáveis);
    /// devolvemos o topo original para tirar só a barra de título.
    /// </summary>
    private void RemoveCaption(ref Message m)
    {
        int originalTop = Marshal.ReadInt32(m.LParam, 4); // rgrc[0].top em NCCALCSIZE_PARAMS
        base.WndProc(ref m);
        int top = originalTop;
        if (Win32.IsZoomed(Handle))
            top += Win32.ResizeFrame(DeviceDpi); // maximizada, a janela passa da tela por essa espessura
        Marshal.WriteInt32(m.LParam, 4, top);
        m.Result = IntPtr.Zero;
    }
}
