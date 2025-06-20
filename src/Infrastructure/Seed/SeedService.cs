using Application.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Seed
{
    public class SeedService
    {
        private readonly IMediator _mediator;
        private readonly IGraphDatabaseService _graphDatabaseService;
        private readonly ILogger<SeedService> _logger;

        public SeedService(
            IMediator mediator,
            IGraphDatabaseService graphDatabaseService,
            ILogger<SeedService> logger)
        {
            _mediator = mediator;
            _graphDatabaseService = graphDatabaseService;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting seeding...");

            var documents = new List<(string Title, string Content)>
            {
                ("ICMS na Energia Solar - Decisão STF", "O Supremo Tribunal Federal decidiu que não incide ICMS sobre a energia elétrica gerada por micro e minigeração distribuída, conforme o Recurso Extraordinário nº 928.943/RS."),
                ("Compensação de Energia - Resolução ANEEL 482", "A Resolução Normativa ANEEL nº 482/2012 estabelece o sistema de compensação de energia elétrica para unidades consumidoras com geração distribuída."),
                ("Regulamentação Estadual de ICMS", "Alguns estados brasileiros implementaram isenção de ICMS em suas legislações estaduais para incentivar a geração solar fotovoltaica."),
                ("Decisão STJ sobre bitributação de ISS", "O STJ definiu que não há bitributação de ISS e ICMS na prestação de serviços de telecomunicações, destacando a competência tributária de cada ente federativo."),
                ("Artigo: Benefícios Ambientais da Energia Solar", "A geração distribuída de energia solar reduz significativamente a emissão de gases de efeito estufa e diminui a necessidade de expansão de usinas termelétricas."),
                ("Revisão tarifária para microgeradores", "A ANEEL revisou a metodologia de aplicação das tarifas para pequenos geradores, trazendo mais previsibilidade para o retorno sobre o investimento."),
                ("Parecer técnico - ICMS x Geração Distribuída", "Há divergências entre estados quanto ao entendimento da não incidência de ICMS sobre a energia compensada, gerando debates jurídicos em vários tribunais.")
            };

            // Inserir os documentos no pipeline completo (Qdrant + Neo4j)
            foreach (var doc in documents)
            {
                _logger.LogInformation("Indexando documento: {Title}", doc.Title);
                await _mediator.Send(new IndexDocumentCommand(doc.Title, doc.Content));
            }

            _logger.LogInformation("Documentos inseridos. Agora criando relações no Neo4j...");

            // Criar relações entre os documentos no Neo4j
            await CreateRelationsAsync();

            _logger.LogInformation("Seeding finalizado com sucesso.");
        }

        private async Task CreateRelationsAsync()
        {
            var relations = new List<(string From, string To)>
            {
                ("ICMS na Energia Solar - Decisão STF", "Compensação de Energia - Resolução ANEEL 482"),
                ("ICMS na Energia Solar - Decisão STF", "Parecer técnico - ICMS x Geração Distribuída"),
                ("ICMS na Energia Solar - Decisão STF", "Regulamentação Estadual de ICMS")
            };

            foreach (var (fromTitle, toTitle) in relations)
            {
                var cypher = @"
                    MATCH (a:Document {Title: $fromTitle}), (b:Document {Title: $toTitle})
                    MERGE (a)-[:RELATED_TO]->(b)
                ";

                var parameters = new Dictionary<string, object>
                {
                    { "fromTitle", fromTitle },
                    { "toTitle", toTitle }
                };

                await _graphDatabaseService.QueryAsync(cypher, parameters);
                _logger.LogInformation("Relação criada: {From} -> {To}", fromTitle, toTitle);
            }
        }
    }
}
