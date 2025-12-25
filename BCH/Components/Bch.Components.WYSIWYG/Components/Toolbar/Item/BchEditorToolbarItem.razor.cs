using Microsoft.AspNetCore.Components;

namespace Bch.Components.WYSIWYG.Components.Toolbar.Item;

public class BchEditorToolbarItem : ComponentBase
{
    [Parameter] public string Key { get; set; } = string.Empty;
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;

    [CascadingParameter(Name = "HtmlTextEditorComponent")] public required BchWysiwygEditor OwnerContainer { get; set; }

    protected override void OnInitialized()
    {
        OwnerContainer.AddCustomToolbarItem(this);
    }
}