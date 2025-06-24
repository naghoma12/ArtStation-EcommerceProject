namespace ArtStation.Dtos.UserDtos
{
    public class UpdateUserProfileDto
    {
        public string Fname { get; set; }
        public string LName { get; set; }

        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
       
        public string? Birthday { get; set; }
        public string? Nationality { get; set; }
        public string? Gender { get; set; }
    }
}
