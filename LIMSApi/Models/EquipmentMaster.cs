using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.Metadata;

namespace LIMSApi.Models;

public partial class EquipmentMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }
    [StringLength(20)]
    public required string EquipmentNo { get; set; }
    public long DepartmentID { get; set; }
    public long EquipmentTypeID { get; set; }
    public long OEMID { get; set; }
    public string? ModelNo { get; set; }
    public string PurchaseDate { get; set; }
    public bool CalibrationRequired { get; set; } = false;
    public DateTime? NextCalibrationDueDate { get; set; }
    public bool MaintenanceRequired { get; set; } = false;
    public string ? MaintenanceInterval { get; set; }
    public DateTime? NextMaintenanceDueDate { get; set; }
    public string? InternalExternal { get; set; }
    public bool IntermediateCheckRequired { get; set; } = false;
    public string? IntermediateCheckInterval { get; set; }

    // Lab Room linkage (Phase 8)
    public long? LabRoomID { get; set; }

    [ForeignKey(nameof(LabRoomID))]
    public virtual LabRoom? LabRoom { get; set; }

    // Phase 7.1: Calibration & Maintenance enhancements
    public DateTime? LastCalibrationDate { get; set; }
    public int? CalibrationFrequencyDays { get; set; }
    [MaxLength(500)]
    public string? MaintenanceSchedule { get; set; }

    public ICollection<EquipmentCalibration> Calibrations { get; set; } = new List<EquipmentCalibration>();
    public ICollection<EquipmentMaintenance> Maintenances { get; set; } = new List<EquipmentMaintenance>();
    public ICollection<EquipmentSOP> SOPs { get; set; } = new List<EquipmentSOP>();

    public ICollection<EquipmentReferenceMaterial> ReferenceMaterials { get; set; } = new List<EquipmentReferenceMaterial>();

    [ForeignKey("EquipmentTypeID")]
    public virtual EquipmentTypeMaster? EquipmentType { get; set; }

    public virtual ICollection<EquipmentAnalysisTechnique> AnalysisTechniques { get; set; } = new List<EquipmentAnalysisTechnique>();
}
