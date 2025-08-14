using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Statistics
{
    public class StatisticsDTO
    {
        
        public int CompaniesCount { get; set; } 

        public int ApplicationUsersCount { get; set; }
        public decimal DailyMoney { get; set; }
        public DailySales dailySales { get; set; }
        public List<int> SalesData { get; set; } = new List<int>();

    }


}
