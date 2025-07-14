using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface INotificationRepository : IGenericRepository<NotificationRequest>
    {
        Task<string> SendNotification(string language, int userId, MessageRequest request);
        Task<int> IsUnReadNotification(int userId);
        Task<IEnumerable<NotificationDTO>> GetNotifications(string language, int userId);
        Task MarkAllNotificationAsRead(int userId);
        Task DeleteNotification(int userId, int notificationId);
        Task DeleteAllNotification (int userId);
    }
}
