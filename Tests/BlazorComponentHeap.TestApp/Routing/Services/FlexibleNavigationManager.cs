using BlazorComponentHeap.TestApp.Routing.Helpers;
using BlazorComponentHeap.TestApp.Routing.Models;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.TestApp.Routing.Services;

public class FlexibleNavigationManager
{
    public Action LocationChanged { get; set; }

    public List<RouteDataModel> History { get; } = new();
    internal List<RouteDataModel> Pages { get; } = new();
    
    internal RouteDataModel? CurrentPage { get; set; }

    private readonly IJSRuntime _jsRuntime;

    public FlexibleNavigationManager(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public void NavigateTo(string route)
    {
        route = UriHelper.Normalize(route);
        
        var page = Pages.FirstOrDefault(x => x.Route == route);
        CurrentPage = page;
        
        if (page is not null) History.Add(page);
        
        LocationChanged.Invoke();

        _jsRuntime.InvokeVoidAsync("navigateToWithoutSaving", route);
    }
}