using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.AI
{
    public class SkinMetric : BaseEntity
    {
        public int SkinAnalysisId { get; set; }
        public SkinAnalysis SkinAnalysis { get; set; }
        public string ProblemName { get; set; }       
        public int Score { get; set; }           
        public string Comment { get; set; }
    }
}
