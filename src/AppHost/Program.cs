var builder = DistributedApplication.CreateBuilder(args);

var qdrantDataPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "qdrant_data"));

Directory.CreateDirectory(qdrantDataPath);

builder.AddDockerfile("qdrant", Path.Combine(Directory.GetCurrentDirectory(), "DockerFiles", "QDrant"))
    .WithHttpEndpoint(port: 6333, targetPort: 6333)
    .WithBindMount(qdrantDataPath, "/qdrant/storage"); // Use WithBindMount em vez de WithVolume

builder.AddProject<Projects.WebAPI>("aichat-api");

builder.Build().Run();