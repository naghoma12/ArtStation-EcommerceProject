using ArtStation.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArtStation.Extensions
{
    
public static class UserManagerExtensions
    {
        public static async Task<TUser> FindByPhoneNumberAsync<TUser>(
            this UserManager<TUser> userManager,
            string phoneNumber)
            where TUser : class
        {
            var users = userManager.Users;

            // Try to cast to IQueryable<TUser> for async support
            if (users is IQueryable<TUser> queryableUsers)
            {
                return await queryableUsers
                    .Where(u => EF.Property<string>(u, "PhoneNumber") == phoneNumber)
                    .FirstOrDefaultAsync();
            }

            // Fallback (not async optimized)
            foreach (var user in users)
            {
                var property = typeof(TUser).GetProperty("PhoneNumber");
                var value = property?.GetValue(user) as string;
                if (value == phoneNumber)
                    return user;
            }

            return null;
        }
    

}
}
