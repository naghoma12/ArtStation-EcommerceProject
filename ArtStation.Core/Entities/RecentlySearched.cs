using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class RecentlySearched : BaseEntity
    {
        public string Name { get; set; }
        public AppUser User { get; set; }
        public int UserId { get; set; }

    }
}
