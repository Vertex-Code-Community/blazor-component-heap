using BlazorComponentHeap.Files.Models;

namespace BlazorComponentHeap.Files;

public interface IBchFilesService
{
    Task<BchFilesContext> RequestFileDialogAsync(bool multiple = false, bool createImagePreview = false);
}