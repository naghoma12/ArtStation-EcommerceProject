using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;

namespace ArtStation_Dashboard.ViewModels.User
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDay { get; set; }
        public string? Nationality { get; set; }
        public string? Image { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
     
    }
}
