using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using Ineventation.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Ineventation.WebApp.ViewModels
{
    public class AdminOptionsView
    {
        public string AccountType { get; set; }
        public string MyId { get; set; }
        public IPagedList<CategoryModel> Categories { get; set; }

        public IPagedList<UserModel> ReportedUsers { get; set; }

        [Display(Name = "Category")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is requred.")]
        public string NewCategory { get; set; }

    }
}