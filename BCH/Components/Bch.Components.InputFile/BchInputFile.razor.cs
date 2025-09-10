using Bch.Components.InputFile.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Bch.Modules.Files;
using Bch.Modules.Files.Models;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;
using Microsoft.AspNetCore.Components.Web;

namespace Bch.Components.InputFile;

public partial class BchInputFile : ComponentBase
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public EventCallback<BchFilesContext> OnChange { get; set; }

    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public int Height { get; set; } = 56;
    [Parameter] public int Width { get; set; } = 290;
    [Parameter] public string Placeholder { get; set; } = "Choose file";
    [Parameter] public bool CreateImagePreview { get; set; } = false;
    [Parameter] public bool DropZoneOnly { get; set; } = false;

    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }

    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;

    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    private readonly string _inputId = $"_id_{Guid.NewGuid()}";
    private readonly string _cssKey = $"_cssKey_{Guid.NewGuid()}";
    private string _inputKey = Guid.NewGuid().ToString();

    private ElementReference _fileInputRef;
    private IDictionary<string, object> _inputAttributes = new Dictionary<string, object>();

    private bool _hasFile = false;
    private string _fileName = string.Empty;
    private bool _isDraggingOver = false;
    private string _fileExtensionCssClass = "ext-generic";

    protected override void OnParametersSet()
    {
        _inputAttributes = AdditionalAttributes is null
            ? new Dictionary<string, object>()
            : new Dictionary<string, object>(AdditionalAttributes);

        if (_inputAttributes.ContainsKey("multiple"))
            _inputAttributes.Remove("multiple");

        if (CreateImagePreview)
            _inputAttributes["create-image-preview"] = string.Empty;
        else if (_inputAttributes.ContainsKey("create-image-preview"))
            _inputAttributes.Remove("create-image-preview");
    }

    private async Task OnChangedInternalAsync(BchFilesOnChangeEvent e)
    {
        var files = e.Files;
        if (files is { Count: > 0 })
        {
            var x = files[0];
            _hasFile = true;
            _fileName = x.Name;
            _fileExtensionCssClass = GetExtensionCssClass(_fileName);

            var selected = new BchBrowserFile
            {
                JsRuntime = JsRuntime,
                Id = x.Id,
                Name = x.Name,
                ContentType = x.ContentType,
                Size = x.Size,
                LastModified = x.LastModified,
                ImagePreviewUrl = x.ImagePreviewUrl
            } as IBrowserFile;

            await OnChange.InvokeAsync(new BchFilesContext
            {
                Files = new List<IBrowserFile> { selected }
            });
        }
        else
        {
            _hasFile = false;
            _fileName = string.Empty;
            _fileExtensionCssClass = "ext-generic";
            await OnChange.InvokeAsync(new BchFilesContext { Files = new List<IBrowserFile>() });
        }

        StateHasChanged();
    }

    private Task OnSelectClickedAsync()
    {
        return Task.CompletedTask;
    }

    private Task OnClearClickedAsync()
    {
        _hasFile = false;
        _fileName = string.Empty;
        _fileExtensionCssClass = "ext-generic";

        _inputKey = Guid.NewGuid().ToString();

        return OnChange.InvokeAsync(new BchFilesContext { Files = new List<IBrowserFile>() });
    }

    private string GetThemeCssClass()
    {
        var themeSpecified = Theme ?? ThemeCascading;
        return EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) +
               (themeSpecified is null ? " bch-no-theme-specified" : "");
    }

    private void OnDragEnter(DragEventArgs _)
    {
        _isDraggingOver = true;
        StateHasChanged();
    }

    private void OnDragOver(DragEventArgs _)
    {
        if (!_isDraggingOver)
        {
            _isDraggingOver = true;
            StateHasChanged();
        }
    }

    private void OnDragLeave(DragEventArgs _)
    {
        _isDraggingOver = false;
        StateHasChanged();
    }

    private void OnDrop(DragEventArgs _)
    {
        _isDraggingOver = false;
        StateHasChanged();
    }

    private static string GetExtensionCssClass(string fileName)
    {
        try
        {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(ext))
                return "ext-generic";

            ext = ext.TrimStart('.').ToLowerInvariant();

            return ext switch
            {
                "png" or "jpg" or "jpeg" or "gif" or "bmp" or "webp" or "svg" or "ico" or "tif" or "tiff" or "heic" or "heif" => "ext-img",
                "zip" or "rar" or "7z" or "gz" or "gzip" or "tar" or "tgz" or "bz" or "bz2" or "xz" or "pkg" or "deb" or "rpm" or "msi" => "ext-zip",
                "txt" or "doc" or "docx" or "csv" or "log" or "json" or "jsonl" or "ndjson" or "xml" or "md" or "yaml" or "yml" or "rtf" or "odt" => "ext-txt",
                "xls" or "xlsx" or "ods" or "tsv" => "ext-xls",
                "ppt" or "pptx" or "odp" => "ext-ppt",
                "pdf" => "ext-pdf",
                "mp3" or "wav" or "flac" or "aac" or "ogg" or "oga" or "m4a" or "aiff" or "alac" or "wma" or "opus" or "mid" or "midi" => "ext-audio",
                "mp4" or "mov" or "m4v" or "avi" or "mkv" or "webm" or "wmv" or "mpg" or "mpeg" or "3gp" or "3g2" or "flv" => "ext-video",
                "cs" or "ts" or "tsx" or "js" or "jsx" or "java" or "kt" or "kts" or "swift" or "go" or "rs" or "py" or "rb" or "php" or "scala" or "sh" or "bash" or "ps1" or "lua" or "hs" or "r" or "sql" or "pl" or "m" or "mm" => "ext-code",
                "bat" or "cmd" or "vbs" or "ps1" or "psm1" or "psd1" => "ext-script",
                "ai" or "eps" or "cdr" => "ext-vector",
                "fig" or "sketch" or "xd" => "ext-design",
                "ttf" or "otf" or "woff" or "woff2" or "eot" => "ext-font",
                "sqlite" or "db" or "db3" or "sqlitedb" or "mdb" or "accdb" or "sas7bdat" or "parquet" or "feather" or "orc" or "avro" or "dta" or "sav" or "rds" => "ext-db",
                "csv.gz" or "tsv.gz" or "parquet" or "feather" or "h5" or "hdf5" => "ext-data",
                "epub" or "mobi" or "azw" or "azw3" or "fb2" or "djvu" => "ext-ebook",
                "obj" or "stl" or "fbx" or "gltf" or "glb" or "dae" => "ext-3d",
                "dwg" or "dxf" or "step" or "stp" or "iges" or "igs" => "ext-cad",
                "exe" or "msi" or "apk" or "ipa" or "app" or "bin" or "dmg" => "ext-exe",
                "iso" or "img" or "dmg" => "ext-disc",
                "srt" or "vtt" or "ass" or "ssa" or "sub" => "ext-sub",
                "ipynb" or "rmd" or "qmd" => "ext-notebook",
                "cr2" or "nef" or "arw" or "orf" or "rw2" or "raf" or "dng" => "ext-raw",
                "torrent" => "ext-torrent",
                "cer" or "crt" or "pem" or "der" or "p7b" or "pfx" or "p12" or "key" => "ext-cert",
                "shp" or "shx" or "dbf" or "geojson" or "kml" or "kmz" or "gpx" or "tif" or "tiff" => "ext-gis",
                _ => "ext-generic"
            };
        }
        catch
        {
            return "ext-generic";
        }
    }
}