using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string Fname { get; set; }
        public string FamilyName { get; set; }
        public string Country { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDay { get; set; }
        public string Nationality { get; set; }
        public string? Image { get; set; }
        public string FCMToken { get; set; }
        public ICollection<Favourite> Favourites = new List<Favourite>();

    }
    public enum Gender
    {
        Male = 1,
        ذكر = 1,
        Female =2,
        أنثى = 2
    }
}
