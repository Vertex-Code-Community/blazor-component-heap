using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Models.Datepicker;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using BlazorComponentHeap.Shared.Models.Events;
using BlazorComponentHeap.Shared.Models.Math;

namespace BlazorComponentHeap.Components.RangeCalendar;

public partial class BCHRangeCalendar : IAsyncDisposable
{
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;

    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string Format { get; set; } = string.Empty;
    [Parameter] public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;
    [Parameter] public EventCallback<DateRange> ValuesChanged { get; set; }

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

    private int _selectedYear = DateTime.Now.Year;
    private int _selectedMonth = DateTime.Now.Month;

    private readonly string _subscriptionKey = $"_key_{Guid.NewGuid()}";
    private Vec2 _containerPos = new();
    private NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    protected override void OnInitialized()
    {
        IJSUtilsService.OnGlobalScroll += OnGlobalScrollAsync;
        
        _culture = new CultureInfo(Culture);

        _values.Start = DateTime.MinValue;
        _values.End = DateTime.MinValue;

        if (string.IsNullOrWhiteSpace(Format))
        {
            Format = _culture.DateTimeFormat.ShortDatePattern;
        }
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsUtilsService.AddDocumentListenerAsync<ExtMouseEventArgs>("mousedown", _subscriptionKey,
                OnDocumentMouseDownAsync);
        }
        
        if (_showDate || _showMonth) await _inputRef.FocusAsync();
    }

    public async ValueTask DisposeAsync()
    {
        IJSUtilsService.OnGlobalScroll -= OnGlobalScrollAsync;
        await JsUtilsService.RemoveDocumentListenerAsync<ExtMouseEventArgs>("mousedown", _subscriptionKey);
    }

    private Task OnGlobalScrollAsync(ScrollEventArgs e)
    {
        var scrollContainer = e.PathCoordinates.FirstOrDefault();
        if (scrollContainer?.Id == $"{_yearsSelectContentId}_scroller") return Task.CompletedTask;
        
        _showDate = false;
        _showMonth = false;
        StateHasChanged();

        return Task.CompletedTask;
    }

    private Task OnDocumentMouseDownAsync(ExtMouseEventArgs e)
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
            
            return Task.CompletedTask;
        }
        
        if (_showMonth)
        {
            _showMonth = false;
            _showDate = true;
            StateHasChanged();
            return Task.CompletedTask;
        }
        
        _showDate = false;
        _showMonth = false;
        StateHasChanged();

        return Task.CompletedTask;
    }

    private string GetValues()
    {
        var value = $"{DateTime.Now.ToString(Format)} - {DateTime.Now.ToString(Format)}";

        if (Values.Start != DateTime.MinValue)
        {
            value = $"{Values.Start.ToString(Format)}";
            _defaultStartDay = Values.Start;
        }

        if (Values.End != DateTime.MinValue)
        {
            value += $" - {Values.End.ToString(Format)}";
            _defaultEndDay = Values.End;
        }

        return value;
    }

    private async Task OnCalendarClickedAsync()
    {
        var containerRect = await JsUtilsService.GetBoundingClientRectAsync(_containerId);
        _containerPos.Set(containerRect.X, containerRect.Y);
        
        _showDate = !_showMonth && !_showDate;
        _showMonth = false;
        _defaultStartDay = DateTime.MinValue;
        
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
            var containerRect = await JsUtilsService.GetBoundingClientRectAsync(_containerId);
            _containerPos.Set(containerRect.X, containerRect.Y);
        }
        
        StateHasChanged();
    }

    private async Task TrustedValuesAsync(DateRange date)
    {
        await ValuesChanged.InvokeAsync(date);
    }
}