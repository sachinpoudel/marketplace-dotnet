using MarketPlace.Api.Extensions;
using MarketPlace.Application.Extensions;
using MarketPlace.Infrastructure;
using MarketPlace.Infrastructure.Extensions;
using Serilog;




Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger();


try {
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);
    
    
    
    builder.Services.AddInfrastructure();
    builder.Services.AddDatabase();
    builder.Services.AddApplication();
    builder.AddPresentation();
    builder.Services.AddOpenApi();
    
    var app = builder.Build();
    
    
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    app.UseRouting();
    app.MapControllers();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapGet("/", () => "Hello World!");
    
    app.Run();
    
 
}catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
