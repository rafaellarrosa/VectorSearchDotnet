# VectorSearchDotnet 🧠 (RAG + Graph Full Stack)

**VectorSearchDotnet** é um projeto completo de **Retrieval-Augmented Generation (RAG)** com IA Generativa, Embeddings e Banco de Grafos, 100% em .NET 8 e Clean Architecture.

---

### 🔧 Tecnologias Utilizadas

- ✅ .NET 8 (Minimal APIs)
- ✅ Clean Architecture (Domain, Application, Infrastructure, WebAPI)
- ✅ Aspire (.NET Distributed Application)
- ✅ Docker (Orquestração local completa)
- ✅ Qdrant (Vector Database)
- ✅ Neo4j (Graph Database)
- ✅ Hugging Face Inference API (Embeddings e Text Generation)
- ✅ Refit (HTTP client de alto nível)
- ✅ MediatR, AutoMapper, Serilog
- ✅ Testes com xUnit, FluentAssertions e NSubstitute

---

## 🧠 Arquitetura Completa

```mermaid
graph TD
    A[Usuário] -->|Pergunta| B[WebAPI (.NET)]
    B -->|Gera Embedding| C[Hugging Face API]
    B -->|Busca Semântica| D[Qdrant]
    B -->|Recupera Relações| E[Neo4j]
    B -->|Geração RAG| F[Hugging Face Text Generation]
    F -->|Resposta| A
```

---

## 🚀 Como Executar

Clone o repositório:

```bash
git clone https://github.com/rafaellarrosa/VectorSearchDotnet.git
cd VectorSearchDotnet
```

Subir toda a stack com Aspire:

```bash
dotnet run --project src/AppHost/AppHost.csproj
```

Acesse o Swagger para testar:

```
http://localhost:{porta}/swagger
```

---

## 🔗 Integrações Externas

- **Hugging Face Inference API**

  - Embedding model: `sentence-transformers/all-mpnet-base-v2`
  - Text Generation model: `mistralai/Mistral-7B-Instruct-v0.1`\
    (Configuração via appsettings + Token HF)

- **Qdrant** (Docker com volume persistente)

- **Neo4j** (Docker standalone via Aspire)

---

## 🛠️ Endpoints Principais

| Método | Rota       | Função                                 |
| ------ | ---------- | -------------------------------------- |
| POST   | /documents | Indexa novo documento (Qdrant + Neo4j) |
| GET    | /search    | Realiza busca semântica + geração RAG  |

---

## 🔬 Pipeline do RAG com Graph

1️⃣ Gera embedding da pergunta com Hugging Face\
2️⃣ Busca vetorial no Qdrant\
3️⃣ Recupera contexto e conexões via Neo4j\
4️⃣ Monta resposta com modelo generativo RAG

---

## 🔮 Extensões futuras

- Upload de PDFs e processamento automático
- Extração de entidades e criação automática de nós no grafo
- UI frontend (Blazor ou React)
- Histórico de consultas e dashboard analítico
- Melhorias no pipeline com CoT (Chain-of-Thought prompting)
- Cache de embeddings para otimização

---

## 📖 Sobre o projeto

Este projeto é uma fundação para:

- RAG corporativo (jurídico, financeiro, documentos técnicos)
- Assistentes de IA contextuais
- Pesquisa semântica híbrida (embedding + grafo)
- Sistemas de recomendação explicáveis

---

## 👨‍💻 Autor

**Rafael Larrosa**\
[GitHub](https://github.com/rafaellarrosa)

