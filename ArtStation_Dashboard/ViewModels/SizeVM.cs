using ArtStation_Dashboard.Resource;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels
{
    public class SizeVM
    {
        public int Id { get; set; }
        public string? SizeAR { get; set; }
      
        public string? SizeEN { get; set; }
        public decimal? Price { get; set; }
    }
}
