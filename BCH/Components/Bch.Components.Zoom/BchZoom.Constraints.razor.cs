using Bch.Components.Zoom.Models;
using Microsoft.AspNetCore.Components;

namespace Bch.Components.Zoom;

public partial class BchZoom
{
    // Expose whether current transform is constrained by containment
    // Per-side constrained flags (true when the respective edge touches the viewport)
    [Parameter] public EventCallback<bool> IsLeftConstrainedChanged { get; set; }
    [Parameter] public bool IsLeftConstrained { get => _isLeftConstrained; set { } }
    [Parameter] public EventCallback<bool> IsRightConstrainedChanged { get; set; }
    [Parameter] public bool IsRightConstrained { get => _isRightConstrained; set { } }
    [Parameter] public EventCallback<bool> IsTopConstrainedChanged { get; set; }
    [Parameter] public bool IsTopConstrained { get => _isTopConstrained; set { } }
    [Parameter] public EventCallback<bool> IsBottomConstrainedChanged { get; set; }
    [Parameter] public bool IsBottomConstrained { get => _isBottomConstrained; set { } }
    
    private bool _isLeftConstrained = false;
    private bool _isRightConstrained = false;
    private bool _isTopConstrained = false;
    private bool _isBottomConstrained = false;
    
    private void ApplyConstraints()
    {
        if (Constraint == ConstraintType.None) return;

        switch (Constraint)
        {
            case ConstraintType.Outside:
                ApplyOutsideConstraints();
                break;
            case ConstraintType.Inside:
                ApplyInsideConstraints();
                break;
        }
    }

    private void ApplyOutsideConstraints()
    {
        if (_navigationSize.X <= 0 || _navigationSize.Y <= 0 || _viewPortSize.X <= 0 || _viewPortSize.Y <= 0) return;

        // Use un-transformed content size (offset) to compute cover scale
        var baseContentW = _navigationOffsetSize.X > 0 ? _navigationOffsetSize.X : _navigationSize.X;
        var baseContentH = _navigationOffsetSize.Y > 0 ? _navigationOffsetSize.Y : _navigationSize.Y;
        var minCoverScaleX = _viewPortSize.X / baseContentW;
        var minCoverScaleY = _viewPortSize.Y / baseContentH;
        var minCoverScale = MathF.Max(minCoverScaleX, minCoverScaleY);

        // Convert to internal _scale domain: _scaleLinear = ln(Scale) + 4
        var minCoverInternal = (float)Math.Log(Math.Max(minCoverScale, 0.0001f)) + 4.0f;

        // Raise MinScale if needed
        if (minCoverInternal > MinScale)
        {
            MinScale = Math.Min(minCoverInternal, MaxScale);
        }

        // Clamp current _scale to at least MinScale
        if (_scale < MinScale) _scale = MinScale;

        // Clamp translation so content never leaves viewport
        var currentScale = Scale; // linear
        var contentW = baseContentW * currentScale;
        var contentH = baseContentH * currentScale;

        var minX = _viewPortSize.X - contentW; // most negative allowed (content right edge flush)
        var minY = _viewPortSize.Y - contentH; // most negative allowed (content bottom edge flush)
        if (float.IsNaN(minX) || float.IsNaN(minY)) return;

        // If content smaller (due to rounding), center it
        if (contentW <= _viewPortSize.X + 0.5f)
        {
            _pos.X = (_viewPortSize.X - contentW) * 0.5f;
        }
        else
        {
            // Clamp to keep both left (<=0) and right (>=minX) edges within viewport
            _pos.X = MathF.Max(minX, MathF.Min(0, _pos.X));
        }

        if (contentH <= _viewPortSize.Y + 0.5f)
        {
            _pos.Y = (_viewPortSize.Y - contentH) * 0.5f;
        }
        else
        {
            // Clamp to keep both top (<=0) and bottom (>=minY) edges within viewport
            _pos.Y = MathF.Max(minY, MathF.Min(0, _pos.Y));
        }

        // Publish constrained state (overall and per-side)
        const float eps = 0.5f;
        bool leftLocked = contentW <= _viewPortSize.X + eps || MathF.Abs(_pos.X - 0) <= eps;
        bool rightLocked = contentW <= _viewPortSize.X + eps || MathF.Abs(_pos.X - minX) <= eps;
        bool topLocked = contentH <= _viewPortSize.Y + eps || MathF.Abs(_pos.Y - 0) <= eps;
        bool bottomLocked = contentH <= _viewPortSize.Y + eps || MathF.Abs(_pos.Y - minY) <= eps;

        if (leftLocked != _isLeftConstrained)
        {
            _isLeftConstrained = leftLocked;
            IsLeftConstrainedChanged.InvokeAsync(_isLeftConstrained);
        }

        if (rightLocked != _isRightConstrained)
        {
            _isRightConstrained = rightLocked;
            IsRightConstrainedChanged.InvokeAsync(_isRightConstrained);
        }

        if (topLocked != _isTopConstrained)
        {
            _isTopConstrained = topLocked;
            IsTopConstrainedChanged.InvokeAsync(_isTopConstrained);
        }

        if (bottomLocked != _isBottomConstrained)
        {
            _isBottomConstrained = bottomLocked;
            IsBottomConstrainedChanged.InvokeAsync(_isBottomConstrained);
        }
    }

    private void ApplyInsideConstraints()
    {
        
    }
}