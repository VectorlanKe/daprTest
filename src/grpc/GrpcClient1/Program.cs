using Dapr.Client;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;

//dapr run --app-id GrpcClient1 --app-port 5157 --app-protocol grpc  --dapr-grpc-port 50007 -- dotnet run
//dapr run --app-id GrpcClient1 --app-port 5157 --app-protocol grpc  --dapr-grpc-port 50007 -- dotnet GrpcClient1.dll http://localhost:50007  --urls=http://*:5157


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();

//https://docs.dapr.io/developing-applications/building-blocks/service-invocation/howto-invoke-services-grpc/
// new grpc ("proxy mode")
//客户端不是直接在端口调用目标服务5156，而是通过端口调用其本地 Dapr sidecar 50007
GrpcChannel channel = GrpcChannel.ForAddress(args.FirstOrDefault() ?? "http://localhost:50007");
var client = new Greeter.GreeterClient(channel);
builder.Services.AddSingleton(client);


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/hello", async (DaprClient daprClient, Greeter.GreeterClient client) =>
    {
        {
            // [Obsolete("[DEPRECATION NOTICE] InvokeService is deprecated and will be removed in the future, please use proxy mode instead,  new grpc (\" proxy mode\")")]
            //HelloReply? result1 = await daprClient.InvokeMethodGrpcAsync<HelloRequest, HelloReply>("GrpcService1", nameof(Greeter.GreeterClient.SayHello), new HelloRequest
            //{
            //    Name = "dapr client"
            //});
        }
        // new grpc (" proxy mode")
        var metadata = new Metadata
        {
            { "dapr-app-id", "GrpcService1" }
        };
        HelloReply? result = await client.SayHelloAsync(new HelloRequest
        {
            Name = $"dapr new client({args.FirstOrDefault()})"
        }, metadata);
        return result.Message;
    })
    .WithName("hello")
    .WithOpenApi();

app.Run();