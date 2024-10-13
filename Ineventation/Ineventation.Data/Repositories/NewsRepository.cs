using Ineventation.Data.Models;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineventation.Data.Repositories
{
    public class NewsRepository
    {
        public NewsModel CreateNews(string message, string idUser, string idObject, string type)
        {
            NewsModel n = new NewsModel();
            n.Message = message;
            n.IdObject = idObject;
            n.Type = type;
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id",idUser);

            try
            {
                var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:News {Id:'" + n.Id + "', Status:'" + n.Status + "', Message:'" + n.Message + "'," +
                                                            "Time:'" + n.Time + "', IdObject:'" + n.IdObject + "', Type:'" + n.Type + "'}) return n",
                                                                queryDict, CypherResultMode.Set);

                NewsModel m = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<NewsModel>(query).ToList().First();

                queryDict.Add("IdNews", m.Id);

                var q1 = new Neo4jClient.Cypher.CypherQuery("match (u:User),(c:News) where u.Id=~{Id} and c.Id=~{IdNews} create (u)-[r:Has]->(c)",
                                                            queryDict, CypherResultMode.Projection);

                ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);

                return m;

            }
            catch (Exception)
            {
                return null;
            }

        }

        public List<NewsModel> GetNews(string token)//set news status to read
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Token", token);

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:News)<-[r:Has]-(i:User) WHERE i.Token =~ {Token}  return n",
                                                            queryDict, CypherResultMode.Set);

            List<NewsModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<NewsModel>(q1).ToList();

            foreach(NewsModel n in obj)
            {
                var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:News) and exists(n.Id) and n.Id =~ '"+ n.Id +
                                                            "'set n.Status='read' return n",
                                                            queryDict, CypherResultMode.Set);

                NewsModel news = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<NewsModel>(query).ToList().First();

            }

            return obj;
        }

        public void DeleteNews(string token)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:News)<-[r:Has]-(i:User) WHERE i.Token =~ {Token} and n.Status=~'read' detach delete n",
                                                                        queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<NewsModel>(q1);
        }
    }
}
