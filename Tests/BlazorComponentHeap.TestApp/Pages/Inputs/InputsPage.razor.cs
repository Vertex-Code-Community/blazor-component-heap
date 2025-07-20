using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using BlazorComponentHeap.Files;
using BlazorComponentHeap.Files.Extensions;
using BlazorComponentHeap.Files.Models;

namespace BlazorComponentHeap.TestApp.Pages.Inputs;

public partial class InputsPage
{
    [Inject] public required IBchFilesService BchFilesService { get; set; }

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
}