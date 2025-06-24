using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Infrastructure.Services.Graph
{
    public class Neo4JGraphDatabaseService : IGraphDatabaseService, IAsyncDisposable
    {
        private readonly IDriver _driver;
        private readonly ILogger<Neo4JGraphDatabaseService> _logger;

        public Neo4JGraphDatabaseService(IConfiguration configuration, ILogger<Neo4JGraphDatabaseService> logger)
        {
            var uri = configuration["GraphDatabase:Uri"] ?? throw new ArgumentNullException("GraphDatabase:Uri", "GraphDatabase URI is not configured.");
            var username = configuration["GraphDatabase:Username"] ?? throw new ArgumentNullException("GraphDatabase:Username", "GraphDatabase Username is not configured.");
            var password = configuration["GraphDatabase:Password"] ?? throw new ArgumentNullException("GraphDatabase:Password", "GraphDatabase Password is not configured.");

            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateNodeAsync(string label, Dictionary<string, object> properties)
        {
            if (string.IsNullOrWhiteSpace(label) || label.Any(c => !char.IsLetterOrDigit(c)))
            {
                throw new ArgumentException("Label contains invalid characters.", nameof(label));
            }

            try
            {
                await using var session = _driver.AsyncSession();
                var cypher = $"CREATE (n:{label}) SET n += $properties";
                _logger.LogDebug("Executing Cypher query: {Cypher} with properties: {Properties}", cypher, properties);

                var parameters = new Dictionary<string, object> { { "properties", properties } };
                await session.RunAsync(cypher, parameters);

                _logger.LogInformation("Node with label {Label} created successfully.", label);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating node with label {Label}.", label);
                throw;
            }
        }

        public async Task<IEnumerable<Dictionary<string, object>>> QueryAsync(string cypherQuery, Dictionary<string, object> parameters)
        {
            try
            {
                await using var session = _driver.AsyncSession();
                var result = await session.RunAsync(cypherQuery, parameters);
                var records = await result.ToListAsync(record => record.Keys.ToDictionary(key => key, key => record[key]));

                _logger.LogInformation("Query executed successfully: {CypherQuery}", cypherQuery);
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query: {CypherQuery}", cypherQuery);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_driver != null)
            {
                await _driver.DisposeAsync();
            }
        }
    }
}
