using GrpcService1.Services;

// dapr run --app-id GrpcService1 --app-port 5156 --app-protocol grpc -- dotnet run
// dapr run --app-id GrpcService1 --app-port 5156 --app-protocol grpc -- dotnet GrpcService1.dll --urls=http://localhost:5156

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
builder.Configuration.AddCommandLine(args);
// Add services to the container.
builder.Services.AddGrpc();
// builder.Services.AddSingleton<GreeterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
//[Obsolete("[DEPRECATION NOTICE] InvokeService is deprecated and will be removed in the future, please use proxy mode instead,  new grpc (\" proxy mode\")")]
// app.MapGrpcService<GreeterServiceDapr>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
