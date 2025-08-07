using Bch.Modules.Files.Models;

namespace Bch.Modules.Files.Events;

public class DropFileEventArgs : EventArgs
{
    public List<BchFileInfoModel> Files { get; set; } = new ();
}