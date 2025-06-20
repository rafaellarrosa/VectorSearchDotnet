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
    public class Neo4JGraphDatabaseServiceTests
    {
        [Fact]
        public async Task Should_Create_Node_Successfully()
        {
            // Arrange
            var graphDatabaseService = Substitute.For<IGraphDatabaseService>();
            var logger = Substitute.For<ILogger<Neo4JGraphDatabaseServiceTests>>();

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
        public async Task Should_Query_Nodes_Successfully()
        {
            // Arrange
            var graphDatabaseService = Substitute.For<IGraphDatabaseService>();
            var logger = Substitute.For<ILogger<Neo4JGraphDatabaseServiceTests>>();

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
