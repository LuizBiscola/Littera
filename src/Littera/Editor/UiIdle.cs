namespace Notas.Editor;

/// <summary>
/// Espera a fila de mensagens esvaziar (pintura, teclado e mouse já processados).
/// Melhor que Task.Delay(1), que custa ~15 ms de timer, e que Task.Yield, que passa na frente da entrada.
/// </summary>
internal static class UiIdle
{
    public static Task NextAsync()
    {
        var idle = new TaskCompletionSource();
        void OnIdle(object? sender, EventArgs e)
        {
            Application.Idle -= OnIdle;
            idle.TrySetResult();
        }

        Application.Idle += OnIdle;
        return idle.Task;
    }
}
