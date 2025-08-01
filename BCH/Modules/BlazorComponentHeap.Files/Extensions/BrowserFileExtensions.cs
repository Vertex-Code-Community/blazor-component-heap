using Microsoft.AspNetCore.Components.Forms;

namespace BlazorComponentHeap.Files.Extensions;

public static class BrowserFileExtensions
{
    public static string? GetImagePreview(this IBrowserFile file)
    {
        return (file as BchBrowserFile)?.ImagePreviewUrl;
    }
}