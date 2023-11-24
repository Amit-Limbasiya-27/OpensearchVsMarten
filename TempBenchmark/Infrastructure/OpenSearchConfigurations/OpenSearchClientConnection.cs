using OpenSearch.Client;
using PerformanceTestOfMartenDBAndOpenSearch.Infrastructure.OpenSearchConfigurations;

namespace UtilityToInsertDataFromCSVtoOpenSearchDB.Infrastructure.OpenSearchConfigurations
{
    public class OpenSearchClientConnection : OpenSearchClient, IOpenSearchClientConnection
    {
        public OpenSearchClientConnection(IConnectionSettingsValues connectionSettingsValues) : base(connectionSettingsValues)
        {

        }
    }
}
