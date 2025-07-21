using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using ClientLibrary.Services.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using ClientLibrary.Services.Contracts;
using ClientLibrary.Services.Implementations;
using Blazored.LocalStorage;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor;
using Client.ApplicationStates;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddHttpClient("SystemApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7295/");
}).AddHttpMessageHandler<CustomHttpHandler>(); 
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7295/") });
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<JsLogger>();
builder.Services.AddScoped<GetHttpClient>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();

// syncfusion
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCdkx0RXxbf1x1ZFRHal9STndYUiweQnxTdEBjXn1fcXVWQmFeVkRzW0leYw==");
builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<SfDialogService>();

// States
builder.Services.AddScoped<DepartmentState>();

await builder.Build().RunAsync();
