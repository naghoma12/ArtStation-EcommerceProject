using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using ArtStation.Dtos.CartDtos;
using ArtStation.Repository;
using System.Globalization;

namespace ArtStation.Helper
{
    public static class Common
    {

        public static DateOnly? ConvertBirthday(string userBirthday)
        {
            DateOnly? birthday = null;

            if (!string.IsNullOrWhiteSpace(userBirthday))
            {
                if (DateOnly.TryParseExact(
                    userBirthday,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedDate))
                {
                    birthday = parsedDate;
                }
            }
            return birthday;
        }

      

    }
}
