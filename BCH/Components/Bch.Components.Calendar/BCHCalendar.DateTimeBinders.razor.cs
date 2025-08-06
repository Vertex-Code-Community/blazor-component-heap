namespace Bch.Components.Calendar;

public partial class BchCalendar
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