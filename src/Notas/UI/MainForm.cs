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
            _ = OpenAsync(_initialFile);
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

            await UiIdle.NextAsync();
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
}
