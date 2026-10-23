namespace LIMSApi.Dtos
{
    public class EquipmentDetailsDto
    {
        public string EquipmentName { get; set; }
        public string EquipmentNo { get; set; }
        public string DepartmmentName { get; set; }
        public string EquipmentType { get; set; }
        public string OEMName { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? NextCalibrationDueDate { get; set; }
        public string IntermediateCheckInterval { get; set; }
        public string ModelNumber{ get; set; }
        public int? CalibrationFrequencyDays { get; set; }
    }
}
