using Dapr.Client;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;

//dapr run --app-id GrpcClient1 --app-port 5157 --app-protocol grpc  --dapr-grpc-port 50007 -- dotnet run

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();

//https://docs.dapr.io/developing-applications/building-blocks/service-invocation/howto-invoke-services-grpc/
// new grpc ("1")
//客户端不是直接在端口调用目标服务5156，而是通过端口调用其本地 Dapr sidecar 50007
var channel = GrpcChannel.ForAddress("http://localhost:50007");
var client = new Greeter.GreeterClient(channel);
builder.Services.AddSingleton(client);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/hello", async (DaprClient daprClient, Greeter.GreeterClient client) =>
    {
        {
            // [Obsolete("[DEPRECATION NOTICE] InvokeService is deprecated and will be removed in the future, please use proxy mode instead,  new grpc (\"1\")")]
            HelloReply? result1 = await daprClient.InvokeMethodGrpcAsync<HelloRequest, HelloReply>("GrpcService1", nameof(Greeter.GreeterClient.SayHello), new HelloRequest
            {
                Name = "dapr client"
            });
        }
        // new grpc ("1")
        var metadata = new Metadata
        {
            { "dapr-app-id", "GrpcService1" }
        };
        HelloReply? result = await client.SayHelloAsync(new HelloRequest
        {
            Name = "dapr client new"
        },metadata);
        return result.Message;
    })
    .WithName("hello")
    .WithOpenApi();

app.Run();