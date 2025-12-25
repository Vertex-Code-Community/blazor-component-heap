using Microsoft.AspNetCore.Components;
using Bch.Components.WYSIWYG.Attributes;
using Bch.Components.WYSIWYG.Models;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.WYSIWYG.Components.ImageOptions;

public partial class BchEditorImageOptions
{
    [Parameter] public EventCallback OnImageReplaceClicked { get; set; }
    [Parameter] public int CommandsActivated { get; set; }
    [Parameter] public EventCallback<(BchEditorControlButton, string)> OnButtonClick { get; set; }

    private Task OnImageReplaceClickedAsync()
    {
        return OnImageReplaceClicked.InvokeAsync();
    }

    private Task OnAlignTextSelectedAsync(string? text)
    {
        if (string.IsNullOrEmpty(text)) return Task.CompletedTask;
        
        var control = text switch
        {
            "left" => BchEditorControlButton.JustifyLeft,
            "right" => BchEditorControlButton.JustifyRight,
            "justify" => BchEditorControlButton.JustifyCenter,
            _ => BchEditorControlButton.FormatBlock
        };

        if (IsActive(control)) return Task.CompletedTask;

        return OnButtonClickedAsync(control);
    }

    private async Task OnButtonClickedAsync(BchEditorControlButton control, string value = null!)
    {
        await OnButtonClick.InvokeAsync((control, value));
    }

    private bool IsActive(BchEditorControlButton button)
    {
        return IsKthBitSet(CommandsActivated, button.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));
    }

    private bool IsKthBitSet(int n, int k) => (n & (1 << k)) != 0;
}