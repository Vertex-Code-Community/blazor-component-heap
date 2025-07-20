using BlazorComponentHeap.Files.Models;

namespace BlazorComponentHeap.Input.Events;

public class BchFilesOnChangeEvent : EventArgs
{
    public List<BchFileInfoModel> Files { get; set; } = new();
}