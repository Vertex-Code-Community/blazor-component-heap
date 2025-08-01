namespace BlazorComponentHeap.Calendar;

public partial class BCHCalendar
{
    private Task OnDateChanged(bool showDate)
    {
        _showDate = showDate;
        StateHasChanged();

        return _showDate || _showMonth ? SubscribeOnGlobalScrollAsync() : UnsubscribeFromGlobalScrollAsync();
    }
    
    private Task OnMonthChanged(bool showMonth)
    {
        _showMonth = showMonth;
        StateHasChanged();

        return _showDate || _showMonth ? SubscribeOnGlobalScrollAsync() : UnsubscribeFromGlobalScrollAsync();
    }
}