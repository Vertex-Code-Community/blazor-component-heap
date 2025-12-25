using Bch.Components.WYSIWYG;
using Bch.Components.WYSIWYG.Models;

namespace Bch.Integration.Pages.Pages.WysiwygEditor;

public partial class WysiwygEditorPage
{
    private BchWysiwygEditor? _htmlEditor;

    private readonly List<BchEditorToolbarItemModel> _toolbarKeys = new()
    {
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Bold).ToLower(), Hint = "Bold" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Italic).ToLower(), Hint = "Italic" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Underline).ToLower(), Hint = "Underline" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Undo).ToLower(), Hint = "Undo" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Redo).ToLower(), Hint = "Redo" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.FontSize).ToLower() },
        new BchEditorToolbarItemModel { Key = "headings" },
        new BchEditorToolbarItemModel { Key = "align" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.OrderedList).ToLower(), Hint = "Ordered List" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.UnorderedList).ToLower(), Hint = "Unordered List" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Indent).ToLower(), Hint = "Indent" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Outdent).ToLower(), Hint = "Outdent" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.RemoveLink).ToLower(), Hint = "Remove Link" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.InsertLink).ToLower(), Hint = "Insert Link" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = "mode", Hint = "View Mode" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = "doc-template" }
    };

    private readonly List<string> _templates = new ()
    {
        "CV",
        "Job Application"
    };

    private async Task OnChangeInEditorAsync()
    {
        
    }
    
    private async Task OnViewModeChangedAsync(BchEditorViewMode viewMode)
    {
        
    }

    private async Task OnEditorLoadedAsync()
    {
        
    }

    private async Task OnReplaceImageClickedAsync()
    {
        
    }

    private async Task OnSelectImageAsync()
    {
        
    }

    private async Task OnSelectTemplateAsync(string? templateName)
    {
        if (_htmlEditor is null || string.IsNullOrEmpty(templateName)) return;
        
        var templateHtml = templateName switch
        {
            "CV" => HtmlTemplates.Template1,
            "Job Application" => HtmlTemplates.Template2,
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(templateHtml)) await _htmlEditor.SetValueAsync(templateHtml);
    }
}