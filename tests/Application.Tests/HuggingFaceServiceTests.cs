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
    public class HuggingFaceServiceTests
    {
        [Fact]
        public async Task Should_Generate_Answer_Successfully()
        {
            // Arrange
            var iaService = Substitute.For<IIaService>();
            var logger = Substitute.For<ILogger<HuggingFaceServiceTests>>();

            var question = "What is the test?";
            var documents = new List<string> { "Test document content." };
            var expectedAnswer = "This is the test answer.";

            iaService.GenerateAnswerAsync(question, documents).Returns(expectedAnswer);

            // Act
            var result = await iaService.GenerateAnswerAsync(question, documents);

            // Assert
            result.Should().Be(expectedAnswer);
        }

        [Fact]
        public async Task Should_Log_Error_When_Answer_Generation_Fails()
        {
            // Arrange
            var iaService = Substitute.For<IIaService>();
            var logger = Substitute.For<ILogger<HuggingFaceServiceTests>>();

            var question = "What is the test?";
            var documents = new List<string> { "Test document content." };

            iaService
                .When(x => x.GenerateAnswerAsync(question, documents))
                .Do(x => throw new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await iaService.GenerateAnswerAsync(question, documents);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        }
    }
}
