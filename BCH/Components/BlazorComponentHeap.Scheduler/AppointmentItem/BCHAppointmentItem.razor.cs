using BlazorComponentHeap.Core.Models.Scheduler;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Scheduler.AppointmentItem;

public partial class BCHAppointmentItem : ComponentBase
{
    [CascadingParameter]
    public BCHScheduler OwnerScheduler { get; set; } = null!;

    [Parameter]
    public RenderFragment<Appointment> ChildContent { get; set; } = null!;

    [Parameter]
    public string Key { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        OwnerScheduler.AddAppointmentItem(this);
    }
}
