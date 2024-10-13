using System.ComponentModel.DataAnnotations;

namespace Ineventation.WebApp.ViewModels
{
    public class LoginModel
    {
        [Display(Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        public int userNo { get; set; }

        public int eventNo { get;set;}

        public int categoryNo { get; set; }

    }
}