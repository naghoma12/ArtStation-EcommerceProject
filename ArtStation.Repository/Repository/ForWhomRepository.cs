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
                    Name = GetLocalizedForWhomName(f, language)
                })
                .ToList();
        }

        public string GetLocalizedForWhomName(ForWhom f, string language)
        {
            return language == "en" ? f.ToString() : f switch
            {
                ForWhom.Men => "للرجال",
                ForWhom.Women => "للنساء",
                ForWhom.Kids => "للأطفال",
                _ => "غير معروف"
            };
        }

    }
}
