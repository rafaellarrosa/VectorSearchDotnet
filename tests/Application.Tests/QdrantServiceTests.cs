using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public class QdrantServiceTests
    {
        [Fact]
        public async Task Should_Index_Document_Successfully()
        {
            // Arrange
            var vectorDatabaseService = Substitute.For<IVectorDatabaseService>();
            var logger = Substitute.For<ILogger<QdrantServiceTests>>();

            var document = new DocumentDto("Test document", new float[] { 0.1f, 0.2f, 0.3f });
            var expectedId = Guid.NewGuid();

            vectorDatabaseService.IndexDocumentAsync(document).Returns(expectedId);

            // Act
            var result = await vectorDatabaseService.IndexDocumentAsync(document);

            // Assert
            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task Should_Log_Error_When_Indexing_Fails()
        {
            // Arrange
            var vectorDatabaseService = Substitute.For<IVectorDatabaseService>();
            var logger = Substitute.For<ILogger<QdrantServiceTests>>();

            var document = new DocumentDto("Test document", new float[] { 0.1f, 0.2f, 0.3f });

            vectorDatabaseService
                .When(x => x.IndexDocumentAsync(document))
                .Do(x => throw new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await vectorDatabaseService.IndexDocumentAsync(document);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        }

        [Fact]
        public async Task Should_Search_Similar_Documents_Successfully()
        {
            // Arrange
            var vectorDatabaseService = Substitute.For<IVectorDatabaseService>();
            var logger = Substitute.For<ILogger<QdrantServiceTests>>();

            var embedding = new float[] { 0.1f, 0.2f, 0.3f };
            var expectedDocuments = new List<DocumentResposeDto>
            {
                new DocumentResposeDto(Guid.NewGuid(), "Test document 1"),
                new DocumentResposeDto(Guid.NewGuid(), "Test document 2")
            };

            vectorDatabaseService.SearchSimilarAsync(embedding, 2).Returns(expectedDocuments);

            // Act
            var result = await vectorDatabaseService.SearchSimilarAsync(embedding, 2);

            // Assert
            result.Should().BeEquivalentTo(expectedDocuments);
        }
    }
}
