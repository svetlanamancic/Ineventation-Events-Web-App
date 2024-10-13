using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineventation.Data.Models
{
    public class NewsModel
    {
        public string Id { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public string IdObject { get; set; }
        public string Type { get; set; }

        public DateTime Time { get; set; }

        public NewsModel()
        {
            Time = DateTime.Now;
            Status = "unread";
            Id = Guid.NewGuid().ToString();
        }

    }
}
