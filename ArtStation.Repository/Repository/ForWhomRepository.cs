using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository.Repository
{
    public class ForWhomRepository :  IForWhomRepository
    {
        public List<ForWhomWithId> GetForWhoms(string language)
        {
            return Enum.GetValues(typeof(ForWhom))
                .Cast<ForWhom>()
                .Select(f => new ForWhomWithId
                {
                    Id = (int)f,
                    Name = language == "en" ? ((int)f % 2 != 0).ToString()
                : ((int)f % 2 == 0).ToString()
                })
                .ToList();
        }
    
    }
}
