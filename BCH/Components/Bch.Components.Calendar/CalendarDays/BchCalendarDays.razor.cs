using System.Globalization;
using Microsoft.AspNetCore.Components;
using DateRange = Bch.Components.Calendar.Models.DateRange;
using WeekDay = Bch.Components.Calendar.Models.WeekDay;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Calendar.CalendarDays;

public partial class BchCalendarDays
{
    [Parameter] public string Id { get; set; } = string.Empty;
    
    [Parameter]
    public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;

    [Parameter]
    public int DefaultMonth { get; set; }

    [Parameter]
    public int DefaultYear { get; set; }

    [Parameter]
    public bool IsDateRange { get; set; }

    [Parameter]
    public EventCallback<bool> IsShowMonth { get; set; }

    [Parameter]
    public EventCallback<DateTime> OnDateSelected { get; set; }

    [Parameter]
    public EventCallback OnFocusOut { get; set; }

    [Parameter]
    public EventCallback OnDateCleared { get; set; }

    [Parameter]
    public DateTime? SelectedDate { get; set; }

    [Parameter]
    public EventCallback<DateTime> SelectedStartDay { get; set; }

    [Parameter]
    public DateTime StartDay { get; set; } = DateTime.MinValue;

    [Parameter]
    public DateTime EndDay { get; set; } = DateTime.MinValue;

    [Parameter]
    public EventCallback OnCloseDate { get; set; }

    [Parameter]
    public EventCallback<DateRange> ValuesChanged { get; set; }

    [Parameter]
    public DateRange Values
    {
        get => _values;
        set
        {
            if (_values == value) return;
            _values = value;

            ValuesChanged.InvokeAsync(value);
        }
    }

    [Parameter]
    public EventCallback<DateRange> TrustedValues { get; set; }

    [Parameter]
    public bool ShowClearButton { get; set; } = false;

    // Theme support
    [Parameter] public BchTheme? Theme { get; set; }
    private readonly string _cssKey = $"_cssKey_{Guid.NewGuid()}";
    private string GetThemeCssClass()
    {
        var themeClass = Theme?.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
        var suffix = Theme is null ? " bch-no-theme-specified" : string.Empty;
        return themeClass + suffix;
    }

    private List<string> _weekDays = new();
    private List<WeekDay> _days = new();
    private int _rowsCount = 0;
    private CultureInfo _culture = null!;
    private int _month = DateTime.Now.Month;
    private int _year = DateTime.Now.Year;
    private DateRange _values = new();

    private DateTime _startDay;
    private DateTime _endDay;

    protected override void OnInitialized()
    {
        _culture = new CultureInfo(Culture);

        _startDay = StartDay;
        _endDay = EndDay;

        if (SelectedDate.HasValue)
        {
            _month = SelectedDate.Value.Month;
            _year = SelectedDate.Value.Year;
        }
        else
        {
            _month = DefaultMonth == 0 ? DateTime.Now.Month : DefaultMonth;
            _year = DefaultYear == 0 ? DateTime.Now.Year : DefaultYear;
        }

        SetWeekDayNamesToCalendars();
        UpdateCalendar(_year, _month);
    }

