namespace BlazorComponentHeap.TestAppServer.TestModels;

public class UserModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime BirthDay { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}