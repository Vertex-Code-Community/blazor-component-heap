namespace BlazorComponentHeap.Core.Events;

public class DropFileEventArgs : EventArgs
{
    public DropFile[] Files { get; set; } = [];
}

public class DropFile
{
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}