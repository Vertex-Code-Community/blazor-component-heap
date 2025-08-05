using Bch.Modules.Files.Events;
using Microsoft.AspNetCore.Components;

namespace Bch.Modules.Files;

[EventHandler("onfiledrop", typeof(DropFileEventArgs), true, true)]
[EventHandler("ondropdragover", typeof(EventArgs), true, true)]
[EventHandler("ondropdragleave", typeof(EventArgs), true, true)]
public class EventHandlers
{
    
}