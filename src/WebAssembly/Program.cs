using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MRVA.Reports.Data.Extensions;
using MRVA.Reports.WebAssembly;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddReportData();
builder.Services.AddMudServices();

var host = builder.Build();

// Fetch and decompress the gzip-compressed SQLite database using the
// browser's native DecompressionStream API (runs in C++, much faster
// than .NET's GZipStream compiled to WASM).
var js = host.Services.GetRequiredService<IJSRuntime>();
var dbModule = await js.InvokeAsync<IJSObjectReference>("import", "./js/db-loader.js");
var dbBytes = await dbModule.InvokeAsync<byte[]>("fetchAndDecompress", "data/mrva-analysis.db.gz");
await host.Services.InitializeReportDataAsync(dbBytes);

await host.RunAsync();
