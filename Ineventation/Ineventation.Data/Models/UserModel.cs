using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ineventation.Data.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        [Display(Name = "First name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Display(Name = "Username")]
        [MinLength(6)]
        [MaxLength(20)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum length is 6 characters.")]
        [MaxLength(15, ErrorMessage = "Maximum length is 15 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "City")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Display(Name = "Account type")]
        public string AccountType { get; set; }

        public string Token { get; set; }

        public string Image { get; set; }

        public List<UserModel> IsFriendsWith { get; set; }
        public List<EventModel> Organises { get; set; }
        public List<EventModel> InterestedIn { get; set; }
        public List<CategoryModel> Favorites { get; set; }
        public List<EventModel> Invited { get; set; }

        public UserModel()
        {
            Organises = new List<EventModel>();
            IsFriendsWith = new List<UserModel>();
            InterestedIn = new List<EventModel>();
            Favorites = new List<CategoryModel>();
            Invited = new List<EventModel>();
            Image = "";
        }


    }
}
