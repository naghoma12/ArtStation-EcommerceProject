using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class Notification: BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public AppUser AppUser { get; set; }
        public bool IsRead { get; set; }
        public string Language { get; set; }

    }
}
