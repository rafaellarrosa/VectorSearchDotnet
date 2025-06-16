# VectorSearchDotnet

Practical example of using **.NET 8**, **Clean Architecture**, and **vector databases** to build a **semantic search API with embeddings**.

📦 Built with **Aspire**, **Minimal APIs**, and designed to integrate with **Qdrant** and AI services like **OpenAI**.

## 🔧 Features

- Embedding generation (mocked for now)  
  > Ready for integration with OpenAI or HuggingFace  
- Vector storage using Qdrant (mocked)
- **Cosine similarity** search
- REST API with **Clean Architecture**
- Tests with **xUnit**, **NSubstitute**, and **FluentAssertions**
- API documentation via **Swagger**
- Local orchestration with **Aspire AppHost**

## 🚀 How to Run

```bash
git clone https://github.com/rafaellarrosa/VectorSearchDotnet.git
cd VectorSearchDotnet
dotnet run --project src/WebAPI/WebAPI.csproj

Swagger:
http://localhost:{port}/swagger

Run tests:
dotnet test tests/Application.Tests/Application.Tests.csproj

✍️ Endpoints
Method	Route	Description
POST	/documents	Index a new text into the vector database
GET	/search	Search semantically similar texts

🔮 Roadmap
Real OpenAI embedding integration (e.g. text-embedding-3-small)

Real Qdrant integration (local or cloud)

Web UI (React) for searching content

PDF or video upload with auto transcription and embedding

🧠 About
This project is a foundation for building systems with semantic search, RAG, smart assistants, context-aware recommendations, and more.

css
Copiar
Editar


Se quiser, posso criar também a badge para CI, linkar com GitHub Actions, ou gerar a versão com imagem de capa do projeto. Deseja isso?

