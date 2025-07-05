using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArtStation_Dashboard.ViewModels.User
{
    public class TraderViewModel
    {
        public int? Id { get; set; }

        public string UserName { get; set; }
        public string DispalyName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Nationality { get; set; }
        public string? Photo { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public string Password { get; set; }

        public IEnumerable<Shipping>   Cities { get; set; } = new List<Shipping>();
    }
}
