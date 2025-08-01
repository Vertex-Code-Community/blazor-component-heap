using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.Files;
using BlazorComponentHeap.Files.Events;
using BlazorComponentHeap.Files.Extensions;
using BlazorComponentHeap.Files.Models;

namespace BlazorComponentHeap.TestApp.Pages.Inputs;

public partial class InputsPage
{
    [Inject] public required IBchFilesService BchFilesService { get; set; }
    
    private bool _isDraggingOver;
    private string? _draggedImage;

    private async Task OnRequestFileAsync()
    {
        var fileContext = await BchFilesService.RequestFileDialogAsync(multiple: true, createImagePreview: true);

        foreach (var file in fileContext.Files)
        {
            var imagePreview = file.GetImagePreview();
        }
    }
    
    private Task OnChangeAsync(BchFilesContext context)
    {
        Console.WriteLine($"Files Count: {context.Files.Count}");
        
        foreach (var file in context.Files)
        {
            var imagePreview = file.GetImagePreview();
            Console.WriteLine($"Image Preview: {imagePreview}");
        }

        return Task.CompletedTask;
    }

    private Task OnFileDropAsync(DropFileEventArgs e)
    {
        var context = BchFilesService.GetContextFromDropEvent(e);
        
        foreach (var file in context.Files)
        {
            Console.WriteLine($"File Name: {file.Name}");
            var imagePreview = file.GetImagePreview();
            _draggedImage = imagePreview;
            Console.WriteLine($"Image Preview: {imagePreview}");
        }
        
        StateHasChanged();
        
        return Task.CompletedTask;
    }
}