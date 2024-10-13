using Ineventation.Data.Models;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ineventation.Data.Repositories
{
    public class EventRepository
    {

        #region BasicEvent

        //delete
        public void DeleteEvent(string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Event) and exists(n.Id) and n.Id =~ {Id} detach delete n",
                                                             queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query);

        }

        //add event
        public EventModel CreateEvent(EventModel e)
        {

            e.Id = Guid.NewGuid().ToString();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Creator", e.Creator.Token);
            queryDict.Add("Id", e.Id);

            try
            {
                var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Event {Id:'"+e.Id+"', Caption:'"+e.Caption+"', Description:'"+e.Description+"'"
                                                                + ", City:'"+e.City+"', Location:'"+e.Location+"', Date:'"+e.Date+ "', DateAndTime:'" + e.DateAndTime + "',Time:'" +
                                                                e.Time+"', Visibility:'"+e.Visibility+ "', Image:'" + e.Image + "'}) return n",
                                                                queryDict, CypherResultMode.Set);

                EventModel ev = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList().First();

                var q1 = new Neo4jClient.Cypher.CypherQuery("match (u:User),(c:Event) where u.Token=~{Creator} and c.Id=~{Id} create (u)-[r:Created]->(c) return r",
                                                            queryDict, CypherResultMode.Projection);
               
                ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);

                

                return ev;
            }
            catch (Exception )
            {
                return null;
            }

        }

        //update event
        public EventModel UpdateEvent(EventModel e)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Caption", e.Caption);
            queryDict.Add("Description", e.Description);
            queryDict.Add("City", e.City);
            queryDict.Add("Location", e.Location);
            queryDict.Add("Date", e.Date);
            queryDict.Add("Time", e.Time);
            queryDict.Add("DateAndTime", e.DateAndTime);
            queryDict.Add("Id", e.Id);
            queryDict.Add("Image", e.Image);


            if (e.Image != "")
            {


                var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Event) and exists(n.Id) and n.Id =~ {Id}" +
                                                                "set n.Caption={Caption}, n.Description={Description}, n.City={City},"
                                                                + "n.Location={Location}, n.Image={Image}, n.Date={Date},  n.DateAndTime={DateAndTime}, n.Time={Time}  return n",
                                                                queryDict, CypherResultMode.Set);
                EventModel ev = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList().First();
                return ev;
            }
            else
            {
                var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Event) and exists(n.Id) and n.Id =~ {Id}" +
                                                                "set n.Caption={Caption}, n.Description={Description}, n.City={City},"
                                                                + "n.Location={Location}, n.Date={Date},  n.DateAndTime={DateAndTime}, n.Time={Time}  return n",
                                                                queryDict, CypherResultMode.Set);
                EventModel ev = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList().First();
                return ev;
            }


        }

        #endregion

        #region GetEvent

        //get by id
        public EventModel GetById(string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Event) and exists(n.Id) and n.Id=~{Id} return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            if (obj.Count > 0)
                return obj.First();

            return null;
        }

        public UserModel GetCreator(string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (u:User)-[:Created]->(n:Event) where  exists(n.Id) and n.Id=~{Id} return u",
                                                            queryDict, CypherResultMode.Set);

            List<UserModel> obj = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            if (obj.Count > 0)
                return obj.First();

            return null;
        }

        //get events by date
        public List<EventModel> GetByDate(string date)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Date", date);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Event) and exists(n.Date) and n.Date =~ {Date} and n.Visibility=~'Public' return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        //get events by city
        public List<EventModel> GetByCity(string city)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("City", city);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Event) and exists(n.City) and n.City =~ {City} and n.Visibility=~'Public' return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        //get events by type
        public List<EventModel> GetByType(string type)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Type", type);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Category)<-[:BelongsTo]-(i:Event) WHERE n.Name =~ {Type} and i.Visibility=~'Public' return i",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public List<EventModel> GetCityDateType(string type, string city, string date)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Type", type);
            queryDict.Add("City", city);
            queryDict.Add("Date", date);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Category)<-[:BelongsTo]-(i:Event) WHERE n.Name =~ {Type} and i.City=~{City} and i.Date=~{Date} and i.Visibility=~'Public' return i",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public List<EventModel> GetCityType(string city, string type)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Type", type);
            queryDict.Add("City", city);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Category)<-[:BelongsTo]-(i:Event) WHERE n.Name =~ {Type} and i.City=~{City} and i.Visibility=~'Public' return i",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public List<EventModel> GetCityDate(string date, string city)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("City", city);
            queryDict.Add("Date", date);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (i:Event) WHERE i.City=~{City} and i.Date=~{Date} and i.Visibility=~'Public' return i",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public List<EventModel> GetDateType(string date, string type)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Type", type);
            queryDict.Add("Date", date);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Category)<-[:BelongsTo]-(i:Event) WHERE n.Name =~ {Type} and i.Date=~{Date} and i.Visibility=~'Public' return i",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        //get events from one user
        public List<EventModel> GetByUser(string token)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[:Created]-(i:User) WHERE i.Token =~ {Token} return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public List<EventModel> GetCreated(string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[:Created]-(i:User) WHERE i.Id =~ {Id} return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public List<EventModel> GetForSpecificUser(string token)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (u:User)-[:Likes]->(c:Category)<-[:BelongsTo]-(i:Event)<-[:Created]-(k:User) WHERE u.Token =~ {Token} and NOT(k.Token=~u.Token) return i",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }



        #endregion

        #region InterestedRegion

        public List<EventModel> GetInterested(string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[:InterestedIn]-(i:User) WHERE i.Id =~ {Id} return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public List<EventModel> GetInterestedIn(string token)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[:InterestedIn]-(i:User) WHERE i.Token =~ {Token} return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public List<UserModel> GetInterestedFriends(string token, string eventId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);
            queryDict.Add("Id", eventId);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[:InterestedIn]-(i:User)-[:IsFriendsWith]-(k:User) WHERE k.Token =~ {Token} and n.Id=~{Id} return i",
                                                            queryDict, CypherResultMode.Set);

            List<UserModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            return list;
        }

        public bool GetInterested(string token, string eventId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);
            queryDict.Add("Id", eventId);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (u:User)-[r:InterestedIn]->(e:Event) WHERE u.Token=~{Token} and e.Id =~ {Id} return e",
                                                            queryDict, CypherResultMode.Set);

            

            if (((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList().Count>0)
                return true;
            return false;
        }

        public void CreateInterested(string token, string eventId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);
            queryDict.Add("Id", eventId);



            var q1 = new Neo4jClient.Cypher.CypherQuery("match (u:User),(c:Event) where u.Token=~{Token} and c.Id=~{Id} create (u)-[r:InterestedIn]->(c)",
                                                            queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
        }

        public void DeleteInterested(string token, string eventId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Token", token);
            queryDict.Add("Id", eventId);

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[r:InterestedIn]-(i:User) WHERE i.Token =~ {Token} and n.Id=~{Id} delete r",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
        }

        #endregion

        #region InviteRegiton

        public List<EventModel> GetInvitedTo(string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[:InvitedTo]-(i:User) WHERE i.Id =~ {Id} return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        public bool ExistsInvitedTo(string idUser, string idEvent)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("IdUser", idUser);
            queryDict.Add("IdEvent", idEvent);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (u:User)-[r:InvitedTo]->(e:Event) WHERE u.Id=~{IdUser} and e.Id =~ {IdEvent} return e",
                                                            queryDict, CypherResultMode.Set);



            if (((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList().Count > 0)
                return true;
            return false;
        }

        public void AddInvitedTo(string idUser, string idEvent)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("UserId", idUser);
            queryDict.Add("EventId", idEvent);
            queryDict.Add("date", DateTime.Now.ToString());




            var query = new Neo4jClient.Cypher.CypherQuery("match (u:User),(c:Event) where u.Id=~{UserId} and c.Id=~{EventId} create (u)-[r:InvitedTo{date:{date}}]->(c)",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(query);

        }

        public List<EventModel> GetInvitedByFriend(string id, string token)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", id);
            queryDict.Add("Token", token);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (x:User)-[:Created]->(n:Event)<-[:InvitedTo]-(i:User) WHERE i.Token =~ {Token} and x.Id=~{Id} return n",
                                                            queryDict, CypherResultMode.Set);

            List<EventModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return list;
        }

        #endregion

        #region Going

        public bool GetGoing(string token, string eventId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);
            queryDict.Add("Id", eventId);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (u:User)-[r:Going]->(e:Event) WHERE u.Token=~{Token} and e.Id =~ {Id} return e",
                                                            queryDict, CypherResultMode.Set);



            if (((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList().Count > 0)
                return true;
            return false;
        }

        public void CreateGoing(string token, string eventId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);
            queryDict.Add("Id", eventId);



            var q1 = new Neo4jClient.Cypher.CypherQuery("match (u:User),(c:Event) where u.Token=~{Token} and c.Id=~{Id} create (u)-[r:Going]->(c)",
                                                            queryDict, CypherResultMode.Projection);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
        }

        public void DeleteGoing(string token, string eventId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("Token", token);
            queryDict.Add("Id", eventId);

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event)<-[r:Going]-(i:User) WHERE i.Token =~ {Token} and n.Id=~{Id} delete r",
                                                            queryDict, CypherResultMode.Set);

            ((IRawGraphClient)DataLayer.getClient()).ExecuteCypher(q1);
        }

        public List<UserModel> GetGoingFriends(string token, string eventId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Token", token);
            queryDict.Add("Id", eventId);


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:User)-[:IsFriendsWith]-(i:User)-[:Going]->(k:Event) WHERE n.Token =~ {Token} and k.Id=~{Id} return i",
                                                            queryDict, CypherResultMode.Set);

            List<UserModel> list = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<UserModel>(query).ToList();

            return list;
        }
        #endregion

        public List<EventModel> GetEvents()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Event) return n",
                                                            dict, CypherResultMode.Set);

            List<EventModel> e = ((IRawGraphClient)DataLayer.getClient()).ExecuteGetCypherResults<EventModel>(query).ToList();

            return e;
        }

    }
}
