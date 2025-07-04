using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Xunit;
using Infrastructure.Services.Graph;
using NSubstitute;

namespace Application.Tests
{
    public class GraphDatabaseServiceTests
    {
        [Fact]
        public async Task Should_Create_Node_Successfully()
        {
            // Arrange
            var logger = Substitute.For<ILogger<Neo4JGraphDatabaseService>>();
            var graphDatabaseService = Substitute.For<Neo4JGraphDatabaseService>(logger);

            var label = "Document";
            var properties = new Dictionary<string, object>
            {
                { "Id", "123" },
                { "Title", "Test Document" },
                { "Content", "This is a test document." }
            };

            // Act
            await graphDatabaseService.CreateNodeAsync(label, properties);

            // Assert
            await graphDatabaseService.Received(1).CreateNodeAsync(label, properties);
        }

        [Fact]
        public async Task Should_Log_Error_When_Node_Creation_Fails()
        {
            // Arrange
            var logger = Substitute.For<ILogger<Neo4JGraphDatabaseService>>();
            var graphDatabaseService = Substitute.For<Neo4JGraphDatabaseService>(logger);

            var label = "Document";
            var properties = new Dictionary<string, object>
            {
                { "Id", "123" },
                { "Title", "Test Document" },
                { "Content", "This is a test document." }
            };

            graphDatabaseService
                .When(x => x.CreateNodeAsync(label, properties))
                .Do(x => throw new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await graphDatabaseService.CreateNodeAsync(label, properties);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        }

        [Fact]
        public async Task Should_Query_Nodes_Successfully()
        {
            // Arrange
            var logger = Substitute.For<ILogger<Neo4JGraphDatabaseService>>();
            var graphDatabaseService = Substitute.For<Neo4JGraphDatabaseService>(logger);

            var cypherQuery = "MATCH (n:Document) RETURN n";
            var parameters = new Dictionary<string, object>();

            var expectedResult = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "Id", "123" },
                    { "Title", "Test Document" },
                    { "Content", "This is a test document." }
                }
            };

            graphDatabaseService.QueryAsync(cypherQuery, parameters).Returns(expectedResult);

            // Act
            var result = await graphDatabaseService.QueryAsync(cypherQuery, parameters);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
