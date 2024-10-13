using Ineventation.Business.Objects.Requests;
using Ineventation.Data.Models;
using Ineventation.WebApp.ViewModels;
using System.Web.Mvc;
using System.Collections.Generic;
using PagedList;
using Ineventation.Business.Objects.Responses;
using System.IO;
using System;

namespace Ineventation.WebApp.Controllers
{
    public class UserController : BaseController
    {
        public UserController():base()
        {
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult CreateProfile(RegisterModel user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Register", user);
            }
            var extension = "";
            if(user.Image!=null)
            {
                extension = Path.GetExtension(user.Image.FileName);
            }
            var result = userService.Register(new RegisterObject { FirstName = user.FirstName, LastName = user.LastName,
                Email = user.Email, Password = user.Password, Username = user.Username,
                City = user.City, AccountType = user.AccountType, Image= extension });

            if (user.Image != null)
            {
                string fileName = result.Target;
                extension = Path.GetExtension(user.Image.FileName);
                fileName = fileName + extension;
                string path = "~/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);

                if (!Directory.Exists(Server.MapPath("~/Image/")))
                {
                    Directory.CreateDirectory("~/Image");
                }

                user.Image.SaveAs(fileName);
            }



            if (result.Status)
            {
                return RedirectToAction("Index","Home");
            }
            return View("Register", user);
        }

        public ActionResult Login(LoginModel loginObject)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index","Home", loginObject);
            }
            
            var result = userService.Login(new LoginObject { Username=loginObject.Username, Password=loginObject.Password});

            if (Session["userToken"] == null && result.Status)
            {
                Session["userToken"] = result.Target.Token;

                return RedirectToAction("UserProfile", "User", new { id = result.Target.Id});
            }

