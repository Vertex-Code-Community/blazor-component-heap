using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using BlazorComponentHeap.Files.Models;

namespace BlazorComponentHeap.Files.Services;

public class BchFilesService : IBchFilesService
{
    private readonly IJSRuntime _jsRuntime;
    
    public BchFilesService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<BchFilesContext> RequestFileDialogAsync(bool multiple = false, bool createImagePreview = false)
    {
        var fileInfos = await _jsRuntime.InvokeAsync<List<BchFileInfoModel>?>("pickFile_BCH", multiple, createImagePreview);

        return new BchFilesContext
        {
            Files = fileInfos?.Select(x => (IBrowserFile) new BchBrowserFile
            {
                JsRuntime = _jsRuntime,
                Id = x.Id,
                Name = x.Name,
                ContentType = x.ContentType,
                Size = x.Size,
                LastModified = x.LastModified,
                ImagePreviewUrl = x.ImagePreviewUrl
            }).ToList() ?? new List<IBrowserFile>()
        };
    }
}