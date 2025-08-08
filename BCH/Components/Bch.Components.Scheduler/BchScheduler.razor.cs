using System.Globalization;
using Microsoft.AspNetCore.Components;
using Bch.Components.Scheduler.AppointmentItem;
using Bch.Modules.DomInterop.Services;
using Bch.Modules.Maths.Extensions;
using Appointment = Bch.Components.Scheduler.Models.Appointment;
using Day = Bch.Components.Scheduler.Models.Day;
using TimeIntersectionGroup = Bch.Components.Scheduler.Models.TimeIntersectionGroup;

namespace Bch.Components.Scheduler;

public partial class BchScheduler
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }

    [Parameter] public string Gap { get; set; } = "4px";
    [Parameter] public string ItemHeight { get; set; } = "90px";
    [Parameter] public DateTime WeekDate { get; set; } = DateTime.Now;
    [Parameter] public List<Appointment> Appointments { get; set; } = new();
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public bool ShowDatePicker { get; set; } = false;
    [Parameter] public bool ShowTodayButton { get; set; } = false;

    [Parameter] public Func<int, string> TimeLabel { get; set; } = (hour) => $"{(hour < 10 ? "0" : "")}{hour}:00";
    [Parameter] public Func<DateTime, string> DayLabel { get; set; } = (d) => d.DayOfWeek.ToString()[..3];
    [Parameter] public Func<DateTime, string> MonthLabel { get; set; } = (d) => d.ToString("MMMM");

    [Parameter] public string BackgroundColor { get; set; } = "#EEEEEE45";
    [Parameter] public string WorkingAreaColor { get; set; } = "#FFFFFF";
    [Parameter] public bool ScrollToWorkingArea { get; set; } = true;
    [Parameter] public int WorkingAreaStartHour { get; set; } = 10;
    [Parameter] public int WorkingAreaEndHour { get; set; } = 19;

    private DateTime _currentWeekStart;
    private DateTime _calendarDateTime;
    private int _scrollIndex = 0;

    private Dictionary<string, BchAppointmentItem> _appointmentTemplates = new();
    private Dictionary<DateTime, Day> _days = new();

    private readonly NumberFormatInfo _numberFormatWithDot = new() { NumberDecimalSeparator = "." };
    private readonly string _contentScrollId = $"_id_{Guid.NewGuid()}";
    private readonly string _contentScrollerId = $"_id_{Guid.NewGuid()}";

    protected override void OnInitialized()
    {
        _currentWeekStart = WeekDate.StartOfWeek();
        _calendarDateTime = _currentWeekStart;

        Update();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && ScrollToWorkingArea)
        {
            var scrollerRect = await DomInteropService.GetBoundingClientRectAsync(_contentScrollerId);
            if (scrollerRect is not null)
            {
                var cellHeightInPixels = (int) (scrollerRect.Height / 24);
                await DomInteropService.ScrollToAsync(_contentScrollId, "0", $"{(cellHeightInPixels * WorkingAreaStartHour)}", "auto");
            }
        }
    }

    public void Update()
    {
        foreach (var appointment in Appointments)
        {
            var dateCounter = appointment.Start.Date;

            do
            {
                _days.TryGetValue(dateCounter, out var day);

                if (day is null)
                {
                    day = new Day();
                    _days.Add(dateCounter, day);
                }

                if (!day.Groups.Any(x => x.Appointments.Contains(appointment)))
                {
                    day.Groups.Add(new TimeIntersectionGroup
                    {
                        Start = appointment.Start,
                        End = appointment.End,
                        Appointments = new List<Appointment>
                        {
                            appointment
                        }
                    });
                }

                dateCounter = dateCounter.AddDays(1);
            }
            while (dateCounter <= appointment.End.Date);
        }

        foreach (var keyValue in _days)
        {
            var day = keyValue.Value;

            DetectGroups(day.Groups);

            foreach (var group in day.Groups)
            {
                group.Start = group.Start.AddDays(1);
                group.Appointments = group.Appointments.OrderByDescending(x => x.End - x.Start).ToList();
            }
        }

        StateHasChanged();
    }

    internal void AddAppointmentItem(BchAppointmentItem item)
    {
        _appointmentTemplates.Add(item.Key, item);
        StateHasChanged();
    }

    private void DetectGroups(List<TimeIntersectionGroup> groups)
    {
        for (var i = 0; i < groups.Count; i++)
        {
            var group = groups[i];

            for (var j = i + 1; j < groups.Count; j++)
            {
                if (!MergeGroups(group, groups[j])) continue;
                
                groups.Remove(groups[j]);
                i = -1;
                break;
            }
        }
    }

    private bool MergeGroups(TimeIntersectionGroup group1, TimeIntersectionGroup group2)
    {
        if ((group1.Start <= group2.End && group1.Start >= group2.Start) || (group2.Start <= group1.End && group2.Start >= group1.Start))
        {
            group1.Start = group2.Start < group1.Start ? group2.Start : group1.Start;
            group1.End = group2.End > group1.End ? group2.End : group1.End;

            group1.Appointments.AddRange(group2.Appointments);

            return true;
        }

        return false;
    }

    private void OnWeekButtonClicked(bool isRight)
    {
        _scrollIndex += isRight ? 1 : -1;
        _calendarDateTime = _currentWeekStart.AddDays(7 * _scrollIndex).StartOfWeek();

        StateHasChanged();
    }

    private float GetTimeNumber(DateTime dateTime)
    {
        return dateTime.Hour + (dateTime.Minute / 60.0f) + (dateTime.Second / 60.0f) * 0.1f;
    }

    private void OnDateChanged(DateTime? dateTime)
    {
        _calendarDateTime = dateTime ?? DateTime.Now;
        _scrollIndex = (_calendarDateTime.StartOfWeek() - _currentWeekStart.StartOfWeek()).Days / 7;

        StateHasChanged();
    }

    private void OnTodayClicked() => OnDateChanged(DateTime.Now);
}
