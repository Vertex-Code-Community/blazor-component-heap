using Microsoft.AspNetCore.Components;
using Bch.Modules.Files.Events;

namespace Bch.Components.FileDrop;

[EventHandler("onbchfiledrop", typeof(DropFileEventArgs), true, true)]
[EventHandler("onbchdropdragover", typeof(EventArgs), true, true)]
[EventHandler("onbchdropdragleave", typeof(EventArgs), true, true)]
public static class EventHandlers
{
    
}