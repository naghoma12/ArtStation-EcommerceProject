using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArtStation.Repository.Data
{
    public static class AppSeeding
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> _usermanager, RoleManager<AppRole> _roleManager)
        {
            if (_usermanager.Users.Count() == 0)
            {
                var arwa = new AppUser()
                {
                    Email = "arwaalaa99@hotmail.com",
                    UserName = "Arwa1",
                    FullName="Arwa Alaa",
                    PhoneNumber = "01011037481"
                };

                var nagham = new AppUser()
                {
                    Email = "nagham15@gmail.com",
                    UserName = "nagham",
                    FullName = "Nagham Yasser",
                    PhoneNumber = "01011037481"
                };

                var role = new AppRole()
                {

                    Name = "Admin"
                };

                var result = await _usermanager.CreateAsync(arwa, "P@ssWord1");
                var result2 = await _usermanager.CreateAsync(nagham, "P@ssWord1");
                var Roleresult = await _roleManager.CreateAsync(role);
                var resultAdd = await _usermanager.AddToRoleAsync(arwa, "Admin");
                var resultAdd2 = await _usermanager.AddToRoleAsync(nagham, "Admin");




            }
        }

        public static async Task SeedShippingCost(ArtStationDbContext _Context)
        {

            if (_Context.Shippings.Count() == 0)
            {
                var costs = File.ReadAllText(".././ArtStation.Repository/Data/DataSeed/ShippingCost.json");
                var methods = JsonSerializer.Deserialize<List<Shipping>>(costs);
                if (methods.Count() > 0)
                {
                    foreach (var item in methods)
                    {
                        _Context.Set<Shipping>().Add(item);
                    }
                    await _Context.SaveChangesAsync();

                }
            }
        }

    }
}
