using Ineventation.Data.Models;
using System.Collections.Generic;

namespace Ineventation.WebApp.ViewModels
{
    public class EventDetailsViewModel
    {
        public string AccountType { get; set; }

        public string MyId { get; set; }

        public string Id { get; set; }

        public string Caption { get; set; }
        
        public string Description { get; set; }
        
        public string City { get; set; }

        public string Location { get; set; }

        public string DateAndTime { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string CreatedBy { get; set; }

        public string Type { get; set; }

        public bool Interested { get; set; }

        public bool Going { get; set; }

        public List<UserModel> list { get; set; }

        public bool Invited { get; set; }


        public bool MyEvent { get; set; }

        public string Image { get; set; }

        public string Visibility { get; set; }


        //public List<UserModel> Friends { get; set; }//friends that are interested

        //public EventDetailsViewModel()
        //{
        //    Friends = new List<UserModel>();
        //}
    }
}