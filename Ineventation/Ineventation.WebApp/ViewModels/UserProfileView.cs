using Ineventation.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ineventation.WebApp.ViewModels
{
    public class UserProfileView
    {
        public string UserAccountType { get; set; }
        public string Id { get; set; }

        public string MyId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public string Username { get; set; }

        public string City { get; set; }

        public string AccountType { get; set; }

        public string Case { get; set; }

        public string Image { get; set; }

        public bool MyProfile { get; set; }

        public bool Friends { get; set; }

        public bool SentRequest { get; set; }

        public bool ReceivedRequest { get; set; }

        public string Message { get; set; }

        public List<EventModel> Organises { get; set; }
        public List<EventModel> InterestedIn { get; set; }

        public List<EventModel> Invited { get; set; }


        public UserProfileView()
        {
            Organises = new List<EventModel>();
            InterestedIn = new List<EventModel>();
            Invited = new List<EventModel>();

        }

    }
}