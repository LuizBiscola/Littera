using Notas.Editor.Highlights;
using Notas.Settings;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>Cria um editor já configurado (um por documento aberto).</summary>
internal static class EditorFactory
{
    public static Scintilla Create(EditorSettings settings)
    {
        var editor = new Scintilla
        {
            Dock = DockStyle.Fill,
            BorderStyle = ScintillaNET.BorderStyle.None,
            Visible = false,
        };

        // Múltiplos cursores (Ctrl+clique) e seleção em coluna (Alt+arrastar) são nativos do Scintilla.
        editor.MultipleSelection = true;
        editor.AdditionalSelectionTyping = true;
        editor.MouseSelectionRectangularSwitch = true;
        editor.MultiPaste = MultiPaste.Each;
        editor.VirtualSpaceOptions = VirtualSpace.RectangularSelection;
        editor.ScrollWidth = 1;
        editor.ScrollWidthTracking = true; // a barra horizontal acompanha a linha mais longa já vista
        editor.TabIndents = true;
        editor.BackspaceUnindents = true;

        ApplySettings(editor, settings);
        AutoIndent.Attach(editor);
        Folding.EditorFolding.Setup(editor);
        BlockHighlighter.Setup(editor);
        OccurrenceHighlighter.Setup(editor);
        Search.FindSession.Setup(editor);
        editor.Insert += (_, _) => EditorMargins.UpdateNumberWidth(editor, settings.ShowLineNumbers);
        editor.Delete += (_, _) => EditorMargins.UpdateNumberWidth(editor, settings.ShowLineNumbers);
        return editor;
    }

    /// <summary>Tudo que depende das configurações; chamado de novo quando elas mudam.</summary>
    public static void ApplySettings(Scintilla editor, EditorSettings settings)
    {
        editor.Technology = settings.Renderer == "directwrite" ? Technology.DirectWrite : Technology.Default;
        EditorStyle.Apply(editor, settings);
        EditorMargins.Apply(editor, settings);
        editor.IndentWidth = settings.IndentSize;
        editor.TabWidth = settings.IndentSize;
        editor.UseTabs = settings.UseTabs;
        editor.IndentationGuides = settings.ShowIndentGuides ? IndentView.LookBoth : IndentView.None;
        editor.WrapMode = settings.WordWrap ? WrapMode.Word : WrapMode.None;
    }
}
