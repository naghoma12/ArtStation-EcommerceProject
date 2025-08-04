using ArtStation.Core.Resources;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Identity
{
    public enum Gender
    {
        [Display(Name = "Male", ResourceType = typeof(Messages))]
        Male ,

        [Display(Name = "Female", ResourceType = typeof(Messages))]
        Female 
    }
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string? InActiveMessage { get; set; }
        public DateTime? DeactivatedAt { get; set; }

        public string? Country { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDay { get; set; }
        public string? Nationality { get; set; }
        public string? Image { get; set; }
        public string FCMToken { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public IList<Address>? Address { get; set; } =new List<Address>();

        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
       // public ICollection<Notification> Notifications { get; set; } = new List<Notification>();


    }
 
}
