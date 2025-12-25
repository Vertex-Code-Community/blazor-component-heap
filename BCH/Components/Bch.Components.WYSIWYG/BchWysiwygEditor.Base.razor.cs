using System.Globalization;
using System.Text.RegularExpressions;
using Bch.Components.WYSIWYG.Attributes;
using Bch.Components.WYSIWYG.Components.Toolbar.Item;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Bch.Components.WYSIWYG.Events;
using Bch.Components.WYSIWYG.Models;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.Maths.Models;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.WYSIWYG;

public partial class BchWysiwygEditor : IAsyncDisposable
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }

    [Parameter] public EventCallback OnChange { get; set; } // TODO: call also on undo, redo
    [Parameter] public EventCallback OnLoaded { get; set; }
    [Parameter] public EventCallback OnDestruct { get; set; }
    [Parameter] public EventCallback OnReplaceImageClicked { get; set; }
    [Parameter] public EventCallback OnSelectImage { get; set; }
    [Parameter] public EventCallback<BchEditorViewMode> ViewModeChanged { get; set; }
    [Parameter] public EventCallback<BchSelectionEventArgs> OnSelectionChange { get; set; }
    [Parameter] public BchEditorViewMode EditorViewMode { get; set; } = BchEditorViewMode.Preview;
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public bool ContentEditable { get; set; } = true;
    [Parameter] public bool ImageReplacingEnabled { get; set; } = false;
    [Parameter] public bool ShowToolbar { get; set; } = true;
    [Parameter] public required RenderFragment ToolbarItemsTemplate { get; set; }
    [Parameter] public List<BchEditorToolbarItemModel> ToolbarKeys { get; set; } = new()
    {
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Undo).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Redo).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Bold).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Italic).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Underline).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.FontSize).ToLower() },
        new BchEditorToolbarItemModel { Key = "headings" },
        new BchEditorToolbarItemModel { Key = "align" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.ColorText).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = "table" },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.OrderedList).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.UnorderedList).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Indent).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Outdent).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.InsertImage).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.RemoveLink).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.InsertLink).ToLower() },
        new BchEditorToolbarItemModel { Key = nameof(BchEditorControlButton.Separator).ToLower() },
        new BchEditorToolbarItemModel { Key = "mode" }
    };

    private string ContentEditAbility => @$"spellcheck=""false"" autocorrect=""off"" contenteditable=""{(_contentEditable ? "true" : "false")}""";

    private bool _contentEditable = false;
    private BchEditorViewMode _viewMode;
    private readonly string _sourceId = $"_id_{Guid.NewGuid()}";
    private readonly string _previewId = $"_id_{Guid.NewGuid()}";
    private readonly string _key = $"_key_{Guid.NewGuid()}";

    private DotNetObjectReference<BchWysiwygEditor>? _dotNetRef;
    private int _commandButtonsBitMask = 0;
    private bool _KCPressed = false;

    private IJSInProcessRuntime _jsInProcessRuntime = null!;

    private string _colorText = string.Empty;
    private string _paragraph = string.Empty;

    private Vec2 _imageRectPos = new();
    private Vec2 _imageRectSize = new();
    private Vec2 _imageRectSizeBeforeDragging = new();
    private Vec2 _lastMousePosition = new();
    private double _imageRatio = 1.0f;

    private bool _showImageRect = false;
    private bool _rectHandleDragged = false;
    private NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    private readonly int _minRectangleWidth = 50;
    private readonly int _minRectangleHeight = 50;

    private Dictionary<string, RenderFragment> _toolbarItems = new();

    private bool _codeMirrorInitialized = false;
    private bool _iFrameLoaded = false;
    private bool _isLoaded = false;

    private bool _showFloatingToolbar = false;
    private bool _showImageOptions = false;
    private bool _showTableOptions = false;
    private Vec2 _floatingToolbarPos = new();
    private Vec2 _imageOptionsPos = new();
    private Vec2 _tableOptionsPos = new();

    private string _htmlTemplate = BchEditorConstants.EditorTemplate;

    #region ComponentState

    protected override async Task OnInitializedAsync()
    {
        _contentEditable = ContentEditable;
        _jsInProcessRuntime = (IJSInProcessRuntime)JsRuntime;

        _viewMode = EditorViewMode;
        _dotNetRef = DotNetObjectReference.Create(this);

        _htmlTemplate = Regex.Replace(_htmlTemplate, "<body\\s", $"<body {ContentEditAbility}", RegexOptions.IgnoreCase);

        if (!ContentEditable) return;

        await GlobalEventsService.AddDocumentListenerAsync<KeyboardEventArgs>("keydown", _key, OnDocumentKeyDownAsync);
        await GlobalEventsService.AddDocumentListenerAsync<KeyboardEventArgs>("keyup", _key, OnDocumentKeyUpAsync);
        await GlobalEventsService.AddDocumentListenerAsync<KeyboardEventArgs>("mouseup", _key, OnDocumentKeyUpAsync);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initHtmlTextEditorCodeMirror", _sourceId, _previewId, _dotNetRef);
            _codeMirrorInitialized = true;

            if (_iFrameLoaded && !_isLoaded)
            {
                await OnLoaded.InvokeAsync();
                _isLoaded = true;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await JsRuntime.InvokeVoidAsync("destroyHtmlTextEditorCodeMirror", _sourceId);

        if (ContentEditable)
        {
            await GlobalEventsService.RemoveDocumentListenerAsync<KeyboardEventArgs>("keydown", _key);
            await GlobalEventsService.RemoveDocumentListenerAsync<KeyboardEventArgs>("keyup", _key);
        }

        await OnDestruct.InvokeAsync();
    }

    #endregion

    #region EventHandlers

    private async Task OnIFrameLoadedAsync()
    {
        if (ContentEditable)
        {
            await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "selectionchange", _dotNetRef, "OnSelectionChangedAsync");
            await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "paste", null, null);
            await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "keydown", _dotNetRef, "OnIFrameKeyDownAsync");
            await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "mousedown", _dotNetRef, "OnIFrameMouseDownAsync");
            await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "mouseup", _dotNetRef, "OnIFrameMouseUp");
            await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "dragover", _dotNetRef, "OnIFrameFirstDragOverAsync");
            await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "scroll", _dotNetRef, "OnIFrameScroll");
            if (ImageReplacingEnabled) await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "contextmenu", _dotNetRef, "OnIFrameContextMenuAsync");
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("addIFrameDocumentListener", _previewId, _key, "selectionchange", _dotNetRef, "OnSelectionChangedNotEditableAsync");
        }

        _iFrameLoaded = true;

        if (_codeMirrorInitialized && !_isLoaded)
        {
            await Task.Delay(200);
            await OnLoaded.InvokeAsync();
            _isLoaded = true;
        }
    }

    private async Task OnButtonClickedAsync((BchEditorControlButton Button, string? Value) param)
    {
        ResetState();

        switch (param.Button)
        {
            case BchEditorControlButton.Source:
            case BchEditorControlButton.Preview:
                await SwitchViewModeAsync();
                break;
            default:
                await ExecCommandAsync(param.Button, param.Value);
                break;
        }
    }

    private async Task SwitchViewModeAsync()
    {
        var isPreview = _viewMode == BchEditorViewMode.Preview;

        if (isPreview) await JsRuntime.InvokeVoidAsync("setContentToSourceElement", _previewId, _sourceId, ContentEditAbility);
        else await JsRuntime.InvokeVoidAsync("setContentToPreviewElement", _previewId, _sourceId, ContentEditAbility);

        _commandButtonsBitMask &= ~(1 << BchEditorControlButton.Undo.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));
        _commandButtonsBitMask &= ~(1 << BchEditorControlButton.Redo.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));

        _viewMode = isPreview ? BchEditorViewMode.Source : BchEditorViewMode.Preview;
        await ViewModeChanged.InvokeAsync(_viewMode);

        StateHasChanged();
    }

    public async Task SetViewModeAsync(BchEditorViewMode viewMode)
    {
        if (_viewMode == viewMode) return;

        await SwitchViewModeAsync();
    }

    public async Task ExecCommandAsync(BchEditorControlButton button, string? value = null)
    {
        var command = button.GetValue<string, BchEditorCommandAttribute>(x => x.Command);
        var undoRedoState = await JsRuntime.InvokeAsync<BchEditorUndoRedoStateModel>("execCommandOnEditorWithSelection", _previewId, command, value);
        var commandBitMask = _commandButtonsBitMask;

        if (undoRedoState.Undo) commandBitMask |= 1 << BchEditorControlButton.Undo.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift);
        else commandBitMask &= ~(1 << BchEditorControlButton.Undo.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));

        if (undoRedoState.Redo) commandBitMask |= 1 << BchEditorControlButton.Redo.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift);
        else commandBitMask &= ~(1 << BchEditorControlButton.Redo.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));

        if (button is BchEditorControlButton.JustifyLeft or BchEditorControlButton.JustifyRight or BchEditorControlButton.JustifyCenter or BchEditorControlButton.FormatBlock)
        {
            commandBitMask &= ~(1 << BchEditorControlButton.JustifyLeft.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));
            commandBitMask &= ~(1 << BchEditorControlButton.JustifyRight.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));
            commandBitMask &= ~(1 << BchEditorControlButton.JustifyCenter.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));
            commandBitMask &= ~(1 << BchEditorControlButton.FormatBlock.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));

            commandBitMask |= 1 << button.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift);
        }

        if (_commandButtonsBitMask != commandBitMask || (_showImageRect && !_rectHandleDragged || _showFloatingToolbar || _showImageOptions))
        {
            _showImageRect = false;
            _showFloatingToolbar = false;
            _showImageOptions = false;
            _showTableOptions = false;

            _commandButtonsBitMask = commandBitMask;
            StateHasChanged();
        }
    }

    private async Task OnDocumentKeyDownAsync(KeyboardEventArgs e) 
    {
        if (e.CtrlKey && e.Code == "75") // K
        {
            _KCPressed = true;
            return;
        }

        if (e.CtrlKey && _KCPressed)
        {
            switch (e.Code)
            {
                case "67": // C
                    await JsRuntime.InvokeVoidAsync("commentSelectionEditor", _sourceId, true);
                    break;
                case "85": // U
                    await JsRuntime.InvokeVoidAsync("commentSelectionEditor", _sourceId, false);
                    break;
                case "68": // D
                    await JsRuntime.InvokeVoidAsync("autoFormatSelectionEditor", _sourceId);
                    break;
            }

            return;
        }
    }

    private Task OnDocumentKeyUpAsync(KeyboardEventArgs e)
    {
        if (!e.CtrlKey) _KCPressed = false;
        return Task.CompletedTask;
    }

    private async Task OnMouseDownAsync(MouseEventArgs e)
    {
        await GlobalEventsService.AddDocumentListenerAsync<object>("mouseup", _key, OnDocumentMouseUpAsync);
        await GlobalEventsService.AddDocumentListenerAsync<MouseEventArgs>("mousemove", _key, OnMouseMoveAsync);

        _lastMousePosition.Set(e.PageX, e.PageY);
        _rectHandleDragged = true;

        _showFloatingToolbar = false;
        _showImageOptions = false;
        _showTableOptions = false;

        StateHasChanged();
    }

    private async Task OnDocumentMouseUpAsync(object e)
    {
        if (!_rectHandleDragged) return;

        await GlobalEventsService.RemoveDocumentListenerAsync<object>("mouseup", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<MouseEventArgs>("mousemove", _key);

        _rectHandleDragged = false;

        StateHasChanged();
    }

    private async Task OnMouseMoveAsync(MouseEventArgs e)
    {
        if (!_rectHandleDragged) return;

        var dX = e.PageX - _lastMousePosition.X;
        var dY = e.PageY - _lastMousePosition.Y;

        var newX = _imageRectSizeBeforeDragging.X + dX;
        var newY = _imageRectSizeBeforeDragging.Y + dY;

        var w = newX < _minRectangleWidth ? _minRectangleWidth : newX;
        var h = newY < _minRectangleHeight ? _minRectangleHeight : newY;

        h = w / (float)_imageRatio;

        _imageRectSize.Set(w, h);
        await JsRuntime.InvokeVoidAsync("applyEditorImageChange", _previewId, w, h);
    }

    [JSInvokable]
    public Task OnIFrameImgObservedAsync(BchIFrameMouseDownEvent evt)
    {
        if (evt.Rect != null!)
        {
            _imageRectPos.Set(evt.Rect.Left, evt.Rect.Y);
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnIFrameFirstDragOverAsync()
    {
        _showImageRect = false;
        _rectHandleDragged = false;

        StateHasChanged();

        return Task.CompletedTask;
    }

    private void ResetState()
    {
        _showImageRect = false;
        _rectHandleDragged = false;
        _showFloatingToolbar = false;
        _showImageOptions = false;
        _showTableOptions = false;

        StateHasChanged();
    }

    private async Task OnReplaceImageClickedAsync()
    {
        _showFloatingToolbar = false;
        _showImageOptions = false;
        _showTableOptions = false;
        _showImageRect = false;

        await OnReplaceImageClicked.InvokeAsync();

        StateHasChanged();
    }

    #endregion

    internal void AddCustomToolbarItem(BchEditorToolbarItem toolbarItem)
    {
        _toolbarItems.Add(toolbarItem.Key, toolbarItem.ChildContent);
    }

    public async Task<string?> GetValueAsync()
    {
        var isPreview = _viewMode == BchEditorViewMode.Preview;

        return await JsRuntime.InvokeAsync<string?>("getHtmlTextEditorValue", _previewId, _sourceId, isPreview);
    }

    public string? GetValue()
    {
        var isPreview = _viewMode == BchEditorViewMode.Preview;
        return _jsInProcessRuntime.Invoke<string?>("getHtmlTextEditorValue", _previewId, _sourceId, isPreview);
    }

    public async Task SetValueAsync(string value)
    {
        var isPreview = _viewMode == BchEditorViewMode.Preview;

        var valueHasHtmlWrapper = value.StartsWith("<html", StringComparison.OrdinalIgnoreCase);

        if (!valueHasHtmlWrapper)
            value = Regex.Replace(_htmlTemplate, @"(<body[^>]*>)(.*?)(</body>)", m => m.Groups[1].Value + value + m.Groups[3].Value,
                                  RegexOptions.Singleline | RegexOptions.IgnoreCase);
        else 
            value = Regex.Replace(value, "<body\\s", $"<body {ContentEditAbility}", RegexOptions.IgnoreCase);

        await JsRuntime.InvokeAsync<string>("setHtmlTextEditorValue", _previewId, _sourceId, isPreview, value);
    }

    public async Task ReplaceSelectedImageAsync(string imageUrl)
    {
        await JsRuntime.InvokeVoidAsync("replaceSelectedEditorImage", _previewId, imageUrl);
    }

    public BchEditorViewMode ViewMode => _viewMode;
}
