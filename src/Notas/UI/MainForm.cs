using System.Diagnostics;
using Notas.Editor;
using Notas.Services;
using Notas.Settings;
using Notas.Theme;
using Notas.UI.Chrome;

namespace Notas.UI;

internal sealed class MainForm : ChromeForm
{
    private readonly DocumentManager _documents;

    public MainForm(string[] args)
    {
        string? initialFile = args.FirstOrDefault(a => !a.StartsWith("--", StringComparison.Ordinal));

        Text = "Notas";
        // Sem auto-escala do WinForms: cada controle já mede em Px() pelo DPI do monitor.
        AutoScaleMode = AutoScaleMode.None;
        ClientSize = new Size(this.Px(1200), this.Px(760));
        MinimumSize = new Size(this.Px(560), this.Px(360));
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Palette.Background;

        var editorHost = new Panel { Dock = DockStyle.Fill, BackColor = Palette.Background };
        Controls.Add(editorHost);
        _documents = new DocumentManager(editorHost, new EditorSettings());
        _documents.Changed += () => Text = $"{_documents.Active?.Title} — Notas";
        _documents.Loaded += document => EditorSetup.ApplySql(document.Editor!); // até existir o registro de linguagens
        _documents.LoadFailed += (document, error) =>
        {
            MessageBox.Show(this, error.Message, "Notas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _documents.Close(document);
        };

        TitleBar.AddLeft(Icons.PanelLeft, "Mostrar ou ocultar abas (Ctrl+B)", () => { }).Toggled = true;
        TitleBar.AddAction(Icons.Search, "Buscar (Ctrl+F)", () => { });
        TitleBar.AddAction(Icons.Settings, "Configurações", () => { });

        if (initialFile is null)
            _documents.New();
        else
            OpenMeasured(initialFile);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        StartupMetrics.Mark("t_shown");
        Application.Idle += OnFirstIdle;
    }

    private void OnFirstIdle(object? sender, EventArgs e)
    {
        Application.Idle -= OnFirstIdle;
        StartupMetrics.Write("ready_ms", StartupMetrics.SinceProcessStart());

        // A trava só conta depois da janela pronta: antes disso o usuário ainda nem pode interagir.
        if (StartupMetrics.Enabled && _documents.Active?.IsLoading == true)
            _loadStalls = new UiStallMonitor();
    }

    private UiStallMonitor? _loadStalls;

    private void OpenMeasured(string path)
    {
        var stopwatch = Stopwatch.StartNew();
        bool measured = !StartupMetrics.Enabled;
        _documents.Loaded += async _ =>
        {
            if (measured)
                return;
            measured = true;
            UiStallMonitor? stalls = _loadStalls;
            _loadStalls = null;
            await UiIdle.NextAsync();
            StartupMetrics.Write("load_ms", stopwatch.Elapsed.TotalMilliseconds);
            StartupMetrics.Write("load_max_ui_stall_ms", stalls?.MaxGapMs ?? 0);
            stalls?.Dispose();
        };
        _documents.Open(path);
    }
}
