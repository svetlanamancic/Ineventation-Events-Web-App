using System;
using System.ComponentModel.DataAnnotations;


namespace Ineventation.Data.Models
{
    public class EventModel
    {
        public string Id { get; set; }

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

        [Display(Name = "Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date is required.")]
        public string Date { get; set; }

        [Display(Name = "Time")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Time is required.")]
        public string Time { get; set; }

        [Display(Name = "DateAndTime")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required.")]
        public DateTime DateAndTime { get; set; }

        [Display(Name = "Event type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required.")]
        public CategoryModel Type { get; set; }

        public string Visibility { get; set; }

        public UserModel Creator { get; set; }

        public string Image { get; set;}


        public EventModel()
        {
            Creator = new UserModel();
            Type = new CategoryModel();
            Image = "";

        }
    }
}