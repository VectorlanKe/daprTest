using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService;

namespace GrpcService1.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IConfiguration _configuration;
        public GreeterService(ILogger<GreeterService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = $"service({_configuration["urls"]}):Hello {request.Name}" 
            });
        }
    }
    [Obsolete("[DEPRECATION NOTICE] InvokeService is deprecated and will be removed in the future, please use proxy mode instead,  new grpc (\"1\")")]
    public class GreeterServiceDapr : AppCallback.AppCallbackBase
    {
        private readonly GreeterService _greeterService;

        public GreeterServiceDapr(GreeterService greeterService)
        {
            _greeterService = greeterService;
        }

        public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var response = new InvokeResponse();
            switch (request.Method)
            {
                case "SayHello":
                    HelloRequest input = request.Data.Unpack<HelloRequest>();
                    response.Data=Any.Pack(await _greeterService.SayHello(input, context));
                    break;
            }
            return response;
        }
    }
}