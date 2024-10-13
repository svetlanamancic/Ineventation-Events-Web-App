using Ineventation.Data.Models;
using System;
using System.Collections.Generic;
using PagedList;

namespace Ineventation.WebApp.ViewModels
{
    public class DiscoverViewModel
    {
        public string AccountType { get; set; }

        public string MyId { get; set; }

        public string Tag { get; set; }//sluzi za indikaciju sta treba da se prikaze
        public IPagedList<EventModel> list { get; set; }

        public List<string> Categories { get; set; }

        public DateTime DateAndTime { get; set; }//for search

        public string City { get; set; }//for search

        public string SelectedCategory { get; set; }//for search


        public DiscoverViewModel()
        {
            Categories = new List<string>();
        }
    }

}