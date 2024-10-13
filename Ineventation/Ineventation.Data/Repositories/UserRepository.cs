using Ineventation.Data.Models;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using Ineventation.Data.Helpers;

namespace Ineventation.Data.Repositories
{
    public class UserRepository
    {

        #region BasicUser

        public string CreateUser(UserModel u)
        {

            u.Id = Guid.NewGuid().ToString();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
           
            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:User {Id:'"+u.Id+"', FirstName:'"+StringHelper.FirstUpperRestLower(u.FirstName)+"', LastName:'"+ StringHelper.FirstUpperRestLower(u.LastName) + "'"
                                                            + ", Username:'"+u.Username+"', Password:'"+u.Password+"', "+ "Email:'"+u.Email+ "', AccountType:'" + u.AccountType + "', Token:'" + u.Token+ "', City:'" + u.City + "', Image:'" + u.Image + "'}) return n",
                                                            queryDict, CypherResultMode.Set);
            try
            {
                UserModel user = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList().First();
                return user.Id;
            }
            catch (Exception)
            {
                return "";
            }

        }

        public UserModel UpdateUser(UserModel u)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("FirstName", StringHelper.FirstUpperRestLower(u.FirstName));
            queryDict.Add("LastName", StringHelper.FirstUpperRestLower(u.LastName));
            queryDict.Add("City", u.City);
            queryDict.Add("Username", u.Username);
            queryDict.Add("Password", u.Password);
            queryDict.Add("Email", u.Email);
            queryDict.Add("Token", u.Token);
            queryDict.Add("Image", u.Image);

            UserModel user = null;

            if (u.Image != "")
            {
                var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:User) and exists(n.Token) and n.Token =~ {Token}" +
                                                                "set n.FirstName={FirstName}, n.LastName={LastName}, n.City={City},"
                                                                + " n.Username={Username}, n.Email={Email}, n.Image={Image}, n.Password={Password} return n",
                                                                queryDict, CypherResultMode.Set);

