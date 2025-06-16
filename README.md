# VectorSearchDotnet 🧠

**VectorSearchDotnet** is a practical example of using **.NET 8**, **Clean Architecture**, and **vector databases** (Qdrant) to build a **semantic search API with embeddings**.

Built with:

- .NET 8 and Minimal APIs
- Aspire for local orchestration and container management
- Clean Architecture (Domain, Application, Infrastructure, WebAPI)
- Qdrant vector database (Docker, with persistent volume)
- Embedding integration ready for OpenAI or HuggingFace (e.g., all-MiniLM-L6-v2)
- Python embedding service using FastAPI + SentenceTransformers (see below)
- Serilog structured logging
- Swagger/OpenAPI documentation
- Tests with xUnit, NSubstitute, FluentAssertions

---

## Embedding Service (Python)

The embedding service is a simple Python project using FastAPI and SentenceTransformers. It exposes an endpoint to generate embeddings from text using a specific model (e.g., `all-MiniLM-L6-v2` or `all-mpnet-base-v2`).

Example code:

```python
from fastapi import FastAPI
from pydantic import BaseModel
from sentence_transformers import SentenceTransformer

app = FastAPI()

# Load the model once
# model = SentenceTransformer("all-MiniLM-L6-v2")
model = SentenceTransformer("all-mpnet-base-v2")

class EmbeddingRequest(BaseModel):
    text: str

@app.post("/embed/")
def generate_embedding(request: EmbeddingRequest):
    embedding = model.encode(request.text).tolist()
    return {"embedding": embedding}

@app.get("/")
def root():
    return {"message": "Gerador de embedding com sentence-transformers 🤖"}
```

Este serviço pode ser executado em um container Docker e é consumido pela aplicação .NET via HTTP (Refit). O modelo pode ser facilmente trocado conforme a necessidade, bastando alterar a linha de carregamento do modelo.

---

## Features

- Embedding generation (mocked or real, via Python service)
- Vector storage and similarity search with Qdrant
- Cosine similarity search
- Clean separation of concerns (Domain, Application, Infrastructure, WebAPI)
- API documentation with Swagger
- Logging with Serilog (console, environment, client info)
- Orchestration using Aspire AppHost (with Docker)
- Persistent Qdrant data volume (no data loss on container stop)

---

## How to Run

Clone the repository:

```bash
git clone https://github.com/rafaellarrosa/VectorSearchDotnet.git
cd VectorSearchDotnet
```

Run the full stack (API + Qdrant + orchestration):

```bash
dotnet run --project src/AppHost/AppHost.csproj
```

Or run only the API:

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

- [ ] Integrate with OpenAI or HuggingFace embeddings (e.g., `all-MiniLM-L6-v2`)
- [ ] Connect to real Qdrant instance (Docker or Cloud)
- [ ] Add frontend UI (React or Blazor)
- [ ] Upload and embed content from PDFs or video transcription
- [ ] Implement vector caching and cold-start protection
- [ ] Add authentication and authorization

---

## About

This project serves as a foundation for building:

- Semantic search APIs
- Retrieval-Augmented Generation (RAG) systems
- Smart assistants with contextual memory
- AI-powered recommendation engines
- Voice/video-aware applications with transcription-based search

---

**Author:** [Rafael Larrosa](https://github.com/rafaellarrosa)
