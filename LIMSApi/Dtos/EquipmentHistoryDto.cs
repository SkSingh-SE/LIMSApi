namespace LIMSApi.Dtos
{
    public class EquipmentHistoryDto
    {
        public string EquipmentName { get; set; }
        public string EquipmentNo { get; set; }
        public string DepartmmentName { get; set; }
        public string EquipmentType { get; set; }
        public string OEMName { get; set; }
        public string PurchaseDate { get; set; }
        public bool CalibrationReq { get; set; }
        public bool MaintenanceReq { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
         public DateTime? NextMaintenanceDueDate { get; set; }
        public DateTime? NextCalibrationDueDate { get; set; }
        public string MaintenanceInterval { get; set; }
        public int? CalibrationFrequencyDays { get; set; }
        public List<MaintenanceListDto> Maintenances { get; set; }
        public List<CalibrationListDto> Calibrations { get; set; }
    }

    public class CalibrationListDto
    {
        public DateTime CalibrationDate { get; set; }
        public DateTime? ReviewerDate { get; set; }
        public DateTime CalibrationCreateDate { get; set; }
        public DateTime CalibrationDueDate { get; set; }
        public bool IsReviewed { get; set; }
        public string ReviewReason { get; set; }
        public string Agency { get; set; }
        public string CalibrationCreateBy { get; set; }
        public string ReviewerBy { get; set; }
        public string Description { get; set; }

    }
    public class MaintenanceListDto
    {
        public DateTime MaintenanceDate { get; set; }
        public DateTime MaintanceCreateDate { get; set; }
        public string MaintanceCreateBy { get; set; }
        public string Description { get; set; }

    }
}
