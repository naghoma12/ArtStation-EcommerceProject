using ArtStation.Core.Entities.AI;
using ArtStation.Core.Helper.AiDtos;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository.Repository
{
    public class ScanReposiory : GenericRepository<SkinMetric> , IScanRepository
    {
        private readonly ArtStationDbContext _context;

        public ScanReposiory(ArtStationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FaceScan>> GetScanHistory(int userId)
        {
            return await _context.SkinAnalyses
                .Where(x => x.IsActive && !x.IsDeleted && x.UserId  == userId)
                .Select(x => new FaceScan()
                {
                    IsFaceDetected = x.IsFaceDetected,
                    FinalNote = x.FinalNote,
                    Image = x.Image,
                    SkinAge = x.SkinAge,
                    SkinScore = x.SkinScore,
                    UserId = userId,
                    Metrics = x.Metrics.Select(s => new SkinMetricDto
                    {
                        Comment = s.Comment,
                        ProblemName = s.ProblemName,
                        Score = s.Score,
                    }).ToList()

                })
                .ToListAsync();
        }
    }
}
