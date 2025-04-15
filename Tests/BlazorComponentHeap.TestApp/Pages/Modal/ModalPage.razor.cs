namespace BlazorComponentHeap.TestApp.Pages.Modal;

public partial class ModalPage
{
    private bool _showModal1 = false;
    private bool _showModal2 = false;

    private void OnModal1SwitchClicked()
    {
        _showModal1 = !_showModal1;
        StateHasChanged();
    }
    
    private void OnModal2SwitchClicked()
    {
        _showModal2 = !_showModal2;
        StateHasChanged();
    }
}
