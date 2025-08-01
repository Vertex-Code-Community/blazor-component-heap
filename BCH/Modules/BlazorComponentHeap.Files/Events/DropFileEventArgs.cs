using BlazorComponentHeap.Files.Models;

namespace BlazorComponentHeap.Files.Events;

public class DropFileEventArgs : EventArgs
{
    public List<BchFileInfoModel> Files { get; set; } = new ();
}