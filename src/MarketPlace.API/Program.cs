using MarketPlace.Api.Extensions;
using MarketPlace.Application.Extensions;
using MarketPlace.Infrastructure;
using MarketPlace.Infrastructure.Extensions;
using MarketPlace.Infrastructure.Identity.Services;
using Serilog;




Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger();


try
{
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);



    builder.Services.AddInfrastructure();
    builder.Services.AddDatabase();
    builder.Services.AddApplication();
    builder.AddPresentation();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    var seedIdentityRoles = builder.Configuration.GetValue("Identity:SeedRolesOnStartup", true);

    if (seedIdentityRoles)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<MarketPlace.Infrastructure.Identity.Services.IdentitySeeder>();
        await seeder.SeedAsync(CancellationToken.None);
    }
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapGet("/", () => "Hello World!");




    app.Run();



}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
