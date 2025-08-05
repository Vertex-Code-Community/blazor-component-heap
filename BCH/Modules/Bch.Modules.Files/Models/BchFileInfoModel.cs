namespace Bch.Modules.Files.Models;

public class BchFileInfoModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset LastModified { get; set; }
    public required long Size { get; set; }
    public required string ContentType { get; set; }
    public string? ImagePreviewUrl { get; set; }
}