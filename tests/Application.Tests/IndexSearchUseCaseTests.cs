using System;
using Application.Interfaces;
using Application.UseCases;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Application.Tests;

public class IndexSearchUseCaseTests
{
    [Fact]
    public async Task Should_Index_Document_With_Embedding()
    {
        // Arrange
        var text = "test document";
        var fakeEmbedding = new float[] { 0.1f, 0.2f, 0.3f };

        var embeddingService = Substitute.For<IEmbeddingService>();
        var vectorDbService = Substitute.For<IVectorDatabaseService>();
        var useCase = new IndexDocumentUseCase(embeddingService, vectorDbService);

        embeddingService.GenerateEmbeddingAsync(text).Returns(Task.FromResult(fakeEmbedding));

        // Act
        await useCase.ExecuteAsync(text);

        // Assert
        await embeddingService.Received(1).GenerateEmbeddingAsync(text);
        await vectorDbService.Received(1).IndexDocumentAsync(Arg.Is<Document>(d =>
            d.Text == text &&
            d.Embedding == fakeEmbedding
        ));
    }

    [Fact]
    public async Task Should_Return_Similar_Documents()
    {
        // Arrange
        var query = "find me something";
        var fakeEmbedding = new float[] { 0.9f, 0.8f, 0.7f };
        var expectedDocs = new List<Document>
            {
                new() { Id = Guid.NewGuid(), Text = "result 1" },
                new() { Id = Guid.NewGuid(), Text = "result 2" }
            };

        var embeddingService = Substitute.For<IEmbeddingService>();
        var vectorDbService = Substitute.For<IVectorDatabaseService>();
        var useCase = new SearchSimilarDocumentsUseCase(embeddingService, vectorDbService);

        embeddingService.GenerateEmbeddingAsync(query).Returns(Task.FromResult(fakeEmbedding));
        vectorDbService.SearchSimilarAsync(fakeEmbedding, 2).Returns(expectedDocs);

        // Act
        var result = await useCase.ExecuteAsync(query, 2);

        // Assert
        result.Should().BeEquivalentTo(expectedDocs);
    }
}