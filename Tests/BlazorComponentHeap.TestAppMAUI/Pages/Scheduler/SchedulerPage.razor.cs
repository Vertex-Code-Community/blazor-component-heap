using BlazorComponentHeap.Components.Models.Scheduler;

namespace BlazorComponentHeap.TestAppMAUI.Pages.Scheduler;

public partial class SchedulerPage
{
    private readonly List<Appointment> _appointments = new();

    protected override void OnInitialized()
    {
        _appointments.Add(new Appointment
        {
            Start = new DateTime(2023, 03, 19, 10, 00, 0),
            End = new DateTime(2023, 03, 19, 11, 15, 0),
            BackgroundColor = "rgb(7, 74, 130)",
            LineColor = "red",
            Key = "type-1",
            Payload = "Urok u Iryny"
        });
        
        _appointments.Add(new Appointment
        {
            Start = new DateTime(2023, 03, 03, 11, 30, 0),
            End = new DateTime(2023, 03, 03, 12, 0, 0),
            BackgroundColor = "rgb(7, 74, 130)",
            LineColor = "red",
            Key = "type-1",
            Payload = "Olena"
        });

        _appointments.Add(new Appointment
        {
            Start = new DateTime(2023, 03, 03, 12, 10, 0),
            End = new DateTime(2023, 03, 03, 13, 45, 0),
            BackgroundColor = "rgb(7, 74, 130)",
            LineColor = "red",
            Key = "type-2",
            Payload = 1
        });

        _appointments.Add(new Appointment
        {
            Start = new DateTime(2023, 03, 03, 12, 0, 0),
            End = new DateTime(2023, 03, 03, 13, 0, 0),
            BackgroundColor = "rgb(240, 78, 71)",
            LineColor = "blue",
            Key = "type-1",
            Payload = "Vitaliy"
        });

        _appointments.Add(new Appointment
        {
            Start = new DateTime(2023, 03, 05, 10, 35, 0),
            End = new DateTime(2023, 03, 05, 12, 15, 0),
            BackgroundColor = "rgb(140, 198, 64)",
            LineColor = "blue",
            Key = "type-2",
            Payload = 4
        });
        _appointments.Add(new Appointment
        {
            Start = new DateTime(2023, 03, 09, 10, 25, 0),
            End = new DateTime(2023, 03, 09, 12, 10, 0),
            BackgroundColor = "rgb(125, 66, 245)",
            LineColor = "green",
            Key = "type-1",
            Payload = "Ruslan"
        });
        _appointments.Add(new Appointment
        {
            Start = new DateTime(2023, 03, 10, 14, 10, 0),
            End = new DateTime(2023, 03, 10, 15, 15, 0),
            BackgroundColor = "rgb(223, 107, 0)",
            LineColor = "blue",
            Key = "type-2",
            Payload = 6
        });
    }
}
