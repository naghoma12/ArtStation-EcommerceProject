using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Helper;
using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtStation.Core;
using Microsoft.EntityFrameworkCore;

namespace ArtStation.Repository.Repository
{
    public class NotificationRepository : GenericRepository<NotificationRequest>, INotificationRepository
    {
        private readonly ArtStationDbContext _context;
        private readonly UserManager<AppUser> _user;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationRepository(ArtStationDbContext context, UserManager<AppUser> user, IUnitOfWork unitOfWork) : base(context)
        {
            _context = context;
            _user = user;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> IsUnReadNotification(int userId)
        {
            var unreadCount = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead
                && !n.IsDeleted && n.IsActive)
                .CountAsync();
            return unreadCount;
        }

        public async Task<string> SendNotification(string language, int userId, MessageRequest request)
        {

            var user = await _user.FindByIdAsync(userId.ToString());
            if (user == null || string.IsNullOrWhiteSpace(request.DeviceToken))
            {
                throw new ArgumentException("Invalid user or device token.");
            }
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = language == "ar" ? request.TitleAR : request.TitleEN,
                    Body = language == "ar" ? request.BodyAR : request.BodyEN
                },
                Token = request.DeviceToken
            };
            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            NotificationRequest notification = new NotificationRequest()
            {
                TitleAR = request.TitleAR,
                TitleEN = request.TitleEN,
                ContentAR = request.BodyAR,
                ContentEN = request.BodyEN,
                UserId = user.Id,
                IsRead = false
            };
            _context.Notifications.Add(notification);
            var count = await _unitOfWork.Complet();
            return result;
        }

        public async Task<IEnumerable<NotificationDTO>> GetNotifications(string language, int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted && n.IsActive)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    Content = language == "ar" ? n.ContentAR : n.ContentEN,
                    Title = language == "ar" ? n.TitleAR : n.TitleEN,
                    Date = n.CreatedDate
                })
                .ToListAsync();
            return notifications;
        }

        public async Task MarkAllNotificationAsRead(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted && n.IsActive)
                .ToListAsync();
            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    _context.Notifications.Update(notification);
                }
                await _unitOfWork.Complet();
            }
        }
        public async Task DeleteNotification(int userId, int notificationId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.UserId == userId && 
                n.Id == notificationId && !n.IsDeleted && n.IsActive);
            if (notification != null)
            {
                notification.IsDeleted = true;
                _context.Notifications.Update(notification);
                await _unitOfWork.Complet();
            }
        }

        public async Task DeleteAllNotification(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted && n.IsActive)
                .ToListAsync();
            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    notification.IsDeleted = true;
                    _context.Notifications.Update(notification);
                }
                await _unitOfWork.Complet();
            }
        }
    }
}
