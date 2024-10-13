using Ineventation.Business.Objects.Requests;
using Ineventation.Business.Objects.Responses;
using Ineventation.Data.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Ineventation.Business.Services
{
    public class UserService:BaseService
    {
        
        public UserService():base()
        {
            
        }

        #region AddUser

        public ProfileResponse Login(LoginObject loginObject)
        {
            try
            {
                var profile = userRepository.GetByUsername(loginObject.Username);

                if (profile!=null)
                {
                    using (var sha256 = SHA256.Create())
                    {
                        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(loginObject.Password));
                        var hash = BitConverter.ToString(hashedBytes).ToLower();

                        if (hash.Equals(profile.Password))
                        {
                            return new ProfileResponse { Status = true, Message = "Login successfull.", Target = profile };
                        }
                        else
                        {
                            return new ProfileResponse { Status = false, Message = "Invalid username or password." };
                        }
                    }
                }
                else
                {
                    return new ProfileResponse { Status = false, Message = "No profile with entered username."};
                }
            }
            catch (Exception)
            {
                return new ProfileResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }//radi

        public StringResponse Register(RegisterObject registerObject)//radi
        {
            try
            {
                var existing = userRepository.GetByUsername(registerObject.Username);
                
                if(existing!=null)
                {
                    return new StringResponse { Status = false, Message = "Username is taken."};
                }

                existing = userRepository.GetByEmail(registerObject.Email);

                if (existing != null)
                {
                    return new StringResponse { Status = false, Message = "Another account already uses entered email."};
                }

                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(registerObject.Password));
                    var hash = BitConverter.ToString(hashedBytes).ToLower();
                    var token = Guid.NewGuid().ToString();
                    var model = new UserModel
                    {
                        FirstName = registerObject.FirstName,
                        LastName = registerObject.LastName,
                        Email = registerObject.Email,
                        Username = registerObject.Username,
                        Password = hash,
                        AccountType = registerObject.AccountType,
                        Token = token,
                        City = registerObject.City,
                        Image=registerObject.Image
                    };
                    var id = userRepository.CreateUser(model);

                    return new StringResponse { Status = true, Message = "Registration successful.", Target=id};
                }
            }
            catch(Exception)
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };

            }
        }

        #endregion

        #region GetUser

        public ProfileResponse LoadProfile(LoadProfileObject loadProfile)//radi
        {
            try
            {
                UserModel profile;
                if (loadProfile.Id == null)
                {
                    profile = userRepository.GetByToken(loadProfile.Token);

                }
                else
                {
                    profile = userRepository.GetById(loadProfile.Id);
                }

                if (profile != null)
                {

                    profile.Organises = eventRepository.GetByUser(profile.Token);

                    profile.InterestedIn = eventRepository.GetInterestedIn(profile.Token);

                    profile.IsFriendsWith = userRepository.GetFriendships(profile.Id);

                    profile.Favorites = categoryRepository.GetLikes(profile.Token);

                    profile.Invited = eventRepository.GetInvitedTo(profile.Id);
                      

                    return new ProfileResponse { Status = true, Message = "Login successfull.", Target = profile };
                }
                else
                {
                    return new ProfileResponse { Status = false, Message = "No profile with entered username." };
                }
            }
            catch (Exception )
            {
                return new ProfileResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public ProfileResponse GetUserForUpdate(LoadProfileObject loadProfile)//radi
        {
            try
            {
                var profile = userRepository.GetById(loadProfile.Id);

                if (profile != null)
                {
                    profile.Favorites = categoryRepository.GetLikes(profile.Token);

                    return new ProfileResponse { Status = true, Message = "Login successfull.", Target = profile };
                }
                else
                {
                    return new ProfileResponse { Status = false, Message = "No profile with entered username." };
                }
            }
            catch (Exception)
            {
                return new ProfileResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        #endregion

        #region Basic
        public StringResponse UpdateProfile(UpdateProfileObject updateProfileObject)//radi
        {
            try
            {
                var profile = userRepository.GetByToken(updateProfileObject.Token);

                if (profile != null)
                {
                    profile.FirstName = updateProfileObject.FirstName;
                    profile.LastName = updateProfileObject.LastName;
                    profile.City = updateProfileObject.City;
                    profile.Image = updateProfileObject.ImagePath;
                    UserModel user = userRepository.UpdateUser(profile);//update osnovnih podataka
                    List<CategoryModel> UserLikes = new List<CategoryModel>();
                    UserLikes = categoryRepository.GetLikes(user.Token);

                    if (updateProfileObject.categories.Count > 0)//ako postoje cekirane kategorije
                    {
                        //raskini vezu sa kategorijama koje vise nisu cekirane
                        UserLikes.ForEach(x =>
                        {
                            if (!updateProfileObject.categories.Contains(x.Name))
                            {
                                categoryRepository.DeleteLikes(user.Token, x.Name);
                            }
                        });

                        //kreiraj vezu sa kategorijama koje su nove
                        List<string> lista = new List<string>();
                        UserLikes.ForEach(x => lista.Add(x.Name));
                        updateProfileObject.categories.ForEach(x =>
                        {
                            if (!lista.Contains(x))
                            {
                                categoryRepository.CreateLikes(user.Token, x);
                            }
                        });
                    }
                    else if (UserLikes.Count > 0)//ako nije nijedna kategorija cekirana, a u bazi postoje kategorije koje korisnik voli, raskini sve veze
                    {
                        UserLikes.ForEach(x => categoryRepository.DeleteLikes(user.Token, x.Name));
                    }                    
                    
                    return new StringResponse { Status = true, Message = "Update successfull.", Target = profile.Token };
                }
                else
                {
                    return new StringResponse { Status = false, Message = "No profile with provided token." };
                }

            }
            catch (Exception)
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public StringResponse DeleteProfile(DeleteProfileObject deleteProfileObject)//radi
        {
            try
            {
                var profile = userRepository.DeleteUser(deleteProfileObject.Id);

                if (profile == null)
                {
                   
                    return new StringResponse { Status = true, Message = "Delete successfull." };
                }
                else
                {
                    return new StringResponse { Status = false, Message = "No profile with provided token." };
                }

            }
            catch (Exception )
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        #endregion

        #region Interested
        public StringResponse CreateInterested(InterestedObject req)
        {
            try
            {
                eventRepository.CreateInterested(req.Token,req.Event);

                return new StringResponse { Status = true, Message = "Success."};


            }
            catch (Exception )
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public StringResponse DeleteInterested(InterestedObject req)
        {
            try
            {
                eventRepository.DeleteInterested(req.Token, req.Event);

                return new StringResponse { Status = true, Message = "Success." };


            }
            catch (Exception)
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public BoolResponse GetInterested(InterestedObject req)
        {
            try
            {
                var res = eventRepository.GetInterested(req.Token, req.Event);

                return new BoolResponse { Status = true, Message = "Success." , Target = res};


            }
            catch (Exception )
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public UserListResponse GetInterestedFriends(InterestedObject req)
        {
            try
            {
                var res = eventRepository.GetInterestedFriends(req.Token, req.Event);

                return new UserListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new UserListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        #endregion

        #region GetListsForUser

        public EventListResponse GetCreatedEvents(string id)
        {
            try
            {
                var res = eventRepository.GetCreated(id);

                return new EventListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception )
            {
                return new EventListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public EventListResponse GetInvitedEvents(string id)
        {
            try
            {
                var res = eventRepository.GetInvitedTo(id);

                return new EventListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new EventListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public EventListResponse GetInterestedEvents(string id)
        {
            try
            {
                var res = eventRepository.GetInterested(id);

                return new EventListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception )
            {
                return new EventListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public UserListResponse GetUsers(SearchObject req)
        {
            try
            {
                var res = userRepository.GetForSearch(req.search);
                res.RemoveAll(x => x.Token == req.token);



                return new UserListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new UserListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public BoolResponse GetFriends(LoadFriendsObject req)
        {
            try
            {
                var res = userRepository.GetFriendship(req.Token, req.FriendId);

                return new BoolResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public UserListResponse GetFriendships(LoadFriendshipsObject req)
        {
            try
            {
                var res = userRepository.GetFriendships(req.Id);

                return new UserListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new UserListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        #endregion

        #region Report

        public BoolResponse ReportProfile(LoadFriendsObject req)
        {
            try
            {
                userRepository.CreateReport(req.Token, req.FriendId);

                return new BoolResponse { Status = true, Message = "Success." };


            }
            catch (Exception)
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public UserListResponse GetReportedUsers()
        {
            try
            {
                var result = userRepository.GetReportedUsers();

                return new UserListResponse { Status = true, Message = "Success.", Target=result };


            }
            catch (Exception)
            {
                return new UserListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public IntResponse GetNumberOfReports(string id)
        {
            try
            {
                var result = userRepository.GetNumberOfReports(id);

                return new IntResponse { Status = true, Message = "Success.", Target = result };


            }
            catch (Exception)
            {
                return new IntResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        #endregion

        #region FriendRequest

        public BoolResponse SendFriendRequest(string senderId, string receiverId)
        {
            try
            {
                userRepository.CreateSentFriendRequest(senderId,receiverId);

                return new BoolResponse { Status = true, Message = "Success." };


            }
            catch (Exception)
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public BoolResponse AcceptFriendRequest(string senderId, string receiverId)
        {
            try
            {
                userRepository.RemoveFriendshipRequest(senderId,receiverId);
                userRepository.CreateFriendship(senderId, receiverId);

                return new BoolResponse { Status = true, Message = "Success." };


            }
            catch (Exception )
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public BoolResponse GetFriendRequest(string senderId, string receiverId)
        {
            try
            {
                var res = userRepository.GetFriendRequest(senderId, receiverId);

                return new BoolResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public BoolResponse RejectRequest(string senderId,string receiverId)
        {
            try
            {
                userRepository.RemoveFriendshipRequest(senderId, receiverId);

                return new BoolResponse { Status = true, Message = "Success." };


            }
            catch (Exception)
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public StringResponse RemoveFriend(LoadFriendsObject req)
        {
            try
            {
                userRepository.DeleteFriendship(req.Token, req.FriendId);
                var result = userRepository.GetByToken(req.Token);

                return new StringResponse { Status = true, Message = "Success.", Target = result.Id };


            }
            catch (Exception)
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }


        #endregion

        #region Invite
        public BoolResponse ExistsInvited(string userId, string eventId)
        {
            try
            {
                var res = eventRepository.ExistsInvitedTo(userId, eventId);

                return new BoolResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public void AddInvited(string userId, string eventId)
        {
            try
            {
                eventRepository.AddInvitedTo(userId, eventId);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Going

        public StringResponse CreateGoing(InterestedObject req)
        {
            try
            {
                eventRepository.CreateGoing(req.Token, req.Event);

                return new StringResponse { Status = true, Message = "Success." };


            }
            catch (Exception)
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public StringResponse DeleteGoing(InterestedObject req)
        {
            try
            {
                eventRepository.DeleteGoing(req.Token, req.Event);

                return new StringResponse { Status = true, Message = "Success." };


            }
            catch (Exception)
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }


        public BoolResponse GetGoing(InterestedObject req)
        {
            try
            {
                var res = eventRepository.GetGoing(req.Token, req.Event);

                return new BoolResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new BoolResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public UserListResponse GetGoingFriends(InterestedObject req)
        {
            try
            {
                var res = eventRepository.GetGoingFriends(req.Token, req.Event);

                return new UserListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new UserListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        #endregion

        public UserListResponse GetUsers()
        {
            try
            {
                var res = userRepository.GetUsers();

                return new UserListResponse { Status = true, Message = "Success.", Target = res };


            }
            catch (Exception)
            {
                return new UserListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

    }
}
