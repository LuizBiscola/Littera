using System.Diagnostics;
using Littera.Editor;
using Littera.Theme;

namespace Littera.UI;

internal sealed partial class OpenFilesPanel
{
    private const int FlashMs = 1200;
    private const int FadeMs = 400;

    private readonly System.Windows.Forms.Timer _flashTimer = new() { Interval = 30 };
    private readonly Stopwatch _flashClock = new();
    private OpenDocument? _flashing;

    /// <summary>Confirmação ao fixar: fundo azul por 1,2 s que some suavemente; a seção rola até a aba.</summary>
    private void Flash(OpenDocument document)
    {
        _flashing = document;
        _flashClock.Restart();
        _pinnedScroll = int.MaxValue; // a aba entrou no fim das fixadas
        ClampScroll();
        _flashTimer.Start();
        Invalidate();
    }

    private void PaintFlash(Graphics g, OpenDocument document, Rectangle item)
    {
        long elapsed = _flashClock.ElapsedMilliseconds;
        if (document != _flashing || elapsed >= FlashMs)
            return;
        int alpha = elapsed < FlashMs - FadeMs ? 255 : (int)(255 * (FlashMs - elapsed) / (double)FadeMs);
        Shapes.FillRounded(g, Color.FromArgb(alpha, Palette.PinFlash), item, this.Px(6));
    }

    private void OnFlashTick()
    {
        if (_flashClock.ElapsedMilliseconds >= FlashMs)
        {
            _flashTimer.Stop();
            _flashing = null;
        }

        Invalidate();
    }
}
