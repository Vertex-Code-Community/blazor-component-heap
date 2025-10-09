using System.Globalization;
using Microsoft.AspNetCore.Components;
using Bch.Components.Calendar.Models;
using Bch.Modules.DomInterop.Services;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.Maths.Models;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.RangeCalendar;

public partial class BchRangeCalendar : IAsyncDisposable
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }

    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string Format { get; set; } = string.Empty;
    [Parameter] public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;
    [Parameter] public EventCallback<DateRange> ValuesChanged { get; set; }
    [Parameter] public bool CollapseOnClickOutside { get; set; } = true;

    // Theme support (cascading + explicit override)
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private readonly string _cssKey = $"_cssKey_{Guid.NewGuid()}";

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
    
    [Parameter] public DateTime DefaultValue { get; set; } = DateTime.Now;

    private DateRange _values = new();
    private bool _showDate = false;
    private bool _showMonth = false;
    private string _containerId = $"_id_{Guid.NewGuid()}";
    private readonly string _calendarDaysId = $"_id_{Guid.NewGuid()}";
    private readonly string _calendarMonthsId = $"_id_{Guid.NewGuid()}";
    private readonly string _yearsSelectContentId = $"_id_{Guid.NewGuid()}";
    private string _inputId = $"_id_{Guid.NewGuid()}";
    private ElementReference _inputRef;
    private CultureInfo _culture = null!;

    private DateTime _defaultStartDay;
    private DateTime _defaultEndDay;

    private int _selectedYear;
    private int _selectedMonth;

    private readonly string _subscriptionKey = $"_key_{Guid.NewGuid()}";
    private Vec2 _containerPos = new();
    private NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };
    private bool _openUp = false;
    private const float _modalWidthPx = 250f;
    private const float _modalHeightEstimatePx = 320f; // approximate calendar height
    private const float _screenPaddingPx = 8f;

    protected override Task OnInitializedAsync()
    {
        _culture = new CultureInfo(Culture);
        _selectedYear = DefaultValue.Year;
        _selectedMonth = DefaultValue.Month;

        if (string.IsNullOrWhiteSpace(Format))
            Format = "MM/dd/yyyy";

        return GlobalEventsService.AddDocumentListenerAsync<BchMouseEventArgs>("mousedown", _subscriptionKey,
            OnDocumentMouseDownAsync);
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_showDate || _showMonth) await _inputRef.FocusAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await GlobalEventsService.RemoveDocumentListenerAsync<BchMouseEventArgs>("mousedown", _subscriptionKey);
    }

    private Task OnWindowGlobalScrollAsync(BchScrollEventArgs e)
    {
        var scrollContainer = e.PathCoordinates.FirstOrDefault();
        if (scrollContainer?.Id == $"{_yearsSelectContentId}_scroller") return Task.CompletedTask;
        
        _showDate = false;
        _showMonth = false;
        StateHasChanged();

        return UnsubscribeFromGlobalScrollAsync();
    }

    private Task OnDocumentMouseDownAsync(BchMouseEventArgs e)
    {
        var container = e.PathCoordinates
            .FirstOrDefault(x => 
                x.Id == _containerId || x.Id == _calendarDaysId || 
                x.Id == _calendarMonthsId || x.Id == _yearsSelectContentId);

        if (container != null || !CollapseOnClickOutside) return Task.CompletedTask; // inside calendar

        var otherCalendar = e.PathCoordinates
            .Any(x => x.ClassList.Contains("bch-datepicker-wrapper") ||
                      x.ClassList.Contains("bch-datepicker-wrapper") ||
                      x.ClassList.Contains("bch-select-container"));

        if (otherCalendar)
        {
            _showDate = false;
            _showMonth = false;
            StateHasChanged();
            
            return UnsubscribeFromGlobalScrollAsync();
        }
        
        if (_showMonth)
        {
            _showMonth = false;
            _showDate = true;
            StateHasChanged();
            return SubscribeOnGlobalScrollAsync();
        }
        
        _showDate = false;
        _showMonth = false;
        StateHasChanged();

        return UnsubscribeFromGlobalScrollAsync();
    }

    private string GetValues()
    {
        if (Values.Start == null && Values.End == null)
        {
            return $"{Format} - {Format}";
        }

        if (Values.Start == DateTime.MinValue && Values.End == DateTime.MinValue)
        {
            return $"{Format} - {Format}";
        }

        if (Values.Start != null && Values.End != null)
        {
            _defaultStartDay = Values.Start.Value;
            _defaultEndDay = Values.End.Value;
            return $"{Values.Start.Value.ToString(Format)} - {Values.End.Value.ToString(Format)}";
        }

        if (Values.Start != null)
        {
            _defaultStartDay = Values.Start.Value;
            return $"{Values.Start.Value.ToString(Format)} - {Format}";
        }

        if (Values.End != null)
        {
            _defaultEndDay = Values.End.Value;
            return $"{Format} - {Values.End.Value.ToString(Format)}";
        }

        return $"{Format} - {Format}";
    }


    private async Task OnCalendarClickedAsync()
    {
        var containerRect = await DomInteropService.GetBoundingClientRectAsync(_containerId);
        if (containerRect is null) return;
        var win = await DomInteropService.GetWindowSizeAsync();
        
        var x = containerRect.X;
        var maxX = MathF.Max(0, win.Width - _modalWidthPx - _screenPaddingPx);
        if (x > maxX) x = maxX;
        if (x < _screenPaddingPx) x = _screenPaddingPx;
        _containerPos.Set(x, containerRect.Y);
        
        var spaceBelow = win.Height - containerRect.Bottom;
        var spaceAbove = containerRect.Top;
        _openUp = spaceBelow < _modalHeightEstimatePx && spaceAbove > spaceBelow;
        
        _showDate = !_showMonth && !_showDate;
        _showMonth = false;
        _defaultStartDay = DateTime.MinValue;
        
        if (_showDate)
            await SubscribeOnGlobalScrollAsync();
        
        StateHasChanged();
    }

    private void SetStartDay(DateTime date)
    {
        _defaultStartDay = date;
    }
    
    private async Task OnShowModeChangedAsync()
    {
        if (_showDate || _showMonth)
        {
            var containerRect = await DomInteropService.GetBoundingClientRectAsync(_containerId);
            if (containerRect is null) return;
            var win = await DomInteropService.GetWindowSizeAsync();
            
            var x = containerRect.X;
            var maxX = MathF.Max(0, win.Width - _modalWidthPx - _screenPaddingPx);
            if (x > maxX) x = maxX;
            if (x < _screenPaddingPx) x = _screenPaddingPx;
            _containerPos.Set(x, containerRect.Y);
            
            var spaceBelow = win.Height - containerRect.Bottom;
            var spaceAbove = containerRect.Top;
            _openUp = spaceBelow < _modalHeightEstimatePx && spaceAbove > spaceBelow;
            
            await SubscribeOnGlobalScrollAsync();
        }
        else
        {
            await UnsubscribeFromGlobalScrollAsync();
        }
        
        StateHasChanged();
    }

    private async Task TrustedValuesAsync(DateRange date)
    {
        await ValuesChanged.InvokeAsync(date);
    }

    private string GetThemeCssClass()
    {
        var themeSpecified = Theme ?? ThemeCascading;
        var themeClass = EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
        return themeClass + (themeSpecified is null ? " bch-no-theme-specified" : "");
    }
}