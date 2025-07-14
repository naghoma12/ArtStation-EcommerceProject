using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper.AiDtos
{
    public class ChatResponse
    {
        public string? Message { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public string? Image { get; set; }
        public string? Note { get; set; }
        [Required]
        public string Reply { get; set; }
        public int UserId { get; set; }
        public List<AIProducts> RecommendedProducts { get; set; } = new List<AIProducts>();

    }
}
