using System.Globalization;
using FinCalc.Endpoints;
using FinCalc.Services;
using FinCalc.Strategies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Default UI language/culture: Bulgarian (Bulgaria)
CultureInfo defaultCulture = new CultureInfo("bg-BG");
CultureInfo[] supportedCultures =
[
    new CultureInfo("bg-BG"),
    new CultureInfo("en-US")
];

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntiforgery();

// Calculation layer (Strategy + Factory)
builder.Services.AddSingleton<ICalculationStrategy, CreditCalculatorStrategy>();
builder.Services.AddSingleton<ICalculationStrategy, RefinancingCalculatorStrategy>();
builder.Services.AddSingleton<ICalculationStrategy, LeasingCalculatorStrategy>();
builder.Services.AddSingleton<ICalculationStrategyFactory, CalculationStrategyFactory>();
builder.Services.AddSingleton<ICalculationService, CalculationService>();

builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();

app.UseAntiforgery();

app.MapHealthChecks("/health");
app.MapCalculationEndpoints();

app.MapRazorComponents<FinCalc.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
