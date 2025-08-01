using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.Files.Events;

namespace BlazorComponentHeap.Files;

[EventHandler("onfiledrop", typeof(DropFileEventArgs), true, true)]
[EventHandler("ondropdragover", typeof(EventArgs), true, true)]
[EventHandler("ondropdragleave", typeof(EventArgs), true, true)]
public class EventHandlers
{
    
}