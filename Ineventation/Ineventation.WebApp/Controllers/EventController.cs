using Ineventation.Business.Objects.Requests;
using Ineventation.Data.Models;
using Ineventation.WebApp.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;
using PagedList;
using System;
using Ineventation.Business.Objects.Responses;
using System.IO;

namespace Ineventation.WebApp.Controllers
{
    public class EventController : BaseController
    {
        public EventController():base()
        {
        }

        public ActionResult CreateEvent()
        {
            if (Session["userToken"] != null)
            {
                var result = categoryService.GetCategories();
                if (result.Status)
                {
                    List<string> categories = new List<string>();
                    result.Target.ForEach(x => categories.Add(x.Name));
                    var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;
                    return View(new CreateEventView
                    {
                        MyId = user.Id,
                        AccountType = user.AccountType,
                        Categories = categories
                    }); ;
                }
            }

            return RedirectToAction("LogOut");
        }

        public ActionResult AddEvent(CreateEventView Event)
        {
            if (Session["userToken"] != null)
            {

                if (!ModelState.IsValid)
                {
                    return RedirectToAction("CreateEvent", Event);
                }
                string extension = "";

                if (Event.Image != null)
                {
                    extension = Path.GetExtension(Event.Image.FileName);
                }

                CreateEventObject sendObject = new CreateEventObject
                {
                    Caption = Event.Caption,
                    Description = Event.Description,
                    Location = Event.Location,
                    City = Event.City,
                    DateAndTime = Event.DateAndTime,
                    Type = Event.Type,
                    Creator = Session["userToken"].ToString(),
                    Image = extension
                };

                var result = eventService.CreateEvent(sendObject);

                if(Event.Image!= null)
                {
                    string fileName = result.Target;
                    extension = Path.GetExtension(Event.Image.FileName);
                    fileName = fileName + extension;
                    string path = "~/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);

                    if (!Directory.Exists(Server.MapPath("~/Image/")))
                    {
                        Directory.CreateDirectory("~/Image");
                    }

                    Event.Image.SaveAs(fileName);
                }

                if (result.Status)
                {
                    return RedirectToAction("EventDetails", "Event", new { idEvent = result.Target });
                }
            }

            return RedirectToAction("LogOut");

        }
        
        public ActionResult UpdateEvent(string id)
        {
            if (Session["userToken"] != null)
            {

                if (id != null)
                {
                    var result = eventService.GetEvent(id);
                    var res = categoryService.GetCategories();

                    if (!(res.Status && result.Status))
                    {
                        return RedirectToAction("LogOut");//doslo do greske
                    }
                    List<string> categories = new List<string>();
                    res.Target.ForEach(x => categories.Add(x.Name));

                    var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;

                    return View(new CreateEventView
                    {
                        MyId = user.Id,
                        AccountType = user.AccountType,
                        Id = result.Target.Id,
                        Categories = categories,
                        Caption = result.Target.Caption,
                        Description = result.Target.Description,
                        Location = result.Target.Location,
                        City = result.Target.City,
                        DateAndTime = result.Target.DateAndTime,
                        Type = result.Target.Type.Name,
                        
                    });
                }
            }
            return RedirectToAction("LogOut");
        }

        public ActionResult UpdateEventInfo(CreateEventView Event)
        {

            if (Session["userToken"] != null)
            {
                string extension = "";

                if (Event.Image != null)
                {
                    string fileName = Event.Id;
                    extension = Path.GetExtension(Event.Image.FileName);
                    fileName = fileName + extension;
                    string path = "~/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);

                    if (!Directory.Exists(Server.MapPath("~/Image/")))
                    {
                        Directory.CreateDirectory("~/Image");
                    }

                    Event.Image.SaveAs(fileName);
                }

                var result = eventService.UpdateEvent(new UpdateEventObject
                {
                    Id = Event.Id,
                    Caption = Event.Caption,
                    Description = Event.Description,
                    Location = Event.Location,
                    DateAndTime = Event.DateAndTime,
                    City = Event.City,
                    Type = Event.Type,
                    Image = extension
                });

                if (result.Status)
                {
                    return RedirectToAction("EventDetails", "Event", new { idEvent = result.Target.Id });

                }
            }

            return RedirectToAction("LogOut");


        }