            return RedirectToAction("Index", "Home", loginObject);
        }

        public ActionResult UserProfile(string id, string message)
        {
            
            if (Session["userToken"] != null)
            {
                ProfileResponse result = new ProfileResponse();

                if (id != null)
                {
                    result = userService.LoadProfile(new LoadProfileObject { Id = id });
                }
                else
                {
                    result = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() });

                }
                var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;

                string myId = user.Id;

                bool myProfile = (result.Target.Token == Session["userToken"].ToString()) ? true : false;

                bool friends = false;

                bool sentFriendRequest = false;

                bool receiverRequest = false;

                result.Target.Organises.ForEach(x=> {
                    if(x.Image!="")
                    {
                        x.Image= "https://localhost:44365/Image/" + x.Id + x.Image;
                    }
                });

                

                result.Target.InterestedIn.ForEach(x => {
                    if (x.Image != "")
                    {
                        x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                    }
                });

                if (!myProfile)
                {
                    friends = userService.GetFriends(new LoadFriendsObject { Token = Session["userToken"].ToString(), FriendId = result.Target.Id }).Target;
                    sentFriendRequest = userService.GetFriendRequest(myId,id).Target;
                    receiverRequest = userService.GetFriendRequest(id,myId).Target;
                }

                if(friends)
                {
                    result.Target.Invited = eventService.GetInvitedByFriend(id, Session["userToken"].ToString()).Target;
                }

                result.Target.Invited.ForEach(x => {
                    if (x.Image != "")
                    {
                        x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                    }
                });

                string imagePath = "";

                if(result.Target.Image!="")
                {
                    imagePath = "https://localhost:44365/Image/" + result.Target.Id + result.Target.Image;
                }
                if (result.Status)
                {
                    return View(new UserProfileView
                    {
                        UserAccountType = user.AccountType,
                        MyId = myId,
                        Id = result.Target.Id,
                        Username = result.Target.Username,
                        FirstName = result.Target.FirstName,
                        LastName = result.Target.LastName,
                        Organises = result.Target.Organises,
                        InterestedIn = result.Target.InterestedIn,
                        City = result.Target.City,
                        AccountType = result.Target.AccountType,
                        MyProfile = myProfile,
                        Friends = friends,
                        Invited = result.Target.Invited,
                        SentRequest = sentFriendRequest,
                        ReceivedRequest= receiverRequest,
                        Image=imagePath,
                        Message = message == null ? "" : message

                    });
                }
            }

            return RedirectToAction("LogOut");
        }

        public ActionResult UpdateProfile(string id)
        {
            if (Session["userToken"] == null)
            {
                return RedirectToAction("Index", "Home");//sa error message
            }
            var token = Session["userToken"].ToString(); 

            var result = userService.GetUserForUpdate(new LoadProfileObject { Id = id});

            var rezultat = categoryService.GetFavorites(new GetCategoriesForUser { Token = token });

            var res = categoryService.GetCategories();

            if (res.Status && rezultat.Status && result.Status)
            {

                return View(new UpdateProfileViewModel
                {
                    AccountType = result.Target.AccountType,
                    MyId = id,
                    Id = id,
                    FirstName = result.Target.FirstName,
                    LastName = result.Target.LastName,
                    City = result.Target.City,
                    Favorites = rezultat.Target,
                    Categories = res.Target
                });
            }

            return RedirectToAction("Index", "Home");//sa error message


        }

        public ActionResult UpdateProfileInfo(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UpdateProfile", model);

            }
            if (Session["userToken"] != null)
            {
                string extension = "";

                if (model.Image != null)
                {
                    string fileName = model.MyId;
                    extension = Path.GetExtension(model.Image.FileName);
                    fileName = fileName + extension;
                    string path = "~/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);

                    if (!Directory.Exists(Server.MapPath("~/Image/")))
                    {
                        Directory.CreateDirectory("~/Image");
                    }

                    model.Image.SaveAs(fileName);
                }

                var updateObject = new UpdateProfileObject { FirstName = model.FirstName, LastName = model.LastName, City = model.City, ImagePath = extension, Token = Session["userToken"].ToString(), categories = model.checkedCategories };

                var result = userService.UpdateProfile(updateObject);

                if (result.Status)
                {
                    return RedirectToAction("UserProfile", "User", new { id = model.Id });
                }
            }

            return RedirectToAction("LogOut");
        }

        public ActionResult DeleteProfile(string userId)
        {
            if (Session["userToken"] != null)
            {
               
                var result = userService.DeleteProfile(new DeleteProfileObject { Id = userId });

                if (result.Status)
                {
                    return RedirectToAction("LogOut");
                }

                return RedirectToAction("UserProfile", "User", new { id = userId });
            }
            return RedirectToAction("LogOut");


        }

        public ActionResult Interested(string id, bool? interested)
        {
            if (Session["userToken"] != null && id != null)
            {
                if (interested!=null && interested==false)
                {
                    var res = userService.CreateInterested(new InterestedObject { Token = Session["userToken"].ToString(), Event = id });
                }
                else if (interested != null && interested == true)
                {
                    var res = userService.DeleteInterested(new InterestedObject { Token = Session["userToken"].ToString(), Event = id });
                }


            }

            return RedirectToAction("EventDetails","Event", new { idEvent = id});
        }

        public ActionResult SearchUser(int? Page_No, string Search)
        {
            if (Session["userToken"] != null)
            {
                if (Search == null)
                {
                    Search = "";
                }

                var result = userService.GetUsers(new SearchObject {token = Session["userToken"].ToString(), search = Search });
                if (result.Status)
                {
                    var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;

                    int pageSize = 4;
                    int no_of_page = (Page_No ?? 1);
                    result.Target.ForEach(x => {
                        if (x.Image != "")
                        {
                            x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                        }
                    });
                    return View(new SearchModel {
                        AccountType=user.AccountType,
                        MyId=user.Id, 
                        list = result.Target.ToPagedList(no_of_page, pageSize), Search = Search 
                    });
                }
            }
            return RedirectToAction("LogOut");
        }

        public ActionResult FriendList(string idUser,string Case,string idEvent, int? Page_No)
        {
            if (Session["userToken"] != null)
            {
                UserListResponse list = new UserListResponse();
                switch (Case) {
                    case "invited":
                        {
                            list = userService.GetGoingFriends(new InterestedObject
                            {
                                Token = Session["userToken"].ToString(),
                                Event = idEvent
                            });

                            
                            break;
                        }
                    case "Public":
                        {
                            list = userService.GetInterestedFriends(new InterestedObject
                            {
                                Token = Session["userToken"].ToString(),
                                Event = idEvent
                            });

                            
                            
                            break;
                        }
                    default:
                        {
                            list = userService.GetFriendships(new LoadFriendshipsObject { Id = idUser });
                            
                            break;
                        }
            }
                if (list.Status)
                {
                    var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;
                    list.Target.ForEach(x =>
                    {
                        if (x.Image != "")
                        {
                            x.Image = "https://localhost:44365/Image/" + x.Id + x.Image;
                        }
                    });
                    int pageSize = 9;
                    int no_of_page = (Page_No ?? 1);
                    return View(new FriendListView { AccountType = user.AccountType, MyId = idUser, Id = idUser, Friends = list.Target.ToPagedList(no_of_page, pageSize) });
                }
            }
            return RedirectToAction("LogOut");
        }

        public ActionResult RemoveFriend(string idFriend)
        {
            if (Session["userToken"] != null)
            {

                var result = userService.RemoveFriend(new LoadFriendsObject { Token= Session["userToken"].ToString(),FriendId = idFriend });
                if (result.Status)
                {
                    return RedirectToAction("UserProfile","User", new { id=idFriend});
                }
            }
            return RedirectToAction("LogOut");
        }

        public ActionResult ReportProfile(string idReport)
        {
            if (Session["userToken"] != null)
            {

                var result = userService.ReportProfile(new LoadFriendsObject { Token = Session["userToken"].ToString(), FriendId = idReport });
                if (result.Status)
                {
                    return RedirectToAction("UserProfile", "User", new { id = idReport, message = "You have reported this user."});
                }
                return RedirectToAction("UserProfile","User", new { id=idReport});
            }
            return RedirectToAction("LogOut");
        }

        public ActionResult AdminOptions(int? page1, int? page2)
        {
            if (Session["userToken"] != null)
            {
                var resultCategories = categoryService.GetCategories();

                var resultUser = userService.GetReportedUsers();

                int p1 = page1 ?? 1;
                int p2 = page2 ?? 1;
                int size = 9;

                var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;

                return View(new AdminOptionsView
                {
                    AccountType= user.AccountType,
                    MyId = user.Id,
                    Categories = resultCategories.Target.ToPagedList(p1, size),
                    ReportedUsers = resultUser.Target.ToPagedList(p2, size)
                });

            }
            return RedirectToAction("LogOut");
        }

        public ActionResult AddCategory(AdminOptionsView model)
        {
           
            if (Session["userToken"] != null)
            {

                var result = categoryService.AddCategory(model.NewCategory);

                if(result.Status)
                {
                    return RedirectToAction("AdminOptions");
                }

            }
            return RedirectToAction("LogOut");
        }

        public ActionResult UserCompleteProfile(string idUser, int? pageO, int? pageL, int? pageF, int? pageI)
        {
            if (Session["userToken"] != null)
            {
                int pO = pageO ?? 1;
                int pL = pageL ?? 1;
                int pF = pageF ?? 1;
                int pI = pageI ?? 1;
                int size = 2;

                var result = userService.LoadProfile(new LoadProfileObject { Id=idUser});

                var res = userService.GetNumberOfReports(idUser);

                if (result.Status && res.Status)
                {
                    var user = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target;
                    if(result.Target.Image != "")
                        {
                            result.Target.Image = "https://localhost:44365/Image/" + result.Target.Id + result.Target.Image;
                        }

                    return View(new UserCompleteProfileView {
                        UserAccountType = user.AccountType,
                        MyId = user.Id,
                        Id = result.Target.Id,
                        Username = result.Target.Username,
                        FirstName = result.Target.FirstName,
                        LastName = result.Target.LastName,
                        Organises = result.Target.Organises.ToPagedList(pO, size),
                        InterestedIn = result.Target.InterestedIn.ToPagedList(pI, size),
                        City = result.Target.City,
                        AccountType = result.Target.AccountType,
                        Friends = result.Target.IsFriendsWith.ToPagedList(pF, size),
                        Likes = result.Target.Favorites.ToPagedList(pL, size),
                        NumberOfReports = res.Target,
                        Image = result.Target.Image
                    });
                }
          
            }
            return RedirectToAction("LogOut");
        }

        public ActionResult BecomeFriends(string idUser)
        {
            if (Session["userToken"] != null)
            {
                var myId = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target.Id; 
                var result = userService.AcceptFriendRequest(myId, idUser);

                if(result.Status)
                {
                    return RedirectToAction("UserProfile","User",new { id=idUser});
                }

            }
            return RedirectToAction("LogOut");
        }

        public ActionResult RejectRequest(string UserId)
        {
            if (Session["userToken"] != null)
            {
                var myId = userService.LoadProfile(new LoadProfileObject { Token = Session["userToken"].ToString() }).Target.Id;

                var result = userService.RejectRequest(myId, UserId);

                if (result.Status)
                {
                    return RedirectToAction("UserProfile", "User", new { id = UserId });
                }

            }
            return RedirectToAction("LogOut");
        }

        public ActionResult Going(string id, bool? going)
        {
            if (Session["userToken"] != null && id != null)
            {
                if (going != null && going == false)
                {
                    var res = userService.CreateGoing(new InterestedObject { Token = Session["userToken"].ToString(), Event = id });
                }
                else if (going != null && going == true)
                {
                    var res = userService.DeleteGoing(new InterestedObject { Token = Session["userToken"].ToString(), Event = id });
                }


            }

            return RedirectToAction("EventDetails", "Event", new { idEvent = id });
        }
    }

    
}