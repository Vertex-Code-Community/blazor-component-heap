namespace BlazorComponentHeap.TestApp.Routing.Helpers;

public class UriHelper
{
    public static string Normalize(string? route)
    {
        if (string.IsNullOrEmpty(route)) return "";

        if (route.IndexOf("/") == 0) route = route.Substring(1);

        var lastIndexOfSlash = route.LastIndexOf("/");
        if (lastIndexOfSlash != -1 && lastIndexOfSlash == route.Length - 1) route = route.Substring(0, route.Length - 1);
        
        return route;
    }
}