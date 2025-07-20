using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.Input.Events;

namespace BlazorComponentHeap.Input;

[EventHandler("onbchonfilechange", typeof(BchFilesOnChangeEvent), true, true)]
public static class EventHandlers
{
    
}