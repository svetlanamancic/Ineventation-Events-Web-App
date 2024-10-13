using Ineventation.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ineventation.WebApp.ViewModels
{
    public class InviteView
    {
        public string MyId { get; set; }

        public string EventId { get; set; }

        public string AccountType { get; set; }

        public List<UserModel> Friends { get; set; }

    }
}