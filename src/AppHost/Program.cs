
var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<Projects.WebAPI>("aichat-api");

builder.Build().Run();
