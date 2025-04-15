using System.Reflection;
using BlazorComponentHeap.TestApp.Extensions;
using BlazorComponentHeap.TestApp.Routing.Helpers;
using BlazorComponentHeap.TestApp.Routing.Models;
using BlazorComponentHeap.TestApp.Routing.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorComponentHeap.TestApp.Routing.Router;

public partial class FlexibleRouter : IDisposable
{
    [Inject] public FlexibleNavigationManager NavigationManager { get; set; }
    [Inject] public NavigationManager DefaultNavigationManager { get; set; }
    
    [Parameter] public Assembly AppAssembly { get; set; }
    // [Parameter] public IEnumerable<Assembly> AdditionalAssemblies { get; set; }
    
    [Parameter] public RenderFragment<RouteDataModel>? Found { get; set; }
    [Parameter] public RenderFragment? NotFound { get; set; }

    protected override void OnInitialized()
    {
        var relativePath = DefaultNavigationManager.ToBaseRelativePath(DefaultNavigationManager.Uri);
        
        Console.WriteLine($"relativePath {relativePath}");
        
        var pages = AppAssembly
            .GetTypesWithAttribute<RouteAttribute>()
            .Select(x => new RouteDataModel
            {
                PageType = x,
                Route = UriHelper.Normalize(x.GetValue<string, RouteAttribute>(a => a.Template))
            })
            .ToList();

        var indexPage = pages.FirstOrDefault(x => x.Route == relativePath);
        NavigationManager.CurrentPage = indexPage;

        if (indexPage is not null)
        {
            NavigationManager.History.Add(indexPage);
        }
        
        
        NavigationManager.Pages.AddRange(pages);
        NavigationManager.LocationChanged += StateHasChanged;
    }
    
    public void Dispose()
    {
        NavigationManager.LocationChanged -= StateHasChanged;
    }
}