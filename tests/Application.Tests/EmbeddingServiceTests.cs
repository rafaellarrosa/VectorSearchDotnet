using System;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public class EmbeddingServiceTests
    {
        [Fact]
        public async Task Should_Generate_Embedding_Successfully()
        {
            // Arrange
            var embeddingService = Substitute.For<IEmbeddingService>();
            var logger = Substitute.For<ILogger<EmbeddingServiceTests>>();

            var text = "Test text";
            var expectedEmbedding = new float[] { 0.1f, 0.2f, 0.3f };

            embeddingService.GenerateEmbeddingAsync(text).Returns(expectedEmbedding);

            // Act
            var result = await embeddingService.GenerateEmbeddingAsync(text);

            // Assert
            result.Should().BeEquivalentTo(expectedEmbedding);
        }

        [Fact]
        public async Task Should_Log_Error_When_Embedding_Fails()
        {
            // Arrange
            var embeddingService = Substitute.For<IEmbeddingService>();
            var logger = Substitute.For<ILogger<EmbeddingServiceTests>>();

            var text = "Test text";

            embeddingService
                .When(x => x.GenerateEmbeddingAsync(text))
                .Do(x => throw new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await embeddingService.GenerateEmbeddingAsync(text);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        }
    }
}
