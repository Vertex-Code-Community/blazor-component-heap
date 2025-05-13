using System.Globalization;
using BlazorComponentHeap.Core.Models.Events;
using BlazorComponentHeap.Core.Models.Math;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Calendar;

public partial class BCHCalendar : IAsyncDisposable
{
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;

    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string Format { get; set; } = string.Empty;
    [Parameter] public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;
    [Parameter] public EventCallback<DateTime> ValueChanged { get; set; }
    [Parameter] public DateTime Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;

            ValueChanged.InvokeAsync(value);
        }
    }

    private DateTime _value = DateTime.Now;
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

    protected override void OnInitialized()
    {
        IJSUtilsService.OnGlobalScroll += OnGlobalScrollAsync;

        _culture = new CultureInfo(Culture);
        Format = string.IsNullOrWhiteSpace(Format) ? _culture.DateTimeFormat.ShortDatePattern : Format;
    }

    public async ValueTask DisposeAsync()
    {
        IJSUtilsService.OnGlobalScroll -= OnGlobalScrollAsync;

        await JsUtilsService.RemoveDocumentListenerAsync<ExtMouseEventArgs>("mousedown", _subscriptionKey);
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

    private async Task OnCalendarClickedAsync()
    {
        var containerRect = await JsUtilsService.GetBoundingClientRectAsync(_containerId);
        _containerPos.Set(containerRect.X, containerRect.Y);
        
        if (Value.Year != _selectedYear) _selectedYear = Value.Year;
        if (Value.Month != _selectedMonth) _selectedMonth = Value.Month;
        
        _showDate = !_showMonth && !_showDate;
        _showMonth = false;
        
        StateHasChanged();
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
}