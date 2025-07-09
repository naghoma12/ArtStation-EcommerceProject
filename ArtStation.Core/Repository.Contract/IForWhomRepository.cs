using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IForWhomRepository
    {
         List<ForWhomWithId> GetForWhoms(string language);
    }
}
