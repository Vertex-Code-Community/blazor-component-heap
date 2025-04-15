namespace BlazorComponentHeap.Core.Services.Interfaces;

public interface IUpdateCheckerService
{
    bool IsUpdateAvailable { get; }
    Action? OnUpdate { get; set; }
}
