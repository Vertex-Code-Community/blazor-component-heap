using Microsoft.AspNetCore.Components;
using Bch.Components.WYSIWYG.Attributes;
using Bch.Components.WYSIWYG.Models;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.WYSIWYG.Components.FloatingToolbar;

public partial class BchEditorFloatingToolbar
{
    [Parameter] public int CommandsActivated { get; set; }
    [Parameter] public string Paragraph { get; set; } = string.Empty;
    [Parameter] public EventCallback<(BchEditorControlButton, string)> OnButtonClick { get; set; }
    
    private async Task OnAlignTextSelectedAsync(string? text)
    {
        if (string.IsNullOrEmpty(text)) return;
        
        var control = text switch
        {
            "left" => BchEditorControlButton.JustifyLeft,
            "right" => BchEditorControlButton.JustifyRight,
            "justify" => BchEditorControlButton.JustifyCenter,
            _ => BchEditorControlButton.FormatBlock
        };

        if (IsActive(control)) return;

        await OnButtonClickedAsync(control);
    }
    
    private async Task OnButtonClickedAsync(BchEditorControlButton control, string value = null!)
    {
        await OnButtonClick.InvokeAsync((control, value));
    }
    
    private bool IsActive(BchEditorControlButton button)
    {
        return IsKthBitSet(CommandsActivated, button.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));
    }
    
    private async Task OnParagraphSelectedAsync(string? text)
    {
        if (string.IsNullOrEmpty(text)) return; 
        
        var parameters = text switch
        {
            "H1" => "<h1>",
            "H2" => "<h2>",
            "H3" => "<h3>",
            "H4" => "<h4>",
            "H5" => "<h5>",
            "H6" => "<h6>",
            "Code" => "<pre>",
            "Quotation" => "<blockquote>",
            _ => "<p>"
        };

        await OnButtonClick.InvokeAsync((BchEditorControlButton.FormatBlock, parameters));
    }
    
    private bool IsKthBitSet(int n, int k) => (n & (1 << k)) != 0;
}