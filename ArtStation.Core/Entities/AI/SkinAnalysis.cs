using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.AI
{
    public class SkinAnalysis: BaseEntity
    {
        public bool IsFaceDetected { get; set; }
        public float SkinScore { get; set; }
        [MaxLength(3)]
        public string SkinAge { get; set; }
        public string FinalNote { get; set; }
        public string? Image { get; set; }

        public int UserId { get; set; }
        public IEnumerable<SkinMetric> Metrics { get; set; } = new List<SkinMetric>();
    }
}
