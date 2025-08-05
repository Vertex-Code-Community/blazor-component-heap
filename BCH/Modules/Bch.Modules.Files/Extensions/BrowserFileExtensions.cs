using Microsoft.AspNetCore.Components.Forms;

namespace Bch.Modules.Files.Extensions;

public static class BrowserFileExtensions
{
    public static string? GetImagePreview(this IBrowserFile file)
    {
        return (file as BchBrowserFile)?.ImagePreviewUrl;
    }
}