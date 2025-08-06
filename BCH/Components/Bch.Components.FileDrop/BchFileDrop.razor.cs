using Microsoft.AspNetCore.Components;
using Bch.Modules.Files.Events;

namespace Bch.Components.FileDrop;

public partial class BchFileDrop
{
    [Parameter] public required RenderFragment ChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public EventCallback<DropFileEventArgs> OnFileDrop { get; set; }

    [Parameter] public EventCallback<bool> IsDraggingChanged { get; set; }
    [Parameter] public bool IsDragging
    {
        get => _isDraggingOver;
        set
        {
            if (value == _isDraggingOver) return; 
            _isDraggingOver = value;
            IsDraggingChanged.InvokeAsync(value);
        }
    }
    
    private bool _shouldRender = true;
    private bool _isDraggingOver = false;
    
    protected override bool ShouldRender()
    {
        var shouldRender = _shouldRender;
        _shouldRender = true;

        return shouldRender;
    }
    
    private void OnDragOver(EventArgs e)
    {
        _shouldRender = false;

        if (!_isDraggingOver)
        {
            _shouldRender = true;
            IsDragging = true;
        }
    }

    private void OnDragLeave(EventArgs e)
    {
        _shouldRender = false;
        
        if (_isDraggingOver)
        {
            _shouldRender = true;
            IsDragging = false;
        }
    }
}