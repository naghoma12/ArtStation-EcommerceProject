using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper.AiDtos
{
    public class FaceScan
    {
        public bool IsFaceDetected { get; set; }
        public float SkinScore { get; set; }
        public string SkinAge { get; set; }
        public string FinalNote { get; set; }
        public string? Image { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public int UserId { get; set; }
        public List<SkinMetricDto> Metrics { get; set; } = new();
    }
}