        public ActionResult DeleteEvent(EventDetailsViewModel Event)
        {
            if (Session["userToken"] == null)
            {
                return RedirectToAction("LogOut");// sa porukom
            }

            DeleteEventObject deleteObject = new DeleteEventObject{ Id = Event.Id };

            var result = eventService.DeleteEvent(deleteObject);

            if (result.Status)
            {

                return RedirectToAction("UserProfile", "User", new { id = Event.MyId});//sa porukom o uspesnom brisanju
            }

            return RedirectToAction("EventDetails", "Event", new { idEvent = Event.Id});



        }

        public ActionResult Discover(string id, int? Page_No, DiscoverViewModel model)
        {
            if (Session["userToken"] != null)
            {
                var res = categoryService.GetCategories();
                List<string> categories = new List<string>();
                res.Target.ForEach(x => categories.Add(x.Name));
                int pageSize = 9;
                int no_of_page = (Page_No ?? 1);
                var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;

                if (model.Tag == "search" && (model.City != null || model.DateAndTime != null || model.SelectedCategory != null))
                {
                    var result = eventService.DiscoverEventsSearch(new LoadEventsSearchObject
                    {
                        Category = model.SelectedCategory,
                        City = model.City,
                        Date = model.DateAndTime
                    });
                    
                    if (result.Status)
                    {
                        result.Target.ForEach(x => {
                            if (x.Image != "")
                            {
                                x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                            }
                        });
                        return View(new DiscoverViewModel
                        {
                            AccountType = user.AccountType,
                            MyId = user.Id,
                            list = result.Target.ToPagedList(no_of_page,pageSize),
                            Categories = categories,
                            City = model.City,
                            DateAndTime = model.DateAndTime,
                            SelectedCategory = model.SelectedCategory,
                        }) ;
                    }
                }
                else
                {
                    
                    var result = eventService.DiscoverEvents(new LoadEventsObject { Token = Session["userToken"].ToString() });

                    if (result.Status && res.Status)
                    {
                        result.Target.ForEach(x => {
                            if (x.Image != "")
                            {
                                x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                            }
                        });
                        return View(new DiscoverViewModel
                        {
                            AccountType = user.AccountType,
                            MyId = user.Id,
                            list = result.Target.ToPagedList(no_of_page,pageSize),
                            Categories = categories
                        });
                    }


                }
            }
            return RedirectToAction("LogOut");
        }

        public ActionResult EventDetails(string idEvent)
        {
            if(Session["userToken"]!=null)
            {
                var result = eventService.GetEvent(idEvent);

                if (result.Status)
                {
                    var myEvent = (result.Target.Creator.Token == Session["userToken"].ToString()) ? true : false;
                    var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;

                    BoolResponse resp =new BoolResponse();
                    List<UserModel> list = new List<UserModel>();
                    var going = false;
                    var invited = userService.ExistsInvited(user.Id,result.Target.Id).Target;

                    if (result.Target.Visibility == "Public")
                    {
                        resp = userService.GetInterested(new InterestedObject { Token = Session["userToken"].ToString(), Event = idEvent });
                        list = userService.GetInterestedFriends(new InterestedObject
                        {
                            Token = Session["userToken"].ToString(),
                            Event = idEvent
                        }).Target;

                        list.ForEach(x => { if (x.Image != "") x.Image = "https://localhost:44365/Image/" + x.Id + x.Image; });

                    }
                    else if (invited|| myEvent)
                    {
                        going = userService.GetGoing(new InterestedObject { Token = Session["userToken"].ToString(), Event = result.Target.Id }).Target;
                        list = userService.GetGoingFriends(new InterestedObject
                        {
                            Token = Session["userToken"].ToString(),
                            Event = idEvent
                        }).Target;

                        list.ForEach(x => { if (x.Image != "") x.Image = "https://localhost:44365/Image/" + x.Id + x.Image; });
                    }
                    
                    string imagePath = "";

                    if (result.Target.Image != "")
                    {
                        imagePath = "https://localhost:44365/Image/" + result.Target.Id + result.Target.Image;
                    }
                    return View(new EventDetailsViewModel
                    {
                        MyId = user.Id,
                        AccountType = user.AccountType,
                        Caption = result.Target.Caption,
                        Description = result.Target.Description,
                        Location = result.Target.Location,
                        City = result.Target.City,
                        Id = result.Target.Id,
                        DateAndTime = result.Target.DateAndTime.ToString(),
                        Date=result.Target.Date,
                        Time=result.Target.Time,
                        Type = result.Target.Type.Name,
                        CreatedBy = result.Target.Creator.Username,
                        Interested = resp.Target,
                        MyEvent = myEvent,
                        Going = going,
                        Invited=invited,
                        Image = imagePath,
                        Visibility = result.Target.Visibility,
                        list = list
                    });

                }
            }

            return RedirectToAction("LogOut");
        }

