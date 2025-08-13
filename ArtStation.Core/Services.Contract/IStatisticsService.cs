using ArtStation.Core.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Services.Contract
{
    public interface IStatisticsService
    {
        Task<StatisticsDTO> GetAdminStatistics();
        StatisticsDTO GetCompanyStatistics(string phone);
    }
}
