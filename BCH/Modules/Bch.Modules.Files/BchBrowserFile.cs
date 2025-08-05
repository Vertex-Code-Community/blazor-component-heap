using Bch.Modules.Files.Streams;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Bch.Modules.Files;

public class BchBrowserFile : IBrowserFile, IDisposable
{
    public required IJSRuntime JsRuntime { get; set; }
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset LastModified { get; set; }
    public required long Size { get; set; }
    public required string ContentType { get; set; }
    public string? ImagePreviewUrl { get; set; }
    
    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = new())
    {
        return new BchBrowserFileStream(JsRuntime, this, maxAllowedSize, cancellationToken);
    }

    public void Dispose()
    {
        try
        {
            JsRuntime.InvokeVoidAsync("disposeFileState_BCH", Id);
        }
        catch
        {
            // ignored
        }
    }
}