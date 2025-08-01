using BlazorComponentHeap.Files.Events;
using BlazorComponentHeap.Files.Models;

namespace BlazorComponentHeap.Files;

public interface IBchFilesService
{
    BchFilesContext GetContextFromDropEvent(DropFileEventArgs e);
    Task<BchFilesContext> RequestFileDialogAsync(bool multiple = false, bool createImagePreview = false);
}