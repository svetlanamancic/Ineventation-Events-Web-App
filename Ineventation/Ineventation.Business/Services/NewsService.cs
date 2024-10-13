using Ineventation.Business.Objects.Responses;
using Ineventation.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineventation.Business.Services
{
    public class NewsService : BaseService
    {
        public NewsService() : base()
        {

        }

        public void AddNews(string receiver, string id, string type)
        {
            try
            {
                switch (type)
                {
                    case "request":
                        {
                            string username = userRepository.GetById(id).Username;
                            string message = "You have received a friend request from " + username + ".";
                            newsRepository.CreateNews(message, receiver, id, type);
                            break;
                        }
                    case "invite":
                        {
                            string caption = eventRepository.GetById(id).Caption;
                            string message = "You have been invited to " + caption + ".";
                            newsRepository.CreateNews(message, receiver, id, type);
                            break;
                        }
                }
            }
            catch (Exception)
            {

            }
        }

        public NewsListResponse ReadNews(string token, string filter)
        {
            try
            {
                var result = newsRepository.GetNews(token);
                if (filter == "invite" || filter == "request")
                {
                    result.RemoveAll(x => x.Type != filter);
                }
                result.OrderByDescending(x => x.Time).ToList();

                return new NewsListResponse { Status = true, Message = "Load successfull.", Target = result };
            }
            catch (Exception)
            {
                return new NewsListResponse { Status = false, Message = "." };

            }
        }

        public BoolResponse DeleteNews(string token)
        {
            try
            {
                newsRepository.DeleteNews(token);
                return new BoolResponse { Status = true, Message = "Success." };

            }
            catch (Exception)
            {
                return new BoolResponse { Status = false, Message = "Error." };

            }
        }

    }
}
