# VectorSearchDotnet

**VectorSearchDotnet** is a practical example of using **.NET 8**, **Clean Architecture**, and **vector databases** to build a **semantic search API with embeddings**.

Built with:

- .NET 8 and Minimal APIs  
- Aspire for local orchestration  
- Clean Architecture  
- Mocked Qdrant vector database  
- Ready for OpenAI or HuggingFace embedding integration

---

## Features

- Mocked embedding generation
- Vector storage with Qdrant (mocked)
- Cosine similarity search
- Clean Architecture: Domain, Application, Infrastructure, WebAPI
- API documentation with Swagger
- Tests with xUnit, NSubstitute, FluentAssertions
- Orchestration using Aspire AppHost

---

## How to Run

Clone the repository:

```bash
git clone https://github.com/rafaellarrosa/VectorSearchDotnet.git
cd VectorSearchDotnet
```

Run the API:

```bash
dotnet run --project src/WebAPI/WebAPI.csproj
```

Open Swagger in your browser:

```
http://localhost:{port}/swagger
```

Run the tests:

```bash
dotnet test tests/Application.Tests/Application.Tests.csproj
```

---

## Endpoints

| Method | Route      | Description                               |
|--------|------------|-------------------------------------------|
| POST   | /documents | Index a new text into the vector database |
| GET    | /search    | Search semantically similar texts         |

---

## Roadmap

- [ ] Integrate with OpenAI embeddings (e.g., `text-embedding-3-small`)
- [ ] Connect to real Qdrant instance (Docker or Cloud)
- [ ] Add frontend UI (React or Blazor)
- [ ] Upload and embed content from PDFs or video transcription
- [ ] Implement vector caching and cold-start protection

---

## About

This project serves as a foundation for building:

- Semantic search APIs  
- Retrieval-Augmented Generation (RAG) systems  
- Smart assistants with contextual memory  
- AI-powered recommendation engines  
- Voice/video-aware applications with transcription-based search
