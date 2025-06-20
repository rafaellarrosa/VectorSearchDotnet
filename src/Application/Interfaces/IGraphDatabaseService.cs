using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGraphDatabaseService
    {
        Task CreateNodeAsync(string label, Dictionary<string, object> properties);
        Task<IEnumerable<Dictionary<string, object>>> QueryAsync(string cypherQuery, Dictionary<string, object> parameters);
    }
}
