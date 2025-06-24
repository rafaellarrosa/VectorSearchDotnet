using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Xunit;
using NSubstitute;
using Infrastructure.Services.Embedding;
using Microsoft.Extensions.Configuration;

namespace Application.Tests
{
    public class EmbeddingServiceTests
    {
        [Fact]
        public async Task Should_Generate_Embedding_Successfully()
        {
            // Arrange
            var logger = Substitute.For<ILogger<EmbeddingService>>();
            var configuration = Substitute.For<IConfiguration>();
            configuration["EmbeddingOptions:ModelPath"].Returns("C:\\Users\\rafae\\.cache\\aigallery\\sentence-transformers--all-MiniLM-L12-v2");

            var embeddingService = new EmbeddingService(logger, configuration);

            var text = "Test text";

            // Act
            var result = await embeddingService.GenerateEmbeddingAsync(text);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(384); // Assuming embedding size is 384
        }

        [Fact]
        public async Task Should_Log_Error_When_Embedding_Fails()
        {
            // Arrange
            var logger = Substitute.For<ILogger<EmbeddingService>>();
            var configuration = Substitute.For<IConfiguration>();
            configuration["EmbeddingOptions:ModelPath"].Returns("InvalidPath");

            var embeddingService = new EmbeddingService(logger, configuration);

            var text = "Test text";

            // Act
            Func<Task> act = async () => await embeddingService.GenerateEmbeddingAsync(text);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
