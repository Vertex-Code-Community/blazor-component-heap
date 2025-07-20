using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using BlazorComponentHeap.Files;
using BlazorComponentHeap.Files.Models;
using BlazorComponentHeap.Input.Events;

namespace BlazorComponentHeap.Input;

public partial class BCHInputFile : ComponentBase
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public EventCallback<BchFilesContext> OnChange { get; set; }

    private Task OnChangedAsync(BchFilesOnChangeEvent e)
    {
        return OnChange.InvokeAsync(new BchFilesContext
        {
            Files = e.Files.Select(x => (IBrowserFile) new BchBrowserFile
            {
                JsRuntime = JsRuntime,
                Id = x.Id,
                Name = x.Name,
                ContentType = x.ContentType,
                Size = x.Size,
                LastModified = x.LastModified,
                ImagePreviewUrl = x.ImagePreviewUrl
            }).ToList()
        });
    }
}