using System.Diagnostics;

namespace Notas.Services;

/// <summary>
/// Mede o maior intervalo em que a thread de UI ficou sem processar mensagens.
/// O timer do Windows tem resolução de ~15,6 ms, então esse é o piso da medida.
/// </summary>
internal sealed class UiStallMonitor : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 10 };
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private double _last;

    public UiStallMonitor()
    {
        _timer.Tick += OnTick;
        _timer.Start();
    }

    public double MaxGapMs { get; private set; }

    private void OnTick(object? sender, EventArgs e)
    {
        double now = _clock.Elapsed.TotalMilliseconds;
        MaxGapMs = Math.Max(MaxGapMs, now - _last);
        _last = now;
    }

    public void Dispose()
    {
        OnTick(null, EventArgs.Empty);
        _timer.Dispose();
    }
}