                user = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList().First();

            }
            else
            {
                var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:User) and exists(n.Token) and n.Token =~ {Token}" +
                                                                "set n.FirstName={FirstName}, n.LastName={LastName}, n.City={City},"
                                                                + " n.Username={Username}, n.Email={Email}, n.Password={Password} return n",
                                                                queryDict, CypherResultMode.Set);

                user = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList().First();
            }

            return user;

        }

        public UserModel DeleteUser(string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);

            var q1 = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:User) and exists(n.Token) and n.Id =~ {Id} detach delete n",
                                                             queryDict, CypherResultMode.Projection);

            var q2 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[:Created]-(i:User) WHERE i.Id =~ {Id} detach delete n",
                                                            queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(q2);// q2 pa q1 da se prvo obrisu dogadjaji pa i sam korisnik

            ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(q1);

            return null;
        }

        #endregion

        #region GetUser

        public UserModel GetById(string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:User) and exists(n.Id) and n.Id=~{Id} return n",
                                                            queryDict, CypherResultMode.Set);

            List<UserModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            if (obj.Count > 0)
                return obj.First();

            return null;
        }

        public UserModel GetByUsername(string username)
        {
            try
            {
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("Username", username);

                var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:User) and exists(n.Username) and n.Username=~{Username} return n",
                                                                queryDict, CypherResultMode.Set);

                List<UserModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

                if (obj.Count > 0)
                    return obj.First();

                return null;
            }
            catch(Exception )
            {
                return null;
            }
        }

        public UserModel GetByToken(string token)
        {
            try
            {
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("Token", token);

                var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:User) and exists(n.Token) and n.Token=~{Token} return n",
                                                                queryDict, CypherResultMode.Set);

                List<UserModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

                if (obj.Count > 0)
                    return obj.First();

                return null;
            }
            catch (Exception )
            {
                return null;
            }
        }

        public UserModel GetByEmail(string email)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Email", email);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:User) and exists(n.Email) and n.Email =~ {Email} return n",
                                                            queryDict, CypherResultMode.Set);

            List<UserModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            if (obj.Count > 0)
                return obj.First();

            return null;

        }

        #endregion
        
        #region Search

        public List<UserModel> GetForSearch(string search)
        {
            string searchPart = ".*" + search + ".*";
            string searchWithHelper = ".*" + StringHelper.FirstUpperRestLower(search) + ".*";
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Username", searchPart);
            queryDict.Add("FirstName", searchWithHelper);
            queryDict.Add("LastName", searchWithHelper);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User) WHERE n.Username =~ {Username} or n.FirstName=~{FirstName} or n.LastName=~{LastName} return n",
                                                            queryDict, CypherResultMode.Set);

            List<UserModel> users = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();
            users.RemoveAll(x=>x.AccountType=="promoter");
            users.RemoveAll(x => x.AccountType == "admin");


            return users;
        }

        #endregion

        #region IsFriendsWithRelationship
        public List<UserModel> GetFriendships(string id)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("Id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[:IsFriendsWith]-(p:User) WHERE n.Id=~{Id} return p", 
                                                            dict, CypherResultMode.Set);

            List<UserModel> friends = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            return friends;
        }

        public bool GetFriendship(string token, string friendId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);
            queryDict.Add("FriendId", friendId);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (u:User)-[:IsFriendsWith]-(e:User) WHERE u.Token=~{Token} and e.Id =~ {FriendId} return e",
                                                            queryDict, CypherResultMode.Set);



            if (((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList().Count > 0)
                return true;
            return false;
        }

        public void CreateFriendship(string u1, string u2)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Id1", u1);
            queryDict.Add("Id2", u2);


            var q1 = new Neo4jClient.Cypher.CypherQuery("match (u:User),(c:User) where u.Id=~{Id1} and c.Id=~{Id2} create (u)-[r:IsFriendsWith]->(c)",
                                                            queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);

            var q2 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[r:SentFriendRequest]->(i:User) WHERE i.Id =~ {Id1} and n.Id=~{Id2} delete r",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q2);

        }

        public void DeleteFriendship(string token, string friendId)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Token", token);
            queryDict.Add("Id", friendId);

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[r:IsFriendsWith]-(i:User) WHERE i.Token =~ {Token} and n.Id=~{Id} delete r",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);

            var q2 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[r:InvitedTo]->(i:Event)<-[:Created]-(u:User) WHERE u.Token =~ {Token} and n.Id=~{Id} delete r",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q2);

            var q3 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[r:InvitedTo]->(i:Event)<-[:Created]-(u:User) WHERE n.Token =~ {Token} and u.Id=~{Id} delete r",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q3);

        }

        #endregion

        #region Report

        public void CreateReport(string token, string idUser)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);
            queryDict.Add("Id", idUser);

            var q2 = new Neo4jClient.Cypher.CypherQuery("match (u:User)-[:Reported]->(c:User) where u.Token=~{Token} and c.Id=~{Id} return c",
                                                            queryDict, CypherResultMode.Set);
            if (((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(q2).ToList().Count == 0)
            {
                var q1 = new Neo4jClient.Cypher.CypherQuery("match (u:User),(c:User) where u.Token=~{Token} and c.Id=~{Id} create (u)-[r:Reported]->(c)",
                                                                queryDict, CypherResultMode.Projection);

                ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
            }
        }

        public List<UserModel> GetReportedUsers()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[:Reported]->(p:User) return distinct p",
                                                            dict, CypherResultMode.Set);

            List<UserModel> users = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            return users;
        }

        public int GetNumberOfReports(string id)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("Id",id);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[:Reported]->(p:User) where p.Id=~{Id} return p",
                                                            dict, CypherResultMode.Set);

            List<UserModel> users = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            return users.Count();
        }

        #endregion

        #region FriendRequest

        public void CreateSentFriendRequest(string sender, string receiver)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("sender", sender);
            queryDict.Add("receiver", receiver);
            queryDict.Add("date", DateTime.Now);


            var q2 = new Neo4jClient.Cypher.
                         CypherQuery("match (u:User)-[:SentFriendRequest]->(c:User) where u.Id=~{sender} and c.Id=~{receiver} return c",
                         queryDict, CypherResultMode.Set);

            var q3 = new Neo4jClient.Cypher.CypherQuery("match (u:User)-[:SentFriendRequest]->(c:User) where u.Id=~{receiver} and c.Id=~{sender} return c",
                                                            queryDict, CypherResultMode.Set);
            if ((((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(q2).ToList().Count == 0) && 
                (((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(q3).ToList().Count == 0))
            {
                var q1 = new Neo4jClient.Cypher.
                             CypherQuery("match (u:User),(c:User) where u.Id=~{sender} and c.Id=~{receiver} create (u)-[r:SentFriendRequest{date:{date}}]->(c)",
                             queryDict, CypherResultMode.Projection);

                ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
            }
        }

        public bool GetFriendRequest(string senderId, string receiverId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", senderId);
            queryDict.Add("Id2", receiverId);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (u:User)-[:SentFriendRequest]->(e:User) WHERE u.Id=~{Id} and e.Id =~ {Id2} return e",
                                                            queryDict, CypherResultMode.Set);



            if (((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList().Count > 0)
                return true;
            return false;
        }

        public void RemoveFriendshipRequest(string sender, string receiver)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("sender", sender);
            queryDict.Add("receiver", receiver);

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[r:SentFriendRequest]->(i:User) WHERE i.Id =~ {sender} and n.Id=~{receiver} delete r",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
        }

        #endregion


        public List<UserModel> GetUsers()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User) return n",
                                                            dict, CypherResultMode.Set);

            List<UserModel> users = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            return users;
        }
    }
}
