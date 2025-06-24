using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Infrastructure.Interfaces;

namespace Application.Tests
{
    public class QdrantServiceTests
    {
        private readonly IVectorDatabaseService _vectorDatabaseService;
        private readonly ILogger<QdrantServiceTests> _logger;

        public QdrantServiceTests()
        {
            _vectorDatabaseService = Substitute.For<IVectorDatabaseService>();
            _logger = Substitute.For<ILogger<QdrantServiceTests>>();
        }

        [Fact]
        public async Task Should_Index_Document_Successfully()
        {
            // Arrange
            var document = new DocumentDto("Test document", new float[] { 0.1f, 0.2f, 0.3f });
            var expectedId = Guid.NewGuid();

            _vectorDatabaseService.IndexDocumentAsync(document).Returns(expectedId);

            // Act
            var result = await _vectorDatabaseService.IndexDocumentAsync(document);

            // Assert
            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task Should_Log_Error_When_Indexing_Fails()
        {
            // Arrange
            var document = new DocumentDto("Test document", new float[] { 0.1f, 0.2f, 0.3f });

            _vectorDatabaseService
                .When(x => x.IndexDocumentAsync(document))
                .Do(x => throw new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _vectorDatabaseService.IndexDocumentAsync(document);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        }

        [Fact]
        public async Task Should_Search_Similar_Documents_Successfully()
        {
            // Arrange
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };
            var expectedDocuments = new List<DocumentResposeDto>
            {
                new(Guid.NewGuid(), "Test document 1", 0.95f),
                new(Guid.NewGuid(), "Test document 2", 0.89f)
            };

            _vectorDatabaseService.SearchSimilarAsync(embedding, 2).Returns(expectedDocuments);

            // Act
            var result = await _vectorDatabaseService.SearchSimilarAsync(embedding, 2);

            // Assert
            result.Should().BeEquivalentTo(expectedDocuments);
        }

        [Fact]
        public async Task Should_Handle_Search_Error()
        {
            // Arrange
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            _vectorDatabaseService
                .When(x => x.SearchSimilarAsync(embedding, 2))
                .Do(x => throw new Exception("Search failed"));

            // Act
            Func<Task> act = async () => await _vectorDatabaseService.SearchSimilarAsync(embedding, 2);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Search failed");
        }
    }
}
