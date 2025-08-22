namespace Bch.Components.Cropper.Events;

public class ImageLoadedEventArgs : EventArgs
{
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    
    public float Width { get; set; }
    public float Height { get; set; }
}