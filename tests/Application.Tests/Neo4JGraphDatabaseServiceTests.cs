using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentAssertions;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public class Neo4JGraphDatabaseServiceTests
    {
        private readonly IGraphDatabaseService _graphDatabaseService;
        private readonly ILogger<Neo4JGraphDatabaseServiceTests> _logger;

        public Neo4JGraphDatabaseServiceTests()
        {
            _graphDatabaseService = Substitute.For<IGraphDatabaseService>();
            _logger = Substitute.For<ILogger<Neo4JGraphDatabaseServiceTests>>();
        }

        [Fact]
        public async Task Should_Create_Node_Successfully()
        {
            // Arrange
            var label = "Document";
            var properties = new Dictionary<string, object>
            {
                { "Id", "123" },
                { "Title", "Test Document" },
                { "Content", "This is a test document." }
            };

            // Act
            await _graphDatabaseService.CreateNodeAsync(label, properties);

            // Assert
            await _graphDatabaseService.Received(1).CreateNodeAsync(label, properties);
        }

        [Fact]
        public async Task Should_Query_Nodes_Successfully()
        {
            // Arrange
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

            _graphDatabaseService.QueryAsync(cypherQuery, parameters).Returns(expectedResult);

            // Act
            var result = await _graphDatabaseService.QueryAsync(cypherQuery, parameters);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task Should_Handle_Node_Creation_Error()
        {
            // Arrange
            var label = "Document";
            var properties = new Dictionary<string, object>
            {
                { "Id", "123" },
                { "Title", "Test Document" },
                { "Content", "This is a test document." }
            };

            _graphDatabaseService
                .When(x => x.CreateNodeAsync(label, properties))
                .Do(x => throw new Exception("Node creation failed"));

            // Act
            Func<Task> act = async () => await _graphDatabaseService.CreateNodeAsync(label, properties);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Node creation failed");
        }

        [Fact]
        public async Task Should_Handle_Query_Error()
        {
            // Arrange
            var cypherQuery = "MATCH (n:Document) RETURN n";
            var parameters = new Dictionary<string, object>();

            _graphDatabaseService
                .When(x => x.QueryAsync(cypherQuery, parameters))
                .Do(x => throw new Exception("Query execution failed"));

            // Act
            Func<Task> act = async () => await _graphDatabaseService.QueryAsync(cypherQuery, parameters);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Query execution failed");
        }
    }
}
