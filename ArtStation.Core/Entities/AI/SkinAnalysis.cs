using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.AI
{
    public class SkinAnalysis: BaseEntity
    {
        public bool IsFaceDetected { get; set; }
        public float SkinScore { get; set; }
        public int SkinAge { get; set; }
        public string FinalNote { get; set; }
        public string? Image { get; set; }


    }
}
