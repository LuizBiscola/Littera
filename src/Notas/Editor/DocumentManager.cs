using Notas.Settings;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>
/// Documentos abertos e qual está ativo. Cada documento tem seu próprio editor, escondido quando inativo:
/// trocar de aba não reconstrói nada, mesmo com arquivos enormes.
/// </summary>
internal sealed partial class DocumentManager(Control host, EditorSettings settings)
{
    private readonly List<OpenDocument> _documents = [];

    public IReadOnlyList<OpenDocument> Documents => _documents;

    public OpenDocument? Active { get; private set; }

    /// <summary>Lista, ativo, carga ou estado "não salvo" mudou.</summary>
    public event Action? Changed;

    /// <summary>O editor visível trocou: quem escuta eventos do editor precisa religar.</summary>
    public event Action? ActiveChanged;

    public void Activate(OpenDocument document)
    {
        if (document == Active)
            return;

        Scintilla editor = EnsureEditor(document);
        editor.Visible = true;
        editor.BringToFront();
        if (Active?.Editor is { } previous)
            previous.Visible = false;

        Active = document;
        editor.Focus();
        ActiveChanged?.Invoke();
        Changed?.Invoke();
    }

    private Scintilla EnsureEditor(OpenDocument document)
    {
        if (document.Editor is not null)
            return document.Editor;

        Scintilla editor = EditorFactory.Create(settings);
        editor.SavePointLeft += (_, _) => Changed?.Invoke();
        editor.SavePointReached += (_, _) => Changed?.Invoke();
        host.Controls.Add(editor);
        document.Editor = editor;
        return editor;
    }
}
