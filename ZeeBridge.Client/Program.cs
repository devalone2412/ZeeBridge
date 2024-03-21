using ZeeBridge.Client.Models;
using ZeeBridge.Extenstion;
using ZeeBridge.Interfaces;
using ZeeBridge.Models;
using ZeeBridge.Resources;

namespace ZeeBridge.Client;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddZeeBridge(builder.Configuration, AppDomain.CurrentDomain.GetAssemblies());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", async (IZeeBridgeClient zeeBridgeClient) =>
            {
                var result = await zeeBridgeClient.StartEventWithResult<WeatherRequest>("GetWeatherProcess", new {City = "aaa"}); // This line is not working
                return result;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();
        
        var bpmnResourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");

        app.CreateDeployment()
            .UsingDirectory(bpmnResourcesPath)
            .AddResource("weather.bpmn")
            .Deploy();

        app.Run();
    }
}