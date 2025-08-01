using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
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
    public class ForWhomRepository : GenericRepository<ProductForWhom>, IForWhomRepository , IProductTypeRepository<ProductForWhom>
    {
        public ForWhomRepository(ArtStationDbContext context): base(context)
        {
            
        }
        public void DeleteRange(List<ProductForWhom> productForWhoms)
        {
            _context.ProductForWhoms.RemoveRange(productForWhoms);
        }

        public void UpdateRange(List<ProductForWhom> productForWhoms)
        {
            _context.ProductForWhoms.UpdateRange(productForWhoms);
        }
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

        public async Task<List<ProductForWhom>> GetProductTypes(int productId)
        {
           return await _context.ProductForWhoms
                .Where(x => x.IsActive && !x.IsDeleted && x.ProductId == productId)
                .ToListAsync();
        }
    }
}
