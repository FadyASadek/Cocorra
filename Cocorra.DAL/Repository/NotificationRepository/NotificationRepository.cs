using Cocorra.DAL.Data;
using Cocorra.DAL.DTOS.ChatDto;
using Cocorra.DAL.Models;
using Cocorra.DAL.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.DAL.Repository.NotificationRepository
{
    public class NotificationRepository : GenericRepositoryAsync<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        
    }
}
