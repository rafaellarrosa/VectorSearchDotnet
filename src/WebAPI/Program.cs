using Microsoft.OpenApi.Models;
using MediatR;
using Infrastructure;
using WebAPI.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vector Search API",
        Version = "v1"
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(typeof(Application.Application).Assembly);

var app = builder.Build();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.EnrichDiagnosticContext = (diagContext, httpContext) =>
        {
            diagContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
            diagContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Vector Search API V1");
        options.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.MapDocumentEndpoints();

app.Run();