        public ActionResult EventList(string idUser, int? Page_No, string Case)
        {
            var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;

            if (Session["userToken"] != null)
            {
                switch (Case)
                {
                    case "created":
                        {
                            var result = userService.GetCreatedEvents(idUser);
                            if (result.Status)
                            {
                                result.Target.ForEach(x => {
                                    if (x.Image != "")
                                    {
                                        x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                                    }
                                });
                                int pageSize = 9;
                                int no_of_page = (Page_No ?? 1);
                                return View(new EventListView
                                {
                                    AccountType=user.AccountType,
                                    MyId = user.Id,
                                    Id = idUser, 
                                    Events = result.Target.ToPagedList(no_of_page, pageSize), 
                                    Case= Case 
                                });
                            }
                            break;
                        }
                    case "interested":
                        {
                            var result = userService.GetInterestedEvents(idUser);
                            if (result.Status)
                            {
                                result.Target.ForEach(x => {
                                    if (x.Image != "")
                                    {
                                        x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                                    }
                                });
                                int pageSize = 9;
                                int no_of_page = (Page_No ?? 1);
                                return View(new EventListView
                                {
                                    MyId = user.Id,
                                    AccountType = user.AccountType,
                                    Id = idUser, 
                                    Events = result.Target.ToPagedList(no_of_page, pageSize), 
                                    Case = Case 
                                });
                            }
                            break;
                        }
                    case "invited":
                        {
                            if (idUser == user.Id)
                            {
                                var result = userService.GetInvitedEvents(idUser);
                                if (result.Status)
                                {
                                    result.Target.ForEach(x =>
                                    {
                                        if (x.Image != "")
                                        {
                                            x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                                        }
                                    });
                                    int pageSize = 9;
                                    int no_of_page = (Page_No ?? 1);
                                    return View(new EventListView
                                    {
                                        MyId = user.Id,
                                        AccountType = user.AccountType,
                                        Id = idUser,
                                        Events = result.Target.ToPagedList(no_of_page, pageSize),
                                        Case = Case
                                    });
                                }
                            }
                            else
                            {
                                var result = eventService.GetInvitedByFriend(idUser, Session["userToken"].ToString());
                                if (result.Status)
                                {
                                    result.Target.ForEach(x =>
                                    {
                                        if (x.Image != "")
                                        {
                                            x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                                        }
                                    });
                                    int pageSize = 9;
                                    int no_of_page = (Page_No ?? 1);
                                    return View(new EventListView
                                    {
                                        MyId = user.Id,
                                        AccountType = user.AccountType,
                                        Id = idUser,
                                        Events = result.Target.ToPagedList(no_of_page, pageSize),
                                        Case = Case
                                    });
                                }
                            }
                            break;
                        }
                }
            }
            return RedirectToAction("LogOut");
        }

        public ActionResult InviteFriends(string idEvent)
        {
            if (Session["userToken"]!=null)
            {
                var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;
                return View(new InviteView { MyId=user.Id, AccountType=user.AccountType, EventId= idEvent, Friends=user.IsFriendsWith});
            }
            return RedirectToAction("LogOut");

        }

        

    }

} 