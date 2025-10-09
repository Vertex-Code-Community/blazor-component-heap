using Bch.Modules.DomInterop.Models;

namespace Bch.Modules.DomInterop.Services;

public interface IDomInteropService
{
    BoundingClientRect? GetBoundingClientRect(string id);
    Task<BoundingClientRect?> GetBoundingClientRectAsync(string id);
    Task ScrollToAsync(string id, string x, string y, string behavior = "smooth"); // auto
    ValueTask FocusAsync(string elementId);
    Task<WindowSize> GetWindowSizeAsync();
}