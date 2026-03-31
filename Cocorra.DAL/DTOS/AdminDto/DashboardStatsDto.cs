using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.DAL.DTOS.AdminDto
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int PendingUsers { get; set; }
        public int BannedUsers { get; set; }
        public int RejectedUsers { get; set; }
        public int ReRecordUsers { get; set; }
    }
}
