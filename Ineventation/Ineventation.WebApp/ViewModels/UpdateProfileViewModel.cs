using Ineventation.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ineventation.WebApp.ViewModels
{
    public class UpdateProfileViewModel
    {
        public string AccountType { get; set; }

        public string MyId { get; set; }

        public string Id { get; set; }

        public List<CategoryModel> Categories { get; set; }


        [Display(Name = "First name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Display(Name = "City")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required.")]
        public string City { get; set; }

        public List<CategoryModel> Favorites { get; set; }

        public List<string> checkedCategories { get; set; }

        public System.Web.HttpPostedFileBase Image { get; set; }

        public UpdateProfileViewModel()
        {
            Categories = new List<CategoryModel>();
            checkedCategories = new List<string>();
        }

    }
}