using BlazorComponentHeap.Cropper;

namespace BlazorComponentHeap.TestApp.Pages.Cropper;

public partial class CropperPage
{
    private BCHCropper _bchCropper = null!;

    private string _resultImage = string.Empty;
    
    private async Task OnGetResultAsync()
    {
        _resultImage = await _bchCropper.GetBase64ResultAsync();
        Console.WriteLine(_resultImage);
    }
}