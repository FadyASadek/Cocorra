using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cocorra.BLL.Base;
using Cocorra.DAL.DTOS.ReportDto;
using Cocorra.DAL.DTOS.SupportDto;
using Cocorra.DAL.Enums;
using Cocorra.DAL.Models;
using Cocorra.DAL.Repository.SupportRepository;

namespace Cocorra.BLL.Services.SupportService
{
    public class SupportService : ResponseHandler, ISupportService
    {
        private readonly ISupportRepository _supportRepo;

        public SupportService(ISupportRepository supportRepo)
        {
            _supportRepo = supportRepo;
        }

        public async Task<Response<string>> SubmitTicketAsync(Guid? userId, SubmitSupportTicketDto dto)
        {
            var ticket = new SupportTicket
            {
                UserId = userId,
                Type = dto.Type,
                Message = dto.Message,
                ContactEmail = dto.ContactEmail,
                Status = "Open"
            };

            await _supportRepo.AddTicketAsync(ticket);

            return Success("Support ticket submitted successfully.");
        }

        public async Task<Response<string>> SubmitReportAsync(Guid reporterId, SubmitReportDto dto)
        {
            var report = new Report
            {
                ReporterId = reporterId,
                ReportedUserId = dto.ReportedUserId,
                ReportedRoomId = dto.ReportedRoomId,
                Category = dto.Category,
                Description = dto.Description,
                Status = "Open"
            };

            await _supportRepo.AddReportAsync(report);

            return Success("Report submitted successfully.");
        }

        public async Task<Response<List<ReportDetailsDto>>> GetFilteredReportsAsync(ReportCategory? category, string? status)
        {
            var reports = await _supportRepo.GetFilteredReportsAsync(category, status);

            var result = reports.Select(r => new ReportDetailsDto
            {
                Id = r.Id,
                ReporterId = r.ReporterId,
                ReporterName = r.Reporter != null
                    ? $"{r.Reporter.FirstName} {r.Reporter.LastName}"
                    : "Unknown",
                ReportedUserId = r.ReportedUserId,
                ReportedUserName = r.ReportedUser != null
                    ? $"{r.ReportedUser.FirstName} {r.ReportedUser.LastName}"
                    : null,
                ReportedRoomId = r.ReportedRoomId,
                Category = (int)r.Category,
                CategoryName = r.Category.ToString(),
                Description = r.Description,
                ScreenshotPath = r.ScreenshotPath,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Success(result);
        }

        public async Task<Response<string>> UpdateReportStatusAsync(Guid reportId, string newStatus)
        {
            var report = await _supportRepo.GetReportByIdAsync(reportId);
            if (report == null) return NotFound<string>("Report not found.");

            report.Status = newStatus;
            report.UpdatedAt = DateTime.UtcNow;

            await _supportRepo.UpdateReportAsync(report);

            return Success($"Report status updated to '{newStatus}'.");
        }
    }
}
