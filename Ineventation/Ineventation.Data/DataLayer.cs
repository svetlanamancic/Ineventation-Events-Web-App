using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Ineventation.Data
{
    public class DataLayer
    {
        private static GraphClient client;
        public static GraphClient getClient()
        {
            if (client == null)
            {
                client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "nbp");
                try
                {
                    client.Connect();
                }
                catch (Exception)
                {
                }
            }

            return client;
        }
    }
}
