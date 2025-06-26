using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IFavouriteRepository
    {
        Task<Favourite> GetFavoriteAsync(int ProductId, int UserId);
        Task<IEnumerable<SimpleProduct>> FavouriteProducts(string language , int UserId);
    }
}
