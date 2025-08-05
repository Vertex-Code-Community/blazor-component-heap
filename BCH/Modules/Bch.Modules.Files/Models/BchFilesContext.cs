using Microsoft.AspNetCore.Components.Forms;

namespace Bch.Modules.Files.Models;

public class BchFilesContext : IDisposable
{
    public List<IBrowserFile> Files { get; set; } = new();
    
    public void Dispose()
    {
        var bchFiles = Files.OfType<BchBrowserFile>().ToList();
        foreach (var bchFile in bchFiles)
        {
            try
            {
                bchFile.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}