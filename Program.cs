using FightArchive.Data;
using FightArchive.Helpers;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

Log.Information($"Application started on {DateTime.Now.ToShortDateString()} at {DateTime.Now.ToShortTimeString()}.");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IServiceCollection? services = builder.Services;

    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

    builder.Host.UseSerilog((ctx, lc) => lc
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    // Add services to the container.
    services.AddRazorPages().AddNewtonsoftJson();
    services.AddServerSideBlazor();
    services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomEnd;

        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.VisibleStateDuration = 5000;
        config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        config.SnackbarConfiguration.ClearAfterNavigation = false;
    });

    services.AddDbContextFactory<DataContext>(options =>
    {
        options.UseSqlite($"Data Source=./db/{DataContext.FightsDb}.db");
    });

    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        await using AsyncServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
        var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<DataContext>>();
        await DbUtil.InitDb(options);
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();

    app.UseStaticFiles();

    app.UseRouting();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception ex)
{
    if (ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
        throw;
    Log.Fatal($"Fatal error occurred.\n{ex.Message}");
}
finally
{
    Log.Information($"Application closing down  on {DateTime.Now.ToShortDateString()} at {DateTime.Now.ToShortTimeString()}.");
    Log.CloseAndFlush();
}