    private void UpdateCalendar(int year, int month)
    {
        var countDayOfMounth = DateTime.DaysInMonth(year, month);

        var countDayInPreviusMonth = DateTime.DaysInMonth(GetPreviousYear(year, month), GetPreviousMonth(month));

        var firstDayInMonth = (int)new DateTime(year, month, 1).DayOfWeek;
        var lastDayInMonth = (int)new DateTime(year, month, countDayOfMounth).DayOfWeek;
        int numberOfEmptyDays = 0;

        firstDayInMonth = firstDayInMonth == 0 ? 7 : firstDayInMonth;
        numberOfEmptyDays = firstDayInMonth - 1;

        _days.Clear();

        if (_culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
        {
            numberOfEmptyDays += 1;
        }

        if (_culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Saturday)
        {
            numberOfEmptyDays += 2;
        }

        for (int i = 0; i < numberOfEmptyDays; i++)
        {
            _days.Add(
                new WeekDay
                {
                    Date = new DateTime(GetPreviousYear(year, month), GetPreviousMonth(month), countDayInPreviusMonth--),
                    IsOtherDay = true
                }
            );
        }

        _days = Enumerable.Reverse(_days).ToList();

        for (int i = 0; i < countDayOfMounth; i++)
        {
            var day = i + 1;
            _days.Add(
                new WeekDay
                {
                    Date = new DateTime(year, month, day),
                    IsOtherDay = false
                }
            );
        }

        if (lastDayInMonth != 7 && lastDayInMonth != 0)
        {
            for (int i = 0; i < (7 - lastDayInMonth); i++)
            {
                var newDay = i + 1;
                _days.Add(
                    new WeekDay
                    {
                        Date = new DateTime(GetNextYear(year, month), GetNextMonth(month), newDay),
                        IsOtherDay = true
                    }
                );
            }
        }

        _rowsCount = _days.Count % 7 == 0 ? (_rowsCount = _days.Count / 7) : (Convert.ToInt32(_days.Count / 7) + 1);
        StateHasChanged();
    }

    private void SetWeekDayNamesToCalendars()
    {
        _weekDays = _culture.DateTimeFormat.ShortestDayNames.ToList();

        if (_culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Monday)
        {
            _weekDays.Add(_weekDays[0]);
            _weekDays.RemoveAt(0);
        }

        if (_culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Saturday)
        {
            _weekDays.Insert(0, _weekDays[6]);
            _weekDays.RemoveAt(7);
        }
    }

    private int GetPreviousMonth(int month)
    {
        return month == 1 ? 12 : month - 1;
    }

    private int GetPreviousYear(int currentYear, int month)
    {
        return month == 1 ? currentYear - 1 : currentYear;
    }

    private int GetNextMonth(int month)
    {
        return month == 12 ? 1 : month + 1;
    }

    private int GetNextYear(int currentYear, int month)
    {
        return month == 12 ? currentYear + 1 : currentYear;
    }

    private async Task NextMonthAsync()
    {
        if (_month == 12)
        {
            _month = 1;
            _year++;
            UpdateCalendar(_year, _month);
            return;
        }
        _month++;
        UpdateCalendar(_year, _month);
        await OnFocusOut.InvokeAsync();

    }

    private async Task PreviousMonthAsync()
    {
        if (_month == 1)
        {
            _month = 12;
            _year--;
            UpdateCalendar(_year, _month);
            return;
        }
        _month--;
        UpdateCalendar(_year, _month);
        await OnFocusOut.InvokeAsync();
    }

    private string GetMonthName(int month)
    {
        return month switch
        {
            1 => _culture.DateTimeFormat.GetMonthName(month),
            2 => _culture.DateTimeFormat.GetMonthName(month),
            3 => _culture.DateTimeFormat.GetMonthName(month),
            4 => _culture.DateTimeFormat.GetMonthName(month),
            5 => _culture.DateTimeFormat.GetMonthName(month),
            6 => _culture.DateTimeFormat.GetMonthName(month),
            7 => _culture.DateTimeFormat.GetMonthName(month),
            8 => _culture.DateTimeFormat.GetMonthName(month),
            9 => _culture.DateTimeFormat.GetMonthName(month),
            10 => _culture.DateTimeFormat.GetMonthName(month),
            11 => _culture.DateTimeFormat.GetMonthName(month),
            12 => _culture.DateTimeFormat.GetMonthName(month),
            _ => ""
        };
    }

    private async Task SelectDateAsync(DateTime date)
    {
        if (IsDateRange)
        {
            await SelectRangeDateAsync(date);
        }

        await OnFocusOut.InvokeAsync();
        await OnDateSelected.InvokeAsync(date);
        StateHasChanged();
    }

    private async Task ApplyDatesAsync()
    {
        Values.Start = _startDay;
        Values.End = _endDay;

        await TrustedValues.InvokeAsync(Values);
        await OnCloseDate.InvokeAsync();
    }

    private async Task ClearDatesAsync()
    {
        _startDay = DateTime.MinValue;
        _endDay = DateTime.MinValue;
        Values.Start = DateTime.MinValue;
        Values.End = DateTime.MinValue;

        await TrustedValues.InvokeAsync(Values);
        StateHasChanged();
    }

    private async Task SelectRangeDateAsync(DateTime date)
    {

        if (_startDay == date)
        {
            _startDay = DateTime.MinValue;
            _endDay = DateTime.MinValue;
        }

        if (_endDay == date)
        {
            _endDay = DateTime.MinValue;
            _startDay = date;
            await SelectedStartDay.InvokeAsync(date);
        }

        if (_startDay == DateTime.MinValue)
        {
            _startDay = date;
            await SelectedStartDay.InvokeAsync(date);
        }

        if (date > _startDay)
        {
            _endDay = date;
        }

        if (date < _startDay)
        {
            _startDay = date;
            _endDay = DateTime.MinValue;
            await SelectedStartDay.InvokeAsync(date);
        }

        StateHasChanged();
    }

    private async Task OnClickByMonthAsync()
    {
        await IsShowMonth.InvokeAsync(true);
    }

    private void ClearSingleDate()
    {
        OnDateCleared.InvokeAsync();
        StateHasChanged();
    }

}
