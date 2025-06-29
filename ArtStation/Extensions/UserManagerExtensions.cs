using ArtStation.Core;
using ArtStation.Core.Entities.Identity;
using ArtStation.Dtos.UserDtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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


        public async static Task<int> AddAddressUser(this UserManager<AppUser> userManager, 
            IUnitOfWork unitOfWork
            , AddressDto address, ClaimsPrincipal User)
        {
            var user = await userManager.GetUserAsync(User);

            Address address1 = new Address()
            {

                FullName = address.FullName,
                PhoneNumber = address.PhoneNumber,
               ShippingId=address.ShippingId,
                Lat = address.Lat,
                Long = address.Long,
                AddressDetails = address.AddressDetails,
                AppUserId = user.Id
            };
            try
            {
                unitOfWork.Repository<Address>().Add(address1);
                return await unitOfWork.Complet();
            }
            catch (Exception ex)
            {
                return 0;

            }

        }
        public async static Task<int> EditAddressUser(this UserManager<AppUser> userManager, 
            IUnitOfWork unitOfWork, AddressDto addressDto, ClaimsPrincipal User, int id)
        {
            var user = await userManager.GetUserAsync(User);
         
            var address = await unitOfWork.Repository<Address>().GetByIdAsync(id);

            address = new Address()
            {

                FullName = addressDto.FullName,
                PhoneNumber = addressDto.PhoneNumber,
                ShippingId=addressDto.ShippingId,
                Lat = addressDto.Lat,
                Long = addressDto.Long,
                AddressDetails =addressDto.AddressDetails,
                AppUserId = user.Id
            };
            try
            {
                unitOfWork.Repository<Address>().Update(address);

                return await unitOfWork.Complet();
            }
            catch (Exception ex)
            {
                return 0;

            }

        }

    }
}
