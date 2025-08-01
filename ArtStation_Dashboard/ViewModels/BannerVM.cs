namespace ArtStation_Dashboard.ViewModels
{
    public class BannerVM
    {
        public int? Id { get; set; }
        public string Title { get; set; }

        public string? ImageUrl { get; set; }
        public IFormFile? Photo { get; set; }
        public int OrderBanner { get; set; }
    }
}
