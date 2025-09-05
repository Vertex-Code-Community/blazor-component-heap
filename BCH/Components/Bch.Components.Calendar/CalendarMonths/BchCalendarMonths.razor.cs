using System.Globalization;
using Microsoft.AspNetCore.Components;
using Month = Bch.Components.Calendar.Models.Month;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Calendar.CalendarMonths;

public partial class BchCalendarMonths
{
    private class Year
    {
        public int Number { get; set; }
    }

    [Parameter] public string Id { get; set; } = string.Empty;
    [Parameter] public string YearsSelectContentId { get; set; } = $"_id_{Guid.NewGuid()}";
    [Parameter] public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;
    [Parameter] public int SelectedMonth { get; set; }
    [Parameter] public int SelectedYear { get; set; }
    [Parameter] public EventCallback<bool> IsShowDate { get; set; }
    [Parameter] public EventCallback<int> MonthChanged { get; set; }
    [Parameter] public EventCallback<int> YearChanged { get; set; }
    [Parameter] public EventCallback OnFocusOut { get; set; }

    // Theme support
    [Parameter] public BchTheme? Theme { get; set; }
    private readonly string _cssKey = $"_cssKey_{Guid.NewGuid()}";
    private string GetThemeCssClass()
    {
        var themeClass = Theme?.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
        var suffix = Theme is null ? " bch-no-theme-specified" : string.Empty;
        return themeClass + suffix;
    }

    private int _monthValue;
    private int _yearValue;
    private bool _isOpenedYearSelect;
    private Year _selectedYear = new();
    private List<Month> _month = new();
    private List<Year> _years = new();
    private CultureInfo _culture = null!;

    protected override void OnInitialized()
    {
        _culture = new CultureInfo(Culture);

        _monthValue = DateTime.Now.Month;
        _yearValue = DateTime.Now.Year;

        SetMonthsToCalendar();
        SetYearsToCalendar();

        _selectedYear = SelectedYear is default(int) ? _years.FirstOrDefault(x => x.Number == _yearValue)! : _years.FirstOrDefault(x => x.Number == SelectedYear)!;
    }

    private void SetMonthsToCalendar()
    {
        var monthNames = _culture.DateTimeFormat.MonthNames;

        for (int i = 0; i < monthNames.Length - 1; i++)
        {
            _month.Add(
                new Month 
                { 
                    Name = DateTime.ParseExact(monthNames[i], "MMMM", _culture).ToString("MMM"), 
                    Number = i + 1 
                }
            );
        }
    }

    private void SetYearsToCalendar()
    {
        for (int i = _yearValue; i > _yearValue - 100; i--)
        {
            _years.Add(new Year { Number = i });
        }

        _years = Enumerable.Reverse(_years).ToList();

        for (int i = _yearValue + 1; i < _yearValue + 100; i++)
        {
            _years.Add(new Year { Number = i });
        }
    }

    private async Task OnSelectMonthAsync(int month)
    {
        await OnFocusOut.InvokeAsync();
        await MonthChanged.InvokeAsync(month);
        await IsShowDate.InvokeAsync(true);
    }

    private Task OnSelectYearAsync(Year? year)
    {
        if (year is null) return Task.CompletedTask; // TODO: have a look on the condition
        return YearChanged.InvokeAsync(year.Number);
    }
}
