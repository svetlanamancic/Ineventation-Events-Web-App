using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using Ineventation.Data.Models;

namespace Ineventation.WebApp.ViewModels
{
    public class SearchModel
    {
        public string AccountType { get; set; }

        public string MyId { get; set; }

        public string Search { get; set; }

        public IPagedList<UserModel> list { get; set; }

        public SearchModel()
        {

        }
    }
}