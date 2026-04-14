using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cocorra.DAL.Enums;
using Cocorra.DAL.Models;

namespace Cocorra.DAL.Repository.SupportRepository
{
    public interface ISupportRepository
    {
        Task AddTicketAsync(SupportTicket ticket);
        Task AddReportAsync(Report report);
        Task<List<Report>> GetFilteredReportsAsync(ReportCategory? category, string? status);
        Task<Report?> GetReportByIdAsync(Guid reportId);
        Task UpdateReportAsync(Report report);
    }
}
