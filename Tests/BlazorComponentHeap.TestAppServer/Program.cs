using Bch.Components.Modal.Extensions;
using Bch.Modules.DomInterop.Extensions;
using Bch.Modules.GlobalEvents.Extensions;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddBchModal();
services.AddBchDomInterop();
services.AddBchGlobalEvents();

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
