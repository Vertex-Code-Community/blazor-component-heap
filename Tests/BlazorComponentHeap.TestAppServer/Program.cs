using BlazorComponentHeap.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddBCHComponents(); 
services.AddRazorPages();
services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
