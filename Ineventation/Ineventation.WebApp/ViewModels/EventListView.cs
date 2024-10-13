using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ineventation.Data.Models;
using PagedList;

namespace Ineventation.WebApp.ViewModels
{
    public class EventListView
    {
        public string AccountType { get; set; }

        public string MyId { get; set; }

        public string Id { get; set; }

        public string Case { get; set; }
        public IPagedList<EventModel> Events { get; set; }

        public EventListView()
        {

        }
    }
}