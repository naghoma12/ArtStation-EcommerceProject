using ArtStation.Core.Helper.AiDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IScanRepository
    {
        Task<IEnumerable<FaceScan>> GetScanHistory(int userId);
    }
}
