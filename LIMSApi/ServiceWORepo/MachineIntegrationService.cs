using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LIMSApi.ServiceWORepo
{
    public class MachineIntegrationService : IMachineIntegrationService
    {
        private readonly LIMSContext _db;

        public MachineIntegrationService(LIMSContext db)
        {
            _db = db;
        }

        public async Task<MachineDataResultDto> ProcessMachineData(MachineDataDto dto)
        {
            var result = new MachineDataResultDto
            {
                TotalReadings = dto.Readings.Count
            };

            // Find the target TestResultHeader
            TestResultHeader? header = null;

            if (dto.TestResultHeaderId.HasValue)
            {
                header = await _db.TestResultHeaders
                    .Include(h => h.Parameters)
                    .FirstOrDefaultAsync(h => h.ID == dto.TestResultHeaderId.Value);
            }
            else if (dto.EquipmentId.HasValue)
            {
                header = await _db.TestResultHeaders
                    .Include(h => h.Parameters)
                    .Where(h => h.IsActive && h.Status == "Started" &&
                        (h.EquipmentID == dto.EquipmentId || (h.EquipmentIdsJson != null && h.EquipmentIdsJson.Contains(dto.EquipmentId.ToString()!))))
                    .FirstOrDefaultAsync();
            }

            if (header == null)
            {
                result.UnmatchedCount = dto.Readings.Count;
                result.UnmatchedParameters = dto.Readings.Select(r => r.ParameterName).ToList();
                return result;
            }

            // Match readings to parameters
            foreach (var reading in dto.Readings)
            {
                var param = header.Parameters.FirstOrDefault(p =>
                    p.ParameterName.Equals(reading.ParameterName, StringComparison.OrdinalIgnoreCase));

                if (param != null)
                {
                    param.Value = reading.Value;
                    result.MatchedCount++;
                    result.MatchedParameters.Add(reading.ParameterName);
                }
                else
                {
                    result.UnmatchedCount++;
                    result.UnmatchedParameters.Add(reading.ParameterName);
                }
            }

            // Save machine data log
            var log = new MachineDataLog
            {
                EquipmentId = dto.EquipmentId,
                TestResultHeaderId = header.ID,
                RawPayloadJson = dto.RawDataJson ?? JsonSerializer.Serialize(dto.Readings),
                MatchedCount = result.MatchedCount,
                UnmatchedCount = result.UnmatchedCount,
                ReceivedAt = DateTime.UtcNow,
                MachineSerialNo = dto.MachineId,
                Source = "API"
            };

            _db.Set<MachineDataLog>().Add(log);
            await _db.SaveChangesAsync();

            result.LogId = log.ID;
            return result;
        }

        public async Task<List<object>> GetPendingTestsForEquipment(long equipmentId)
        {
            var headers = await _db.TestResultHeaders
                .Where(h => h.IsActive && h.Status == "Started" &&
                    (h.EquipmentID == equipmentId || (h.EquipmentIdsJson != null && h.EquipmentIdsJson.Contains(equipmentId.ToString()))))
                .Select(h => new
                {
                    h.ID,
                    h.SampleID,
                    h.LaboratoryTestID,
                    h.Status,
                    h.StartedAt,
                    ParameterCount = h.Parameters.Count
                })
                .ToListAsync();

            return headers.Cast<object>().ToList();
        }

        public async Task<List<object>> GetMachineDataLogs(long headerId)
        {
            var logs = await _db.Set<MachineDataLog>()
                .Where(l => l.TestResultHeaderId == headerId && l.IsActive)
                .OrderByDescending(l => l.ReceivedAt)
                .Select(l => new
                {
                    l.ID,
                    l.EquipmentId,
                    l.MatchedCount,
                    l.UnmatchedCount,
                    l.ReceivedAt,
                    l.MachineSerialNo,
                    l.Source
                })
                .ToListAsync();

            return logs.Cast<object>().ToList();
        }
    }
}
