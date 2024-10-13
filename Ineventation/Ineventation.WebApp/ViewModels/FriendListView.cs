using Ineventation.Data.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ineventation.WebApp.ViewModels
{
    public class FriendListView
    {
        public string AccountType { get; set; }

        public string MyId { get; set; }

        public string Id { get; set; }


        public IPagedList<UserModel> Friends { get; set; }

        public FriendListView()
        {

        }
    }
}