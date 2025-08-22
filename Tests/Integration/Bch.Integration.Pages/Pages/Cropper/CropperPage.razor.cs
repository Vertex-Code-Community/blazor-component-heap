using Bch.Components.Cropper;
using Bch.Modules.Files.Extensions;

namespace Bch.Integration.Pages.Pages.Cropper;

public partial class CropperPage
{
    private BchCropper? _bchCropper;
    private string? _previewImageUrl = null;
    
    private async Task OnGetResultAsync()
    {
        if (_bchCropper is null) return;
        
        var fileContext = await _bchCropper.GetFileResultAsync();
        _previewImageUrl = fileContext.Files.FirstOrDefault()?.GetImagePreview();
    }
}