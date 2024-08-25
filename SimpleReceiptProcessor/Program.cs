using System.Reflection;
using SimpleReceiptProcessor.Controllers.Converters;

var builder = WebApplication.CreateBuilder(args);

// add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// add controllers
builder.Services.AddControllers()    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new CustomTimeSpanConverter());
});

// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// enable swagger
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Receipt Processor V1"); });

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();