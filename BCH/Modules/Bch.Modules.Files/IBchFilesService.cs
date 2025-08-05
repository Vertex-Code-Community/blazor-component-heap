using Bch.Modules.Files.Events;
using Bch.Modules.Files.Models;

namespace Bch.Modules.Files;

public interface IBchFilesService
{
    BchFilesContext GetContextFromDropEvent(DropFileEventArgs e);
    Task<BchFilesContext> RequestFileDialogAsync(bool multiple = false, bool createImagePreview = false);
}