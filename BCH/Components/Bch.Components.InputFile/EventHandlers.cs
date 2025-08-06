using Bch.Components.InputFile.Events;
using Microsoft.AspNetCore.Components;

namespace Bch.Components.InputFile;

[EventHandler("onbchonfilechange", typeof(BchFilesOnChangeEvent), true, true)]
public static class EventHandlers
{
    
}