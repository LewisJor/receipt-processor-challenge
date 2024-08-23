using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SimpleReceiptProcessor.Controllers.Converters;
using SimpleReceiptProcessor.Db;

var builder = WebApplication.CreateBuilder(args);

// add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// add db
builder.Services.AddDbContext<ReceiptsDbContext>(options =>
    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

// add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Simple Receipt Processor",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Receipt Processor V1");
    });}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();