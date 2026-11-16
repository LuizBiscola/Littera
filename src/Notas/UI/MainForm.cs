using System.Diagnostics;
using Notas.Editor;
using Notas.Services;
using Notas.Settings;
using Notas.Theme;
using Notas.UI.Chrome;

namespace Notas.UI;

internal sealed partial class MainForm : ChromeForm
{
    private readonly AppSettings _settings;
    private readonly DocumentManager _documents;
    private readonly StatusBar _status = new();
    private readonly OpenFilesPanel _openFiles;
    private readonly ChromeButton _panelButton;
    private readonly ChromeButton _folderButton;
    private readonly Panel _editorHost;

    public MainForm(AppSettings settings, string[] args, Task<SessionState>? session = null)
    {
        StartupMetrics.Mark("f_base");
        SuspendLayout(); // um layout só no fim, em vez de um a cada controle adicionado
        _settings = settings;
        string? initialFile = args.FirstOrDefault(a => !a.StartsWith("--", StringComparison.Ordinal));

        Text = "Notas";
        // Sem auto-escala do WinForms: cada controle já mede em Px() pelo DPI do monitor.
        AutoScaleMode = AutoScaleMode.None;
        ClientSize = new Size(this.Px(1200), this.Px(760));
        MinimumSize = new Size(this.Px(560), this.Px(360));
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Palette.Background;

        _editorHost = new Panel { Dock = DockStyle.Fill, BackColor = Palette.Background };
        Controls.Add(_editorHost);
        Controls.Add(_status);
        _status.SegmentClicked += OnStatusSegmentClicked;
        _status.ExpandAllClicked += () => SetAllFolds(expand: true);
        _status.ChangesClicked += () => GoToChange(+1);
        _highlightTimer.Tick += (_, _) => RefreshHighlights();
        _draftTimer.Tick += (_, _) => SaveDrafts(AutosaveLimit);
        _documents = new DocumentManager(_editorHost, settings.Editor) { NewFileFormat = NewFileFormat(settings.Files) };
        _documents.Changed += () => Text = $"{_documents.Active?.Title} — Notas";
        _documents.Changed += UpdateStatus;
        _documents.ActiveChanged += BindActiveEditor;
        _documents.Saved += OnDocumentSaved;
        _documents.Loaded += CaptureBaseline; // referência das barras de alteração
        _documents.LoadFailed += (document, error) =>
        {
            if (document.Pinned)
            {
                document.Missing = document.IsLoaded = true; // fixada fica, vazia e riscada, até o usuário desafixar
                return;
            }

            MessageBox.Show(this, error.Message, "Notas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _documents.Close(document);
        };

        _openFiles = new OpenFilesPanel(_documents) { Visible = settings.Window.ShowOpenFiles };
        _openFiles.NewRequested += () => _documents.New();
        _openFiles.CloseRequested += document => _ = RequestCloseAsync(document);
        Controls.Add(_openFiles);
        Controls.SetChildIndex(_openFiles, 1); // encaixa depois da barra de status: ela ocupa a largura toda
        StartupMetrics.Mark("f_panels");

        _panelButton = TitleBar.AddLeft(Icons.PanelLeft, "Mostrar ou ocultar abas (Ctrl+B)", ToggleOpenFiles);
        _panelButton.Toggled = settings.Window.ShowOpenFiles; // Visible é falso até a janela aparecer
        TitleBar.AddAction(Icons.Search, "Buscar (Ctrl+F)", () => FindPanel.Open(replace: false));
        _folderButton = TitleBar.AddAction(Icons.PanelRight, "Painel de pasta (Ctrl+Shift+E)", ToggleFolderPanel);
        TitleBar.AddAction(Icons.Settings, "Configurações (Ctrl+,)", ShowSettings);

        TitleBar.MenuRequested += ShowAppMenu;
        _documents.EditorCreated += editor =>
        {
            editor.Insert += (_, _) => ScheduleDrafts();
            editor.Delete += (_, _) => ScheduleDrafts();
            editor.MarginClick += (_, e) => OnFoldMarginClick(editor, e);
            AttachFoldPreview(editor);
            AttachChangePreview(editor);
        };

        StartupMetrics.Mark("f_drop");
        RestoreWindowPlacement();
        OpenDocument? restored = RestoreSession(session?.Result ?? SettingsStore.LoadSession());
        if (initialFile is not null)
            OpenMeasured(initialFile);
        else if (restored is not null)
            _documents.Activate(restored);
        else
            _documents.New();
        ResumeLayout(performLayout: true);
        StartupMetrics.Mark("f_open");
    }

    private void ToggleOpenFiles()
    {
        bool show = !_settings.Window.ShowOpenFiles;
        _settings.Window.ShowOpenFiles = show;
        _openFiles.Visible = show;
        _panelButton.Toggled = show;
        TitleBar.Invalidate();
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
        EnableFileDrop(); // registrar alvos OLE custa; ninguém arrasta um arquivo nos primeiros milissegundos
        _ = Shortcuts; // monta os atalhos (e completa o settings.json) fora do caminho da abertura

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
