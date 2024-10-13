using Ineventation.Business.Services;
using Ineventation.Data.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;

namespace Ineventation.WebApp
{
    [HubName("myHyb")]
    public class HubClass : Hub 
    {
        UserService userService = new UserService();
        NewsService newsService = new NewsService();

        [HubMethodName("sendRequest")]
        public void SendRequest(string idUser)
        {
            if (ConnectionMapping.Instance.Connections.Any(x => x.Key == idUser) && ConnectionMapping.Instance.Connections.Any(x => x.Value == Context.ConnectionId))
            {
                string connectionId = ConnectionMapping.Instance.Connections.Where(x => x.Key == idUser).ToList().First().Value;
                string senderId = ConnectionMapping.Instance.Connections.Where(x => x.Value == Context.ConnectionId).ToList().First().Key;
                if (userService.GetFriendRequest(senderId, idUser).Target == false)
                {
                    Clients.Client(connectionId).receiveRequest(senderId);
                    userService.SendFriendRequest(senderId, idUser);
                    newsService.AddNews(idUser,senderId,"request");
                }
            }
            else
            {
                string senderId = ConnectionMapping.Instance.Connections.Where(x => x.Value == Context.ConnectionId).ToList().First().Key;
                if (userService.GetFriendRequest(senderId, idUser).Target == false)
                {
                    userService.SendFriendRequest(senderId, idUser);
                    newsService.AddNews(idUser, senderId, "request");
                }
            }

        }

        [HubMethodName("invite")]
        public void Invite(string id, List<string> friends)
        {
            foreach(string idFriend in friends)
            {
                if (ConnectionMapping.Instance.Connections.Any(x => x.Key == idFriend))
                {
                    string connectionId = ConnectionMapping.Instance.Connections.Where(x => x.Key == idFriend).ToList().First().Value;
                    if (userService.ExistsInvited(idFriend, id).Target == false)
                    {
                        Clients.Client(connectionId).receiveInvite(id);
                        userService.AddInvited(idFriend, id);
                        newsService.AddNews(idFriend, id, "invite");
                    }
                }
                else
                {
                    if (userService.ExistsInvited(idFriend, id).Target == false)
                    {
                        userService.AddInvited(idFriend, id);
                        newsService.AddNews(idFriend, id, "invite");

                    }
                }
            }
        }

        [HubMethodName("connect")]
        public void Connect(string userId)
        {
            if(!ConnectionMapping.Instance.Connections.Any(x=>x.Key==userId))
            {
                ConnectionMapping.Instance.Connections.Add(userId, Context.ConnectionId);
            }
            else
            {
                ConnectionMapping.Instance.Connections.Remove(userId);
                ConnectionMapping.Instance.Connections.Add(userId, Context.ConnectionId);
            }
        }

        [HubMethodName("disconnect")]
        public void Disconnect()
        {
            if(ConnectionMapping.Instance.Connections.Any(x => x.Value == Context.ConnectionId))
            {
                string userId = ConnectionMapping.Instance.Connections.Where(x => x.Value == Context.ConnectionId).ToList().First().Key;
                ConnectionMapping.Instance.Connections.Remove(userId);
            }
        }
    }
}