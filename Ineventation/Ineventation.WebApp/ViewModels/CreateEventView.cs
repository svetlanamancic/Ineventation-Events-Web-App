using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ineventation.WebApp.ViewModels
{
    public class CreateEventView
    {
        public string MyId { get; set; }

        public string AccountType { get; set; }


        public string Id { get; set; }
        public List<string> Categories { get; set; }

        [Display(Name = "Caption")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "You have to write a caption.")]
        public string Caption { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "City")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City is requred.")]
        public string City { get; set; }

        [Display(Name = "Location")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        [Display(Name = "Date and time")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date is required.")]
        public DateTime DateAndTime { get; set; }

        [Display(Name = "Event type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required.")]
        public string Type { get; set; }

        public System.Web.HttpPostedFileBase Image { get; set; }

        public string lat { get; set; }
        public string lng { get; set; }




        public CreateEventView()
        {
            Categories = new List<string>();
        }
    }
}