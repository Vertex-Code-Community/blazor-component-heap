using Microsoft.AspNetCore.Components;
using Bch.Components.Cropper.Events;

namespace Bch.Components.Cropper;

[EventHandler("onimageloaded", typeof(ImageLoadedEventArgs), true, true)]
public static class EventHandlers
{
}