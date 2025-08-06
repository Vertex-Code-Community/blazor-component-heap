namespace Bch.Integration.Pages.Pages.Toggle;

public partial class TogglePage
{

    private bool _value = true;

    private void OnClick()
    {
        Console.WriteLine(_value);
    }

}
