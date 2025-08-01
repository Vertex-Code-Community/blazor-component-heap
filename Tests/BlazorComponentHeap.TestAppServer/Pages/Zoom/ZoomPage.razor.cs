using BlazorComponentHeap.Scheduler.Models;
using BlazorComponentHeap.Zoom.Models;

namespace BlazorComponentHeap.TestAppServer.Pages.Zoom;

public partial class ZoomPage
{
    private List<Appointment> _appointments = new();
    private ZoomContext _zoomContext = new();

    protected override void OnInitialized()
    {
        _appointments.Add(new Appointment
        {
            Start = new DateTime(2022, 09, 19, 9, 00, 0),
            End = new DateTime(2022, 09, 19, 10, 15, 0),
            BackgroundColor = "rgb(7, 74, 130)",
            LineColor = "red",
            Key = "type-1",
            Payload = "Urok u Iryny"
        });

        _appointments.Add(new Appointment
        {
            Start = new DateTime(2022, 08, 03, 1, 30, 0),
            End = new DateTime(2022, 08, 03, 4, 0, 0),
            BackgroundColor = "rgb(7, 74, 130)",
            LineColor = "red",
            Key = "type-1",
            Payload = "Olena"
        });

        _appointments.Add(new Appointment
        {
            Start = new DateTime(2022, 08, 03, 4, 10, 0),
            End = new DateTime(2022, 08, 03, 6, 45, 0),
            BackgroundColor = "rgb(7, 74, 130)",
            LineColor = "red",
            Key = "type-2",
            Payload = 1
        });

        _appointments.Add(new Appointment
        {
            Start = new DateTime(2022, 08, 03, 1, 0, 0),
            End = new DateTime(2022, 08, 03, 3, 0, 0),
            BackgroundColor = "rgb(240, 78, 71)",
            LineColor = "blue",
            Key = "type-1",
            Payload = "Vitaliy"
        });

        _appointments.Add(new Appointment
        {
            Start = new DateTime(2022, 08, 05, 2, 35, 0),
            End = new DateTime(2022, 08, 05, 5, 15, 0),
            BackgroundColor = "rgb(140, 198, 64)",
            LineColor = "blue",
            Key = "type-2",
            Payload = 4
        });
        _appointments.Add(new Appointment
        {
            Start = new DateTime(2022, 08, 09, 1, 25, 0),
            End = new DateTime(2022, 08, 09, 4, 10, 0),
            BackgroundColor = "rgb(125, 66, 245)",
            LineColor = "green",
            Key = "type-1",
            Payload = "Ruslan"
        });
        _appointments.Add(new Appointment
        {
            Start = new DateTime(2022, 08, 10, 1, 10, 0),
            End = new DateTime(2022, 08, 10, 3, 15, 0),
            BackgroundColor = "rgb(223, 107, 0)",
            LineColor = "blue",
            Key = "type-2",
            Payload = 6
        });


        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 7),
        //    End = new DateTime(2022, 10, 11),
        //    BackgroundColor = "rgba(240, 78, 71, 0.1)",
        //    LineColor = "#F04E47"
        //});
        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 8),
        //    End = new DateTime(2022, 10, 12),
        //    BackgroundColor = "rgba(125, 66, 245, 0.1)",
        //    LineColor = "#7337EE"
        //});
        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 6),
        //    End = new DateTime(2022, 10, 14),
        //    BackgroundColor = "rgba(140, 198, 64, 0.1)",
        //    LineColor = "#8CC640"
        //});
        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 4),
        //    End = new DateTime(2022, 10, 7, 12, 0, 0),
        //    BackgroundColor = "rgba(7, 74, 130, 0.1)",
        //    LineColor = "#074A82"
        //});
        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 8, 12, 0, 0),
        //    End = new DateTime(2022, 10, 12),
        //    BackgroundColor = "rgba(223, 107, 0, 0.1)",
        //    LineColor = "#DF6B00"
        //});
        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 9),
        //    End = new DateTime(2022, 10, 13, 14, 0, 0),
        //    BackgroundColor = "rgba(240, 78, 71, 0.1)",
        //    LineColor = "#F04E47"
        //});
        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 26),
        //    End = new DateTime(2022, 10, 30),
        //    BackgroundColor = "rgba(140, 198, 64, 0.1)",
        //    LineColor = "#8CC640"
        //});
        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 24),
        //    End = new DateTime(2022, 10, 27),
        //    BackgroundColor = "rgba(7, 74, 130, 0.1)",
        //    LineColor = "#074A82"
        //});
        //_appointments.Add(new Appointment
        //{
        //    Start = new DateTime(2022, 10, 25),
        //    End = new DateTime(2022, 10, 30, 12, 0, 0),
        //    BackgroundColor = "rgba(240, 78, 71, 0.1)",
        //    LineColor = "#F04E47"
        //});
    }
}