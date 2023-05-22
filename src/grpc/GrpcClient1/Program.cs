using Dapr.Client;
using GrpcService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (DaprClient daprClient) =>
    {
        HelloReply? result =
            await daprClient.InvokeMethodGrpcAsync<HelloRequest, HelloReply>("myBackEndGrpc",
                nameof(Greeter.GreeterClient.SayHello),new HelloRequest
                {
                    Name= "dapr client"
                });
        return result.Message;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();