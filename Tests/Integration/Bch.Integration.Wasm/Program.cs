using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Bch.Modules.DomInterop.Extensions;
using Bch.Modules.Files.Extensions;
using Bch.Modules.GlobalEvents.Extensions;
using Bch.Components.Modal.Extensions;
using Bch.Components.Modal.Root;
using Bch.Integration.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<BchRootModal>("#bch-modal");

var services = builder.Services;

services.AddBchModal();
services.AddBchFiles();
services.AddBchDomInterop();
services.AddBchGlobalEvents();

await builder.Build().RunAsync();