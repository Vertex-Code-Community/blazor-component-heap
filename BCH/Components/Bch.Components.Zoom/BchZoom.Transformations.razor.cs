namespace Bch.Components.Zoom;

public partial class BchZoom
{
    public void Transform(float x, float y, float zoomDelta, float angleDelta)
    {
        Transform(x, y, zoomDelta, angleDelta, true);
    }
    
    private void Transform(float x, float y, float zoomDelta, float angleDelta, bool rotation)
    {
        if (rotation)
        {
            _touchDirToPos.Set(x, y).Subtract(_pos);
            _rotatedPoint.Set(_touchDirToPos).Rotate(angleDelta);
        
            _pos.Add(_touchDirToPos.X - _rotatedPoint.X, _touchDirToPos.Y - _rotatedPoint.Y);
            _rotationAngle += angleDelta;
        }
        
        _zoomPoint.Set(x, y);

        // determine the point on where the slide is zoomed in
        _zoomTarget.Set((_zoomPoint.X - _pos.X) / Scale, (_zoomPoint.Y - _pos.Y) / Scale);
        
        // apply zoom
        _scale += zoomDelta * _scaleFactor;
        _scale = Math.Max(_minScale, Math.Min(_maxScale, _scale));
        
        // Console.WriteLine($"ZOOM _scale = {_scale}");

        // calculate x and y based on zoom
        _pos.Set(
            -_zoomTarget.X * Scale + _zoomPoint.X,
            -_zoomTarget.Y * Scale + _zoomPoint.Y
        );

        ApplyConstraints();
        Update();
    }
}