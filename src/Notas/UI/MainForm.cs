using System.Diagnostics;
using System.Text;
using Notas.Editor;
using Notas.Services;
using Notas.Theme;
using Notas.UI.Chrome;
using ScintillaNET;

namespace Notas.UI;

internal sealed class MainForm : ChromeForm
{
    private readonly Scintilla _editor;
    private readonly string? _initialFile;

    public MainForm(string[] args)
    {
        _initialFile = args.FirstOrDefault(a => !a.StartsWith("--", StringComparison.Ordinal));

        // GDI é o padrão: DirectWrite custou +90 ms e +40 MB na Fase 0.
        // Chave temporária até existir a opção no settings.json (Fase 1).
        Technology technology = Environment.GetEnvironmentVariable("NOTAS_TECH") == "directwrite"
            ? Technology.DirectWrite
            : Technology.Default;

        Text = "Notas";
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96f, 96f);
        ClientSize = new Size(1200, 760);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;

        StartupMetrics.Mark("t_before_editor");
        _editor = new Scintilla { Dock = DockStyle.Fill };
        StartupMetrics.Mark("t_editor_new");
        EditorSetup.ConfigureView(_editor, technology);
        EditorSetup.ApplySql(_editor);
        StartupMetrics.Mark("t_editor_setup");
        Controls.Add(_editor);

        TitleBar.AddLeft(Icons.PanelLeft, "Mostrar ou ocultar abas (Ctrl+B)", () => { }).Toggled = true;
        TitleBar.AddAction(Icons.Search, "Buscar (Ctrl+F)", () => { });
        TitleBar.AddAction(Icons.Settings, "Configurações", () => { });
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        StartupMetrics.Mark("t_shown");
        Application.Idle += OnFirstIdle;

        if (_initialFile is not null)
        {
            // Experimento da Fase 0 (não adotado): carga progressiva em blocos.
            if (Environment.GetEnvironmentVariable("NOTAS_LOAD") == "chunks")
                _ = OpenInChunksAsync(_initialFile);
            else
                _ = OpenAsync(_initialFile);
        }
    }

    private async Task OpenInChunksAsync(string path)
    {
        var stopwatch = Stopwatch.StartNew();
        using UiStallMonitor? stalls = StartupMetrics.Enabled ? new UiStallMonitor() : null;

        var chunks = System.Threading.Channels.Channel.CreateBounded<string>(2);
        Task producer = Task.Run(async () =>
        {
            var options = new FileStreamOptions { BufferSize = 1 << 16, Options = FileOptions.SequentialScan };
            using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, options);
            char[] buffer = new char[1 << 19];
            string carry = "";
            int read;
            while ((read = reader.ReadBlock(buffer, 0, buffer.Length)) > 0)
            {
                // Não corta par substituto nem CRLF entre dois blocos.
                int keep = read;
                if (read == buffer.Length && (char.IsHighSurrogate(buffer[read - 1]) || buffer[read - 1] == '\r'))
                    keep--;

                await chunks.Writer.WriteAsync(carry + new string(buffer, 0, keep));
                carry = keep < read ? buffer[read - 1].ToString() : "";
            }

            if (carry.Length > 0)
                await chunks.Writer.WriteAsync(carry);
            chunks.Writer.Complete();
        });

        const int SCI_SETUNDOCOLLECTION = 2012;
        _editor.DirectMessage(SCI_SETUNDOCOLLECTION, IntPtr.Zero);
        _editor.ClearAll();
        await foreach (string chunk in chunks.Reader.ReadAllAsync())
        {
            _editor.AppendText(chunk);
            await Task.Delay(1); // devolve a vez para a fila de mensagens (pintura, teclado, mouse)
        }

        await producer;
        _editor.DirectMessage(SCI_SETUNDOCOLLECTION, new IntPtr(1));
        _editor.EmptyUndoBuffer();
        Text = Path.GetFileName(path) + " — Notas";

        await WaitForIdleAsync();
        StartupMetrics.Write("load_ms", stopwatch.Elapsed.TotalMilliseconds);
        if (stalls is not null)
            StartupMetrics.Write("load_max_ui_stall_ms", stalls.MaxGapMs);
    }

    private void OnFirstIdle(object? sender, EventArgs e)
    {
        Application.Idle -= OnFirstIdle;
        StartupMetrics.Write("ready_ms", StartupMetrics.SinceProcessStart());
    }

    private async Task OpenAsync(string path)
    {
        var stopwatch = Stopwatch.StartNew();
        using UiStallMonitor? stalls = StartupMetrics.Enabled ? new UiStallMonitor() : null;

        try
        {
            long length = new FileInfo(path).Length;
            ILoader loader = _editor.CreateLoader((int)Math.Min(length, int.MaxValue));
            Document document = await Task.Run(() => ReadInto(loader, path));
            StartupMetrics.Write("f_read_ms", stopwatch.Elapsed.TotalMilliseconds);

            _editor.Document = document;
            StartupMetrics.Write("f_setdoc_ms", stopwatch.Elapsed.TotalMilliseconds);
            _editor.ReleaseDocument(document); // O controle passa a ser o único dono.
            EditorSetup.ApplySql(_editor);
            StartupMetrics.Write("f_lexer_ms", stopwatch.Elapsed.TotalMilliseconds);
            Text = Path.GetFileName(path) + " — Notas";

            await WaitForIdleAsync();
            StartupMetrics.Write("load_ms", stopwatch.Elapsed.TotalMilliseconds);
            if (stalls is not null)
                StartupMetrics.Write("load_max_ui_stall_ms", stalls.MaxGapMs);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, ex.Message, "Notas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static Document ReadInto(ILoader loader, string path)
    {
        var options = new FileStreamOptions { BufferSize = 1 << 16, Options = FileOptions.SequentialScan };
        using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, options);

        // Scintilla.NET 7.1.1 ignora o parâmetro length de AddData e envia o array inteiro
        // (o resto vira NUL no documento). Por isso: blocos sempre cheios (ReadBlock) e,
        // no último bloco parcial, um array do tamanho exato.
        char[] buffer = new char[1 << 16];
        int read;
        while ((read = reader.ReadBlock(buffer, 0, buffer.Length)) > 0)
        {
            char[] chunk = read == buffer.Length ? buffer : buffer[..read];
            if (!loader.AddData(chunk, read))
            {
                loader.Release();
                throw new IOException("Memória insuficiente para carregar o arquivo.");
            }
        }

        return loader.ConvertToDocument();
    }

    private static Task WaitForIdleAsync()
    {
        var done = new TaskCompletionSource();
        void OnIdle(object? sender, EventArgs e)
        {
            Application.Idle -= OnIdle;
            done.SetResult();
        }

        Application.Idle += OnIdle;
        return done.Task;
    }
}
