using Microsoft.AspNetCore.Components;
using Appointment = Bch.Components.Scheduler.Models.Appointment;

namespace Bch.Components.Scheduler.AppointmentItem;

public partial class BchAppointmentItem : ComponentBase
{
    [CascadingParameter] public required BchScheduler OwnerScheduler { get; set; }
    [Parameter] public required RenderFragment<Appointment> ChildContent { get; set; }
    [Parameter] public string Key { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        OwnerScheduler.AddAppointmentItem(this);
    }
}
