var builder = DistributedApplication.CreateBuilder(args);

var qdrantDataPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "qdrant_data"));

Directory.CreateDirectory(qdrantDataPath);

builder.AddDockerfile("qdrant", Path.Combine(Directory.GetCurrentDirectory(), "DockerFiles", "QDrant"))
    .WithHttpEndpoint(port: 6333, targetPort: 6333)
    .WithBindMount(qdrantDataPath, "/qdrant/storage"); // Use WithBindMount em vez de WithVolume

var neo4jDataPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "neo4j_data"));
Directory.CreateDirectory(neo4jDataPath);

builder.AddDockerfile("neo4j", Path.Combine(Directory.GetCurrentDirectory(), "DockerFiles", "Neo4j"))
    .WithHttpEndpoint(port: 7474, targetPort: 7474) // Web UI (opcional)
    .WithEndpoint(name: "bolt", port: 7687, targetPort: 7687) // Porta para a aplicação
    .WithBindMount(neo4jDataPath, "/data") // Persistência opcional (igual fez no qdrant)
    .WithEnvironment("NEO4J_AUTH", "neo4j/#Web327390");

builder.AddProject<Projects.WebAPI>("aichat-api");

builder.Build().Run();