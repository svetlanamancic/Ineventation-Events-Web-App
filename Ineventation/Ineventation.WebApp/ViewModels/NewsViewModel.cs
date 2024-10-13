using Ineventation.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ineventation.WebApp.ViewModels
{
    public class NewsViewModel
    {
        public string MyId { get; set; }

        public string AccountType { get; set; }
        public List<NewsModel> list { get; set; }

        public string Filter { get; set; }

        public NewsViewModel()
        {
            list = new List<NewsModel>();
            Filter = "";
        }
    }
}