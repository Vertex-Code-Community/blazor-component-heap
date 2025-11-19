using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Bch.Modules.Files;
using Bch.Modules.Files.Models;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;
using Bch.Components.InputFile.Events;
using Bch.Modules.DomInterop.Services;
using Bch.Modules.Maths.Models;

namespace Bch.Components.InputFile;

public partial class BchInputFile : ComponentBase
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Inject] public required IDomInteropService DomInteropService { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    [Parameter] public EventCallback<BchFilesContext> OnChange { get; set; }

    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string Width { get; set; } = "290px";
    [Parameter] public string Height { get; set; } = "56px";
    [Parameter] public string Placeholder { get; set; } = "Choose file";

    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }

    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;

    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    private readonly string _inputId = $"_id_{Guid.NewGuid()}";
    private string _inputKey = Guid.NewGuid().ToString();

    private readonly List<BchBrowserFile> _files = new();
    private bool _showSelectedFiles = false;

    private Vec2 _ddlContentPos = new();

    private Task OnChangedInternalAsync(BchFilesOnChangeEvent e)
    {
        _files.Clear();
        _files.AddRange(e.Files.Select(x => new BchBrowserFile
        {
            JsRuntime = JsRuntime,
            Id = x.Id,
            Name = x.Name,
            ContentType = x.ContentType,
            Size = x.Size,
            LastModified = x.LastModified,
            ImagePreviewUrl = x.ImagePreviewUrl
        }).ToList());

        return OnChange.InvokeAsync(new BchFilesContext
        {
            Files = _files.Select(x => x as IBrowserFile).ToList()
        });
    }

    private Task OnClearClickedAsync()
    {
        _inputKey = Guid.NewGuid().ToString();
        _files.Clear();

        return OnChange.InvokeAsync(new BchFilesContext { Files = new() });
    }

    private string GetInfoText()
    {
        return _files.Count switch
        {
            0 => Placeholder,
            1 => _files[0].Name,
            _ => $"Selected {_files.Count} files"
        };
    }

    private async Task OnExpandFilesListAsync()
    {
        if (!_showSelectedFiles)
        {
            var containerRect = await DomInteropService.GetBoundingClientRectAsync(_containerId);
            if (containerRect is null) return;
            _ddlContentPos.Set(containerRect.X, containerRect.Y + containerRect.Height);
        }

        _showSelectedFiles = !_showSelectedFiles;
        StateHasChanged();
    }

    private string GetThemeCssClass()
    {
        var themeSpecified = Theme ?? ThemeCascading;
        return EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) +
               (themeSpecified is null ? " bch-no-theme-specified" : "");
    }
}