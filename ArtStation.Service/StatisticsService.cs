using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Services.Contract;
using ArtStation.Core.Statistics;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderRepository _orderRepository;

        public StatisticsService(UserManager<AppUser> userManager
            , IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
        }
        private decimal CalculatePercentageChange(int todaySalesCount)
        {
            var yesterdaySalesCount = _orderRepository.GetYesterdayOrdersCount();
            if (yesterdaySalesCount == 0) return 0; // All growth if no sales yesterday

            return ((todaySalesCount - yesterdaySalesCount) / (decimal)yesterdaySalesCount) * 100;
        }
        private decimal CalculateCompanySalesPercentage(int todaySalesCount, string phoneNumber)
        {
            var yesterdaySalesCount = _orderRepository.GetYesterdayOrdersCount(phoneNumber);
            if (yesterdaySalesCount == 0) return 0; // All growth if no sales yesterday

            return ((todaySalesCount - yesterdaySalesCount) / (decimal)yesterdaySalesCount) * 100;
        }
        public async Task<StatisticsDTO> GetAdminStatistics()
        {
            var companies = await _userManager.GetUsersInRoleAsync("Trader");
            var appUsers = await _userManager.GetUsersInRoleAsync("Customer");
            var dailyMoney = _orderRepository.GetDailyMoneyCount();
            var todatSale = _orderRepository.GetDailyOrdersCount();
            var statistics = new StatisticsDTO()
            {
                ApplicationUsersCount = appUsers.Count,
                CompaniesCount = companies.Count,
                DailyMoney = dailyMoney,
                dailySales = new DailySales()
                {
                    TodaySale = todatSale,
                    LastUpdated = DateTime.Now,
                    Percentage = CalculatePercentageChange(todatSale)
                },
                SalesData = _orderRepository.GetWeeklySales()
            };
            return statistics;
        }
        public StatisticsDTO GetCompanyStatistics(string phone)
        {
            var dailyMoney = _orderRepository.GetDailyCompanyMoneyCount(phone);
            var dailySales = _orderRepository.GetCompanyOrdersCount(phone);
            var statistics = new StatisticsDTO()
            {
                DailyMoney = dailyMoney,
                dailySales = new DailySales()
                {
                    TodaySale = dailySales,
                    LastUpdated = DateTime.Now,
                    Percentage = CalculateCompanySalesPercentage(dailySales, phone)
                },
                SalesData = _orderRepository.GetWeeklySales(phone)
            };
            return statistics;
        }

       
    }
}
