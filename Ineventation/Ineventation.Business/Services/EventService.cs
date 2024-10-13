using Ineventation.Business.Objects.Requests;
using Ineventation.Business.Objects.Responses;
using Ineventation.Data.Models;
using System;
using System.Collections.Generic;

namespace Ineventation.Business.Services
{
    public class EventService:BaseService
    {
        public EventService() : base()
        {
        }

        #region Basic
        public StringResponse CreateEvent(CreateEventObject sentObject)
        {
            try
            {
                var creator = userRepository.GetByToken(sentObject.Creator);
                string visibility = "";

                if (creator.AccountType == "promoter")
                {
                    visibility = "Public";
                }
                else
                {
                    visibility = "Private";
                }
                var model = new EventModel
                {
                    Caption = sentObject.Caption,
                    Description = sentObject.Description,
                    Location = sentObject.Location,
                    City = sentObject.City,
                    Date = sentObject.DateAndTime.ToShortDateString(),
                    Time = sentObject.DateAndTime.ToShortTimeString(),
                    DateAndTime = sentObject.DateAndTime,
                    Creator = creator,
                    Visibility = visibility,
                    Image=sentObject.Image
                };
                var result = eventRepository.CreateEvent(model);

                categoryRepository.CreateBelongsTo(result.Id,sentObject.Type);

                return new StringResponse { Status = true, Message = "Successful.", Target = result.Id };
                
            }
            catch (Exception)
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };

            }
        } 

        public EventResponse GetEvent(string id)
        {
            try
            {
                var result = eventRepository.GetById(id);

                result.Creator = eventRepository.GetCreator(result.Id);
                    
                
                return new EventResponse { Status = true, Message = "Successful.", Target = result };

            }
            catch (Exception )
            {
                return new EventResponse { Status = false, Message = "Internal server error.", Code = 501 };

            }
        }

        public StringResponse DeleteEvent(DeleteEventObject request)
        {
            try
            {
                eventRepository.DeleteEvent(request.Id);

                return new StringResponse { Status = true, Message = "Successful." };

            }
            catch (Exception )
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };

            }
        }

        public EventResponse UpdateEvent(UpdateEventObject request)
        {
            try
            {
                if (request != null)
                {
                    categoryRepository.UpdateBelongsTo(request.Id, request.Type);

                    var result = eventRepository.UpdateEvent(new EventModel {
                        Id = request.Id,
                        Caption = request.Caption,
                        Description = request.Description,
                        Location = request.Location,
                        Date = request.DateAndTime.ToShortDateString(),
                        Time = request.DateAndTime.ToShortTimeString(),
                        DateAndTime = request.DateAndTime,
                        City = request.City,
                        Image=request.Image
                    });

                    
                    if (result != null)
                    {
                        return new EventResponse { Status=true, Message="Success.", Target=result};
                    }

                    return new EventResponse {Status=false, Message="Failed to update." };
                }
                return new EventResponse { Status = false,  Message = "Request error." };
            }
            catch(Exception)
            {
                return new EventResponse { Status=false, Code=501, Message="Internal server error."};
            }
        }

        public EventListResponse GetInvitedByFriend(string friendId, string myToken)
        {
            try
            {
                var result = eventRepository.GetInvitedByFriend(friendId, myToken);


                if (result != null)
                {
                    return new EventListResponse { Status = true, Message = "Success.", Target = result };
                }

                return new EventListResponse { Status = false, Message = "Failed to load." };

            }
            catch (Exception)
            {
                return new EventListResponse { Status = false, Code = 501, Message = "Internal server error." };
            }
        }


        #endregion

        #region Search

        public EventListResponse DiscoverEvents(LoadEventsObject req)
        {
            try
            {
                if(req!=null)
                {
                    var result = eventRepository.GetForSpecificUser(req.Token);
                    result.RemoveAll(x => DateTime.Compare(x.DateAndTime.Date, DateTime.Now.Date) < 0);
                    result.RemoveAll(x=>x.Visibility=="Private");

                    if (result!=null)
                    {
                        return new EventListResponse { Status = true, Message = "Success.", Target= result };
                    }
                    return new EventListResponse { Status = false, Message = "Failed to get events." };
                }
                return new EventListResponse { Status = false,  Message = "Request not provided." };

            }
            catch (Exception)
            {
                return new EventListResponse { Status = false, Code = 501, Message = "Internal server error." };

            }
        }

        public EventListResponse DiscoverEventsSearch(LoadEventsSearchObject req)
        {
            try
            {
                if (req != null)
                {
                    var result = new List<EventModel>();
                    if (req.Category!=null && req.City != null && req.Date.ToString() != "01/01/0001 12:00:00 AM")
                    {
                        result = eventRepository.GetCityDateType(req.Category, req.City, req.Date.ToShortDateString());
                    }
                    else if (req.City!=null && req.Category!=null)
                    {
                        result = eventRepository.GetCityType(req.City, req.Category);
                    }
                    else if (req.Date.ToString() != "01/01/0001 12:00:00 AM" && req.City!=null)
                    {
                        result = eventRepository.GetCityDate(req.Date.ToShortDateString(), req.City);
                    }
                    else if (req.Date.ToString() != "01/01/0001 12:00:00 AM" && req.Category != null)
                    {
                        result = eventRepository.GetDateType(req.Date.ToShortDateString(), req.Category);
                    }
                    else if (req.Date.ToString() != "01/01/0001 12:00:00 AM")
                    {
                        result = eventRepository.GetByDate(req.Date.ToShortDateString());
                    }
                    else if (req.City != null)
                    {
                        result = eventRepository.GetByCity(req.City);

                    }
                    else if (req.Category != null)
                    {
                        result = eventRepository.GetByType(req.Category);
                    }
                    result.RemoveAll(x => DateTime.Compare(x.DateAndTime.Date, DateTime.Now.Date) < 0);


                    if (result.Count>0)
                    {
                        return new EventListResponse { Status = true, Message = "Success.", Target = result };
                    }
                    return new EventListResponse { Status = true, Message = "No events found.", Target = result};
                }
                return new EventListResponse { Status = false, Message = "Request not provided." };

            }
            catch (Exception)
            {
                return new EventListResponse { Status = false, Code = 501, Message = "Internal server error." };

            }
        }

        #endregion

        public EventListResponse GetEvents()
        {
            try
            {
                var res = eventRepository.GetEvents();

                return new EventListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new EventListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

    }
}
