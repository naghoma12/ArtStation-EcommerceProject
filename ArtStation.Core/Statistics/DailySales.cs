using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Statistics
{
    public class DailySales
    {
        public decimal TodaySale { get; set; }
        public decimal Percentage { get; set; }
        public DateTime LastUpdated { get; set; } 
    }
}
