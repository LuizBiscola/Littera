namespace Notas.UI;

/// <summary>Soltar arquivos do Explorer em qualquer parte da janela abre cada um numa aba.</summary>
internal static class FileDrop
{
    public static void Attach(Control target, Action<string> openFile)
    {
        target.AllowDrop = true;
        target.DragEnter += (_, e) => e.Effect = Effect(e);
        target.DragOver += (_, e) => e.Effect = Effect(e);
        target.DragDrop += (_, e) =>
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files)
                return;
            foreach (string file in files.Where(File.Exists))
                openFile(file);
        };
    }

    /// <summary>Arquivos: copiar. Texto (só no editor): mover, ou copiar com Ctrl, como no Windows.</summary>
    public static DragDropEffects Effect(DragEventArgs e)
    {
        if (e.Data is null)
            return DragDropEffects.None;
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            return DragDropEffects.Copy;
        if (!e.Data.GetDataPresent(DataFormats.UnicodeText))
            return DragDropEffects.None;

        bool copy = (e.KeyState & 8) != 0 || !e.AllowedEffect.HasFlag(DragDropEffects.Move); // 8 = Ctrl
        return copy ? DragDropEffects.Copy : DragDropEffects.Move;
    }
}
