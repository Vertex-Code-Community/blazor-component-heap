using System.Globalization;
using Microsoft.AspNetCore.Components;
using Bch.Modules.DomInterop.Services;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.Maths.Models;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Calendar;

public partial class BchCalendar : IAsyncDisposable
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }

    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string Format { get; set; } = string.Empty;
    [Parameter] public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;
    [Parameter] public EventCallback<DateTime?> ValueChanged { get; set; }
    [Parameter] public DateTime? Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;

            ValueChanged.InvokeAsync(value);
        }
    }

    [Parameter] public bool ShowClearButton { get; set; } = false;

    // Theme support (cascading + explicit override)
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }

    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() 
    {
        var cssClass = EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
        Console.WriteLine($"BchCalendar EffectiveTheme: {EffectiveTheme}, CSS Class: '{cssClass}'");
        return cssClass;
    }

    private DateTime? _value = null;
    private bool _showDate = false;
    private bool _showMonth = false;
    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    private readonly string _calendarDaysId = $"_id_{Guid.NewGuid()}";
    private readonly string _calendarMonthsId = $"_id_{Guid.NewGuid()}";
    private readonly string _yearsSelectContentId = $"_id_{Guid.NewGuid()}";
    private readonly string _inputId = $"_id_{Guid.NewGuid()}";
    private readonly string _subscriptionKey = $"_key_{Guid.NewGuid()}";
    private ElementReference _inputRef;
    private CultureInfo _culture = null!;
    private Vec2 _containerPos = new ();
    private NumberFormatInfo _nF = new () { NumberDecimalSeparator = "." };

    private int _selectedYear;
    private int _selectedMonth;

    protected override Task OnInitializedAsync()
    {
        _culture = new CultureInfo(Culture);
        Format = string.IsNullOrWhiteSpace(Format) ? _culture.DateTimeFormat.ShortDatePattern : Format;
        
        return GlobalEventsService.AddDocumentListenerAsync<BchMouseEventArgs>("mousedown", _subscriptionKey,
            OnDocumentMouseDownAsync);
    }

    public async ValueTask DisposeAsync()
    {
        await GlobalEventsService.RemoveDocumentListenerAsync<BchMouseEventArgs>("mousedown", _subscriptionKey);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_showDate || _showMonth) await _inputRef.FocusAsync();
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

        if (container != null) return Task.CompletedTask; // inside calendar

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

    private async Task OnCalendarClickedAsync()
    {
        var containerRect = await DomInteropService.GetBoundingClientRectAsync(_containerId);
        if (containerRect is null) return;
        
        _containerPos.Set(containerRect.X, containerRect.Y);
        
        var currentDate = Value ?? DateTime.Now;
        if (currentDate.Year != _selectedYear) _selectedYear = currentDate.Year;
        if (currentDate.Month != _selectedMonth) _selectedMonth = currentDate.Month;
        
        _showDate = !_showMonth && !_showDate;
        _showMonth = false;

        if (_showDate)
            await SubscribeOnGlobalScrollAsync();
        
        StateHasChanged();
    }

    private async Task OnShowModeChangedAsync()
    {
        if (_showDate || _showMonth)
        {
            var containerRect = await DomInteropService.GetBoundingClientRectAsync(_containerId);
            if (containerRect is null) return;
            
            _containerPos.Set(containerRect.X, containerRect.Y);
            
            await SubscribeOnGlobalScrollAsync();
        }
        else
        {
            await UnsubscribeFromGlobalScrollAsync();
        }
        
        StateHasChanged();
    }

    private string GetPlaceholderText()
    {
        return Format switch
        {
            "MM/dd/yyyy" => "MM-DD-YYYY",
            "dd/MM/yyyy" => "DD-MM-YYYY",
            "yyyy-MM-dd" => "YYYY-MM-DD",
            _ => Culture switch
            {
                "en-US" => "MM-DD-YYYY",
                "en-GB" => "DD-MM-YYYY",
                _ => "YYYY-MM-DD"
            }
        };
    }

    private void ClearValue()
    {
        Value = null;
        StateHasChanged();
    }
}