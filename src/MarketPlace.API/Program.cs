using MarketPlace.Api.Extensions;
using MarketPlace.Application.Extensions;
using MarketPlace.Infrastructure;
using MarketPlace.Infrastructure.Extensions;

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

app.UseHttpsRedirection();


app.Run();

