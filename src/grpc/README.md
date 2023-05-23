#### 调试运行一、可以配置launchSettings.json如下

注：无法命中断点只是记录一下

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "dapr": {
      "commandName": "Executable",
      "executablePath": "dapr",
      "commandLineArgs": "run --app-id GrpcService1 --app-port 5156 --app-protocol grpc -- dotnet run",
      "workingDirectory": "./",
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "nativeDebugging": true
    },
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5156",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

```

#### 调试运行二、使用http配置运行，单独运行dapr进程

注：dapr进程生命周期最好与主进程一致

```shell
#服务端
dapr run --app-id GrpcService1 --app-port 5156 --app-protocol grpc -- dotnet run
#dapr run --app-id GrpcService1 --app-port 5156 --app-protocol grpc

#客户端 （dapr会动态http port、grpc port、需要placement，daprd默认使用3500、50001端口与daprclient访问端口一致）
daprd --app-id GrpcClient1 --app-port 5157 --app-protocol grpc
```

