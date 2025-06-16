// using System;
// using Application.Interfaces;
// using Domain.Entities;
//
// namespace Application.UseCases
// {
//     public class IndexDocumentUseCase(IEmbeddingService embeddingService, IVectorDatabaseService vectorDbService)
//     {
//         public async Task ExecuteAsync(string text)
//         {
//             var embedding = await embeddingService.GenerateEmbeddingAsync(text);
//
//             var document = new Document
//             {
//                 Id = $"{Guid.NewGuid()}",
//                 Text = text,
//                 Embedding = embedding
//             };
//
//             await vectorDbService.IndexDocumentAsync(document);
//         }
//     }
// }