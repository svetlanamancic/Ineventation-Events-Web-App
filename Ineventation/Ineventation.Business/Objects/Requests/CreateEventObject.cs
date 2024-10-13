using System;

namespace Ineventation.Business.Objects.Requests
{
    public class CreateEventObject
    {
        public string Caption { get; set; }

        public string Description { get; set; }

        public string City { get; set; }

        public string Location { get; set; }

        public DateTime DateAndTime { get; set; }

        public string Type { get; set; }

        public string Creator { get; set; }

        public string Image { get; set; }



    }
}
