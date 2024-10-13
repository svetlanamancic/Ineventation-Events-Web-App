using Ineventation.Data.Models;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ineventation.Data.Repositories
{
    public class CategoryRepository
    {

        public void DeleteCategory(string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Name", name);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Category) and exists(n.Name) and n.Name =~ {Name} detach delete n",
                                                             queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<CategoryModel>(query);

        }

        public CategoryModel CreateCategory(string name)
        {

            string Id = Guid.NewGuid().ToString();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            try
            {
                var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Category {Id:'" + Id + "', Name:'" + name + "'}) return n",
                                                                queryDict, CypherResultMode.Set);

                CategoryModel cat = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<CategoryModel>(query).ToList().First();

                return cat;

            }
            catch (Exception )
            {
                return null;
            }

        }

        public bool ExistsCategory(string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Name", name);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Category) and exists(n.Name) and n.Name =~ {Name} return n",
                                                            queryDict, CypherResultMode.Set);

            List<CategoryModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<CategoryModel>(query).ToList();

            if (obj.Count > 0)
                return true;

            return false ;
        }

        public List<CategoryModel> GetAllCategories()
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Category) return n",
                                                            queryDict, CypherResultMode.Set);

            List<CategoryModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<CategoryModel>(query).ToList();

            return obj;
        }


        #region LikesRelationship

        public void CreateLikes(string token, string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Token", token);
            queryDict.Add("Name", name);

            var q1 = new Neo4jClient.Cypher.CypherQuery("match (u:User),(c:Category) where u.Token=~{Token} and c.Name=~{Name} create (u)-[r:Likes]->(c)",
                                                            queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
        }

        public void DeleteLikes(string token, string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Token", token);
            queryDict.Add("Name", name);

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Category)<-[r:Likes]-(i:User) WHERE i.Token =~ {Token} and n.Name=~{Name} delete r",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
        }

        public List<CategoryModel> GetLikes(string token)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Token", token);

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Category)<-[r:Likes]-(i:User) WHERE i.Token =~ {Token} return n",
                                                            queryDict, CypherResultMode.Set);

            List<CategoryModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<CategoryModel>(q1).ToList();

            return obj;
        }

        #endregion


        #region BelongsToRelationship

        public void UpdateBelongsTo(string eventId, string typeName)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Id", eventId);
            queryDict.Add("Name", typeName);


            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Category)<-[r:BelongsTo]-(i:Event) WHERE i.Id =~ {Id} and n.Name=~{Name} return n",
                                                            queryDict, CypherResultMode.Set);

            if(((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<CategoryModel>(q1).ToList()==null)
            {

                //obrisi vezu sa trenutnom
                var q2 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Category)<-[r:BelongsTo]-(i:Event) WHERE i.Id =~ {Id} delete r",
                                                           queryDict, CypherResultMode.Set);

                ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<CategoryModel>(q2);

                //kreiraj vezu sa novom kategorijom
                var q3 = new Neo4jClient.Cypher.CypherQuery("match (u:Event),(c:Category) where u.Id=~{Id} and c.Name=~{Name} create (u)-[r:BelongsTo]->(c)",
                                                            queryDict, CypherResultMode.Projection);

                ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q3);

            }

        }

        public void CreateBelongsTo(string eventId, string typeName)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Id", eventId);
            queryDict.Add("Name", typeName);

            var q = new Neo4jClient.Cypher.CypherQuery("match (u:Event),(c:Category) where u.Id=~{Id} and c.Name=~{Name} create (u)-[r:BelongsTo]->(c)",
                                                            queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q);
        }

        #endregion

    }
}
