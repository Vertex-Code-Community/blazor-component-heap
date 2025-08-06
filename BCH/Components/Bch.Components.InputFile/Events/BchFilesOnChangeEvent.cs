using Bch.Modules.Files.Models;

namespace Bch.Components.InputFile.Events;

public class BchFilesOnChangeEvent : EventArgs
{
    public List<BchFileInfoModel> Files { get; set; } = new();
}