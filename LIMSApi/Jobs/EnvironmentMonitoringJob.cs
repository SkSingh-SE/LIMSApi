using LIMSApi.Data;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    /// <summary>
    /// Daily job: auto-generates environment monitoring daily records for all active lab rooms.
    /// Copies previous day's values as defaults. User can edit later.
    /// Runs daily at 8 AM.
    /// </summary>
    public class EnvironmentMonitoringJob
    {
        private readonly LIMSContext _db;
        private readonly ILogger<EnvironmentMonitoringJob> _logger;

        public EnvironmentMonitoringJob(LIMSContext db, ILogger<EnvironmentMonitoringJob> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Execute()
        {
            var today = DateTime.UtcNow.Date;
            var currentMonth = today.Month;
            var currentYear = today.Year;

            // Get all active lab rooms with monitoring enabled
            var labRooms = await _db.LabRooms
                .Where(r => r.IsActive && r.MonitoringEnabled)
                .ToListAsync();

            foreach (var room in labRooms)
            {
                try
                {
                    // Find or create monthly monitoring header for this room
                    var monthlyHeader = await _db.NablEnvironmentMonitorings
                        .Include(m => m.DailyRecords)
                        .FirstOrDefaultAsync(m => m.IsActive && !m.IsObsolete
                            && m.LabRoomId == room.ID
                            && m.MonitoringMonth == currentMonth
                            && m.MonitoringYear == currentYear);

                    if (monthlyHeader == null)
                    {
                        // Create new monthly header
                        monthlyHeader = new NablEnvironmentMonitoring
                        {
                            LabRoomId = room.ID,
                            RoomName = room.Name,
                            DepartmentId = room.DepartmentID,
                            MonitoringMonth = currentMonth,
                            MonitoringYear = currentYear,
                            MonitoringDate = today,
                            AcceptableTemperatureMin = room.DefaultTempMin,
                            AcceptableTemperatureMax = room.DefaultTempMax,
                            AcceptableHumidityMin = room.DefaultHumidityMin,
                            AcceptableHumidityMax = room.DefaultHumidityMax,
                            FormCode = "F-12",
                            Status = "Draft",
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = 0,
                            CompanyCode = room.CompanyCode ?? "LIMS",
                            IsActive = true
                        };
                        _db.NablEnvironmentMonitorings.Add(monthlyHeader);
                        await _db.SaveChangesAsync();
                    }

                    // Check if today's record already exists
                    var existsToday = monthlyHeader.DailyRecords
                        .Any(d => d.RecordDate.Date == today && d.IsActive);

                    if (!existsToday)
                    {
                        // Get yesterday's record to copy as default
                        var yesterday = today.AddDays(-1);
                        var previousRecord = monthlyHeader.DailyRecords
                            .Where(d => d.RecordDate.Date == yesterday && d.IsActive)
                            .FirstOrDefault();

                        // Create today's record with previous day's values
                        var dailyRecord = new EnvironmentDailyRecord
                        {
                            EnvironmentMonitoringID = monthlyHeader.ID,
                            RecordDate = today,
                            Temperature = previousRecord?.Temperature ?? room.DefaultTempMin,
                            Humidity = previousRecord?.Humidity ?? room.DefaultHumidityMin,
                            ObservedTime = "08:00",
                            ObservedBy = "System (Auto-generated)",
                            IsWithinLimits = true,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = 0,
                            CompanyCode = room.CompanyCode ?? "LIMS",
                            IsActive = true
                        };

                        // Check limits
                        if (room.DefaultTempMin.HasValue && room.DefaultTempMax.HasValue && dailyRecord.Temperature.HasValue)
                        {
                            dailyRecord.IsWithinLimits = dailyRecord.Temperature >= room.DefaultTempMin
                                && dailyRecord.Temperature <= room.DefaultTempMax;
                        }

                        _db.EnvironmentDailyRecords.Add(dailyRecord);
                        await _db.SaveChangesAsync();

                        _logger.LogInformation("Auto-generated environment record for Room '{RoomName}' on {Date}",
                            room.Name, today.ToString("dd-MMM-yyyy"));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to auto-generate environment record for Room '{RoomName}'", room.Name);
                }
            }
        }
    }
}
