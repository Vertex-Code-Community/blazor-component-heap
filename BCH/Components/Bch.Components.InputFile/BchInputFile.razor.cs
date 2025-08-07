using Bch.Components.InputFile.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Bch.Modules.Files;
using Bch.Modules.Files.Models;

namespace Bch.Components.InputFile;

public partial class BchInputFile : ComponentBase
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