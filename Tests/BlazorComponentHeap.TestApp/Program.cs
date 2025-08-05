using Bch.Components.Modal.Extensions;
using Bch.Components.Modal.Root;
using Bch.Modules.DomInterop.Extensions;
using Bch.Modules.Files.Extensions;
using Bch.Modules.GlobalEvents.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorComponentHeap.TestApp;
using BlazorComponentHeap.TestApp.Routing.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<BchRootModal>("body::after");

var services = builder.Services;

services.AddScoped<FlexibleNavigationManager>();
services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

services.AddBchModal();
services.AddBchFiles();
services.AddBchDomInterop();
services.AddBchGlobalEvents();

await builder.Build().RunAsync();