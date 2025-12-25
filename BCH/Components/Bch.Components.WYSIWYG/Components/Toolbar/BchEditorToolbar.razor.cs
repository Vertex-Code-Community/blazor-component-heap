using System.Globalization;
using Microsoft.AspNetCore.Components;
using Bch.Components.WYSIWYG.Attributes;
using Bch.Components.WYSIWYG.Models;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.WYSIWYG.Components.Toolbar;

public partial class BchEditorToolbar
{
    [Parameter] public BchEditorViewMode ViewMode { get; set; }
    [Parameter] public required List<BchEditorToolbarItemModel> ToolbarKeys { get; set; } = new();
    [Parameter] public EventCallback<(BchEditorControlButton, string?)> OnButtonClick { get; set; }
    [Parameter] public int CommandsActivated { get; set; }
    [Parameter] public string ColorText { get; set; } = string.Empty;
    [Parameter] public string Paragraph { get; set; } = string.Empty;
    [Parameter] public Dictionary<string, RenderFragment> ToolbarItems { get; set; } = new();
    [Parameter] public bool Enabled { get; set; } = true;

    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    private async Task OnButtonClickedAsync(BchEditorControlButton control, string value = null!)
    {
        await OnButtonClick.InvokeAsync((control, value));
    }

    private async Task OnColorTextPickAsync(string? color)
    {
        if (color == ColorText) return;
        await OnButtonClick.InvokeAsync((BchEditorControlButton.ColorText, color));
    }

    private Task OnParagraphSelectedAsync(string? text)
    {
        StateHasChanged();
        if (string.IsNullOrEmpty(text)) return Task.CompletedTask;
        
        var parameters = text switch
        {
            "H1" => "<h1>",
            "H2" => "<h2>",
            "H3" => "<h3>",
            "H4" => "<h4>",
            "H5" => "<h5>",
            "H6" => "<h6>",
            "Code" => "<pre>",
            "Quotation" => "<blockquote>",
            _ => "<p>"
        };

        return OnButtonClick.InvokeAsync((BchEditorControlButton.FormatBlock, parameters));
    }

    private Task OnFontSizeSelectedAsync(string? fontSize)
    {
        StateHasChanged();
        return string.IsNullOrEmpty(fontSize) ? Task.CompletedTask : OnButtonClick.InvokeAsync((BchEditorControlButton.FontSize, fontSize));
    }

    private async Task OnTableSelectedAsync(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        var index = BchEditorConstants.Colors.IndexOf(text);

        var n = (index / 10) + 1;
        var m = (index % 10) + 1;

        var tableHtml = "<table style=\"width: 100%;min-width: 0px;\">";

        var cellWidth = 100.0f / m;

        for (var i = 0; i < n; i++)
        {
            tableHtml += "<tr>";

            for (var j = 0; j < m; j++)
            {
                tableHtml += $"<td style=\"width: {cellWidth.ToString(_nF)}%; border: 1px solid #BDBDBD; height: 20px; vertical-align: middle; padding: 2px 5px; min-width: 20px;\"></td>";
            }

            tableHtml += "</tr>";
        }

        tableHtml += "</table>";

        await OnButtonClickedAsync(BchEditorControlButton.InsertHTML, tableHtml);
    }

    private string GetParagraphText()
    {
        return Paragraph switch
        {
            "<h1>" => "H1",
            "<h2>" => "H2",
            "<h3>" => "H3",
            "<h4>" => "H4",
            "<h5>" => "H5",
            "<h6>" => "H6",
            "<pre>" => "Code",
            "<blockquote>" => "Quotation",
            _ => "Paragraph"
        };
    }

    private bool IsActive(BchEditorControlButton button)
    {
        return ViewMode == BchEditorViewMode.Preview &&
            IsKthBitSet(CommandsActivated, button.GetValue<int, BchEditorCommandAttribute>(x => x.BitShift));
    }

    private async Task OnAlignTextSelectedAsync(string? text)
    {
        StateHasChanged();
        if (string.IsNullOrEmpty(text)) return;
        
        var control = text switch
        {
            "left" => BchEditorControlButton.JustifyLeft,
            "right" => BchEditorControlButton.JustifyRight,
            "justify" => BchEditorControlButton.JustifyCenter,
            _ => BchEditorControlButton.FormatBlock
        };

        if (IsActive(control)) return;

        await OnButtonClickedAsync(control);
    }

    private string GetActiveAlignment()
    {
        if (IsActive(BchEditorControlButton.JustifyLeft)) return "left";
        if (IsActive(BchEditorControlButton.JustifyRight)) return "right";
        if (IsActive(BchEditorControlButton.JustifyCenter)) return "justify";
        return "center";
    }

    private bool IsKthBitSet(int n, int k) => (n & (1 << k)) != 0;
}