namespace Bch.Integration.Pages.Models;

public class UserModel
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Sex { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    public DateTime ReportDate { get; set; }
}