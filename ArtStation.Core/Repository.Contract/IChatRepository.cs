using ArtStation.Core.Helper.AiDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IChatRepository
    {
        Task<IEnumerable<ChatResponse>> ChatResponses(int userId);
    }
}
