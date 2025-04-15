using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorComponentHeap.TestApp.Pages.Test.Popup;

public class PopupComponent : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, "<h1>Hello, world!</h1>\r\n\r\nWelcome to your new app.\r\n\r\n");
        //builder.RootCo
        //builder.OpenComponent<MyFirstBlazorApp.Client.Shared.SurveyPrompt>(1);
        //builder.AddAttribute(2, "Title", "How is Blazor working for you?");
        //builder.CloseComponent();
    }
}
