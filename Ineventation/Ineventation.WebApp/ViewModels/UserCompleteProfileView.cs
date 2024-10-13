using Ineventation.Data.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ineventation.WebApp.ViewModels
{
    public class UserCompleteProfileView
    {
        public string UserAccountType { get; set; }

        public string MyId { get; set; }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string City { get; set; }

        public string Image { get; set; }


        public string AccountType { get; set; }

        public int NumberOfReports { get; set; }

        public IPagedList<UserModel> Friends { get; set; }

        public IPagedList<EventModel> Organises { get; set; }

        public IPagedList<EventModel> InterestedIn { get; set; }

        public IPagedList<CategoryModel> Likes { get; set; }


        public UserCompleteProfileView()
        {
           
        }
    }
}