// using System;
// using Application.Interfaces;
// using Domain.Entities;

// namespace Application.UseCases
// {
//         public class SearchSimilarDocumentsUseCase(IEmbeddingService embeddingService, IVectorDatabaseService vectorDbService)
//         {
//             public async Task<List<Document>> ExecuteAsync(string query, int topK = 5)
//             {
//                 var embedding = await embeddingService.GenerateEmbeddingAsync(query);
//                 return await vectorDbService.SearchSimilarAsync(embedding, topK);
//             }
//         }
// }