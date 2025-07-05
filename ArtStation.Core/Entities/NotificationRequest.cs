using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class NotificationRequest: BaseEntity
    {
            public string TitleAR { get; set; }
            public string TitleEN { get; set; }
            public string ContentAR { get; set; }
            public string ContentEN { get; set; }
            public AppUser User { get; set; }
            public int UserId { get; set; }
            public bool IsRead { get; set; }

    }
    
}
