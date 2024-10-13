using System.Collections.Generic;

namespace Ineventation.Business.Objects.Requests
{
    public class UpdateProfileObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string Token { get; set; }

        public List<string> categories { get; set; }

        public string ImagePath { get; set; }
    }
}
