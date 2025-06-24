using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Identity
{
    public enum Gender
    {
        [EnumMember(Value = "male")]
        Male = 1,

        [EnumMember(Value = "female")]
        Female = 2
    }
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }
      
        public string? Country { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDay { get; set; }
        public string? Nationality { get; set; }
        public string? Image { get; set; }
        public string FCMToken { get; set; } = string.Empty;

        public IList<Address> Addresses { get; set; }

        public ICollection<Favourite> Favourites = new List<Favourite>();
        public ICollection<Notification> Notifications = new List<Notification>();

    }
 
}
