using System;
using System.Collections.Generic;
using System.Diagnostics;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LIMSApi.Data;

public partial class LIMSContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LIMSContext(DbContextOptions<LIMSContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual DbSet<AreaMaster> AreaMasters { get; set; }
    public virtual DbSet<BankMaster> BankMasters { get; set; }
    public virtual DbSet<CalibrationAgencyMaster> CalibrationAgencyMasters { get; set; }
    public virtual DbSet<CityMaster> CityMasters { get; set; }
    public virtual DbSet<ClassificationMaster> ClassificationMasters { get; set; }
    public virtual DbSet<CompanyMaster> CompanyMasters { get; set; }

    public virtual DbSet<CountryMaster> CountryMasters { get; set; }
    public virtual DbSet<CourierMaster> CourierMasters { get; set; }

    public virtual DbSet<CurrencyMaster> CurrencyMasters { get; set; }
    public virtual DbSet<CustomerTypeMaster> CustomerTypeMasters { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<ContactPerson> ContactPersons { get; set; }

    public virtual DbSet<DepartmentMaster> DepartmentMasters { get; set; }

    public virtual DbSet<DesignationMaster> DesignationMasters { get; set; }

    public virtual DbSet<DimensionalFactorMaster> DimensionalFactorMasters { get; set; }
    public virtual DbSet<DisciplineMaster> DisciplineMasters { get; set; }
    public virtual DbSet<DispatchModeMaster> DispatchModeMasters { get; set; }
    public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }
    public virtual DbSet<EmployeeQualification> EmployeeQualifications { get; set; }
    public virtual DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
    public virtual DbSet<EquipmentMaster> EquipmentMasters { get; set; }

    public virtual DbSet<EquipmentTypeMaster> EquipmentTypeMasters { get; set; }

    public virtual DbSet<HeatTreatmentMaster> HeatTreatmentMasters { get; set; }

    public virtual DbSet<ItemMaster> ItemMasters { get; set; }
    public virtual DbSet<IndustryMaster> IndustryMasters { get; set; }
    public virtual DbSet<GroupMaster> GroupMasters { get; set; }
    public virtual DbSet<LabScopeMaster> LabScopeMasters { get; set; }
    public virtual DbSet<MakerMaster> MakerMasters { get; set; }
    public virtual DbSet<OEMMaster> OEMMasters { get; set; }
    public virtual DbSet<OrganisationMaster> OrganisationMasters { get; set; }
    public virtual DbSet<ParameterMaster> ParameterMasters { get; set; }

    public virtual DbSet<ParameterUnitMaster> ParameterUnitMasters { get; set; }

    public virtual DbSet<PermissionMaster> PermissionMasters { get; set; }

    public virtual DbSet<ProductConditionMaster> ProductConditionMasters { get; set; }

    public virtual DbSet<RoleMaster> RoleMasters { get; set; }
    public virtual DbSet<RemarkMaster> RemarkMasters { get; set; }

    public virtual DbSet<SiteActivity> SiteActivities { get; set; }

    public virtual DbSet<SiteError> SiteErrors { get; set; }

    public virtual DbSet<SpecificationHeader> SpecificationHeaders { get; set; }

    public virtual DbSet<SpecificationLine> SpecificationLines { get; set; }

    public virtual DbSet<SpecimenOrientationMaster> SpecimenOrientationMasters { get; set; }
    public virtual DbSet<SpecimenTypeMaster> SpecimenTypeMasters { get; set; }

    public virtual DbSet<StandardOrganizationMaster> StandardOrganizationMasters { get; set; }
    public virtual DbSet<SubContractorMaster> SubContractorMasters { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }
    public virtual DbSet<SubGroupMaster> SubGroupMasters { get; set; }
    public virtual DbSet<SupplierMaster> SupplierMasters { get; set; }
    public virtual DbSet<TaxMaster> TaxMasters { get; set; }
    public virtual DbSet<TestGroup> TestGroups { get; set; }
    public virtual DbSet<TestGroupMapping> TestGroupMappings { get; set; }
    public virtual DbSet<TestMaster> TestMasters { get; set; }
    public virtual DbSet<TestMethodMaster> TestMethodMasters { get; set; }
    public virtual DbSet<TestMethodSubGroup> TestMethodSubGroups { get; set; }
    public virtual DbSet<TestMethodStandard> TestMethodStandards { get; set; }
    public virtual DbSet<TestTypeMaster> TestTypeMasters { get; set; }
    public virtual DbSet<TPIMaster> TPIMasters { get; set; }

    public virtual DbSet<UniversalCodeTypeMaster> UniversalCodeTypeMasters { get; set; }
    public virtual DbSet<UploadFile> UploadFiles { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }
    public virtual DbSet<UOMMaster> UOMMasters { get; set; }
    public virtual DbSet<VendorMaster> VendorMasters { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the BankName= syntax to read it from _configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=LIMS_Backup;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<CompanyMaster>(entity =>
        //{
        //    entity.ToTable("CompanyMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.AreaID).HasColumnName("AreaID");
        //    entity.Property(e => e.CityID).HasColumnName("CityID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CompanyName).HasMaxLength(50);
        //    entity.Property(e => e.CompanyType).HasMaxLength(50);
        //    entity.Property(e => e.CountryID).HasColumnName("CountryID");
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.CurrencyID).HasColumnName("CurrencyID");
        //    entity.Property(e => e.EmailId)
        //        .HasMaxLength(100)
        //        .HasColumnName("EmailId");
        //    entity.Property(e => e.Gstno)
        //        .HasColumnType("image")
        //        .HasColumnName("GSTNo");
        //    entity.Property(e => e.Logo).HasColumnType("image");
        //    entity.Property(e => e.ContactNo1).HasMaxLength(20);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.PhoneNo).HasMaxLength(20);
        //    entity.Property(e => e.RegistrationNo).HasMaxLength(20);
        //    entity.Property(e => e.StateID).HasColumnName("StateID");
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //    entity.Property(e => e.Vatno)
        //        .HasMaxLength(20)
        //        .HasColumnName("VATNo");
        //    entity.Property(e => e.Website).HasMaxLength(100);
        //});

        //modelBuilder.Entity<CountryMaster>(entity =>
        //{
        //    entity.HasKey(e => e.ID).HasName("PK_Country");

        //    entity.ToTable("CountryMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.Code).HasMaxLength(10);
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(50);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<CurrencyMaster>(entity =>
        //{
        //    entity.HasKey(e => e.ID).HasName("PK_Currency");

        //    entity.ToTable("CurrencyMaster");

        //    entity.Property(e => e.ID)
        //        .ValueGeneratedNever()
        //        .HasColumnName("ID");
        //    entity.Property(e => e.Code).HasMaxLength(50);
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(50);
        //    entity.Property(e => e.IsActive)
        //         .HasDefaultValue((byte)0)
        //         .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<DepartmentMaster>(entity =>
        //{
        //    entity.HasKey(e => e.ID).HasName("PK_Department");

        //    entity.ToTable("DepartmentMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Department).HasMaxLength(100);
        //    entity.Property(e => e.DepartmentCode).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<DesignationMaster>(entity =>
        //{
        //    entity.HasKey(e => e.ID).HasName("PK_Designation");

        //    entity.ToTable("DesignationMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Designation).HasMaxLength(100);
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<DimensionalFactorsMaster>(entity =>
        //{
        //    entity.ToTable("DimensionalFactorsMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(100);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<EmployeeMaster>(entity =>
        //{
        //    entity.ToTable("EmployeeMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.BirthDate).HasColumnType("datetime");
        //    entity.Property(e => e.BloodGroup).HasMaxLength(50);
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.DepartmentID).HasColumnName("DepartmentID");
        //    entity.Property(e => e.DesignationID).HasColumnName("DesignationID");
        //    entity.Property(e => e.EmailId).HasMaxLength(50);
        //    entity.Property(e => e.EmergencyMobileNo).HasMaxLength(50);
        //    entity.Property(e => e.FatherName).HasMaxLength(50);
        //    entity.Property(e => e.Gender)
        //        .HasMaxLength(5)
        //        .IsUnicode(false)
        //        .IsFixedLength();
        //    entity.Property(e => e.JoinDate).HasColumnType("datetime");
        //    entity.Property(e => e.ContactNo1).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.MotherName).HasMaxLength(50);
        //    entity.Property(e => e.BankName).HasMaxLength(50);
        //    entity.Property(e => e.SpouseName).HasMaxLength(50);
        //    entity.Property(e => e.IsActive)
        //         .HasDefaultValue((byte)0)
        //         .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //    entity.Property(e => e.TestTypeID).HasColumnName("TestTypeID");
        //    entity.Property(e => e.UserID).HasColumnName("UserID");
        //});

        //modelBuilder.Entity<EquipmentTypeMaster>(entity =>
        //{
        //    entity.ToTable("EquipmentTypeMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(100);
        //});

        //modelBuilder.Entity<HeatTreatmentMaster>(entity =>
        //{
        //    entity.ToTable("HeatTreatmentMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(250);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<ParameterMaster>(entity =>
        //{
        //    entity.ToTable("ParameterMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.AliasName).HasMaxLength(500);
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(250);
        //    entity.Property(e => e.ParameterType).HasMaxLength(50);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //    entity.Property(e => e.Uomid).HasColumnName("UOMID");
        //});

        //modelBuilder.Entity<ParameterUnitMaster>(entity =>
        //{
        //    entity.HasKey(e => e.ID).HasName("PK_UOMMaster");

        //    entity.ToTable("ParameterUnitMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(100);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<PermissionMaster>(entity =>
        //{
        //    entity.HasKey(e => e.ID).HasName("PK_Permission");

        //    entity.ToTable("PermissionMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.Addp).HasDefaultValueSql("('No')");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Deletep).HasDefaultValueSql("('No')");
        //    entity.Property(e => e.Editp).HasDefaultValueSql("('No')");
        //    entity.Property(e => e.ExportP).HasDefaultValueSql("('No')");
        //    entity.Property(e => e.MenuID).HasColumnName("MenuID");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.RoleID).HasColumnName("RoleID");
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //    entity.Property(e => e.Viewp).HasDefaultValueSql("('No')");
        //});

        //modelBuilder.Entity<ProductConditionMaster>(entity =>
        //{
        //    entity.ToTable("ProductConditionMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(250);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<RoleMaster>(entity =>
        //{
        //    entity.ToTable("RoleMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    //entity.Property(e => e.CompanyID).HasColumnName("CompanyID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Dashboard).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.RoleName).HasMaxLength(50);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<SiteActivity>(entity =>
        //{
        //    entity.ToTable("SiteActivity");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.Browser).HasMaxLength(50);
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.Ipaddress)
        //        .HasMaxLength(50)
        //        .HasColumnName("IPAddress");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModuleName).HasMaxLength(512);
        //    entity.Property(e => e.TraceId)
        //        .HasMaxLength(256)
        //        .HasColumnName("TraceID");
        //    entity.Property(e => e.WebUrl)
        //        .HasMaxLength(1024)
        //        .HasColumnName("WebURL");
        //});

        //modelBuilder.Entity<SiteError>(entity =>
        //{
        //    entity.ToTable("SiteError");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.Browser).HasMaxLength(50);
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.ErrorCode).HasMaxLength(100);
        //    entity.Property(e => e.Ipaddress)
        //        .HasMaxLength(50)
        //        .HasColumnName("IPAddress");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.WebUrl).HasColumnName("WebURL");
        //});

        //modelBuilder.Entity<SpecificationHeader>(entity =>
        //{
        //    entity.ToTable("SpecificationHeader");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.AliasName).HasMaxLength(250);
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Grade).HasMaxLength(50);
        //    entity.Property(e => e.IsUns)
        //        .HasDefaultValue(false)
        //        .HasComment("IsUNS(0=false=Steel Number, 1=true=UNS Number) ")
        //        .HasColumnName("IsUNS");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.Part).HasMaxLength(50);
        //    entity.Property(e => e.SpecificationCode)
        //        .HasMaxLength(12)
        //        .IsUnicode(false)
        //        .HasComputedColumnSql("('SPEC-'+right('0000000'+CONVERT([varchar](7),[ID]),(7)))", true);
        //    entity.Property(e => e.Standard).HasMaxLength(50);
        //    entity.Property(e => e.StandardOrganizationID).HasColumnName("StandardOrganizationID");
        //    entity.Property(e => e.StandardYear).HasMaxLength(50);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //    entity.Property(e => e.UnsSteelNumber)
        //        .HasMaxLength(50)
        //        .HasColumnName("UNS_SteelNumber");
        //});

        //modelBuilder.Entity<SpecificationLine>(entity =>
        //{
        //    entity.ToTable("SpecificationLine");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.DimensionalFactorID).HasColumnName("DimensionalFactorID");
        //    entity.Property(e => e.HeatTreatmentID).HasColumnName("HeatTreatmentID");
        //    entity.Property(e => e.LowerLimit).HasMaxLength(50);
        //    entity.Property(e => e.LowerLimitValue)
        //        .HasDefaultValue(0m)
        //        .HasColumnType("decimal(18, 2)");
        //    entity.Property(e => e.MaxValue)
        //        .HasDefaultValue(0m)
        //        .HasColumnType("decimal(18, 2)");
        //    entity.Property(e => e.MinValue)
        //        .HasDefaultValue(0m)
        //        .HasColumnType("decimal(18, 2)");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.ParameterID).HasColumnName("ParameterID");
        //    entity.Property(e => e.ProductConditionID1).HasColumnName("ProductConditionID1");
        //    entity.Property(e => e.ProductConditionID2).HasColumnName("ProductConditionID2");
        //    entity.Property(e => e.PropertyType).HasMaxLength(50);
        //    entity.Property(e => e.SpecificationHeaderID).HasColumnName("SpecificationHeaderID");
        //    entity.Property(e => e.SpecimenOrientationID).HasColumnName("SpecimenOrientationID");
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //    entity.Property(e => e.Uomid).HasColumnName("UOMID");
        //    entity.Property(e => e.UpperLimit).HasMaxLength(50);
        //    entity.Property(e => e.UpperLimitValue)
        //        .HasDefaultValue(0m)
        //        .HasColumnType("decimal(18, 2)");
        //});

        //modelBuilder.Entity<SpecimenOrientationMaster>(entity =>
        //{
        //    entity.ToTable("SpecimenOrientationMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(100);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<StandardOrganizationMaster>(entity =>
        //{
        //    entity.ToTable("StandardOrganizationMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(100);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<StateMaster>(entity =>
        //{
        //    entity.HasKey(e => e.ID).HasName("PK_State_1");

        //    entity.ToTable("StateMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.Code).HasMaxLength(50);
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CountryID)
        //        .HasDefaultValue(0L)
        //        .HasColumnName("CountryID");
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Gstcode)
        //        .HasMaxLength(50)
        //        .HasColumnName("GSTCode");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(50);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<UniversalCodeTypeMaster>(entity =>
        //{
        //    entity.ToTable("UniversalCodeTypeMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(dateadd(minute,(330),getutcdate()))")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.BankName).HasMaxLength(100);
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //});

        //modelBuilder.Entity<UserMaster>(entity =>
        //{
        //    entity.ToTable("UserMaster");

        //    entity.Property(e => e.ID).HasColumnName("ID");
        //    entity.Property(e => e.CompanyCode).HasMaxLength(50);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.EmailId)
        //        .HasMaxLength(100)
        //        .HasColumnName("EmailId");
        //    entity.Property(e => e.EmployeeID).HasColumnName("EmployeeID");
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.Password).HasMaxLength(100);
        //    entity.Property(e => e.RoleID)
        //        .HasDefaultValue(0L)
        //        .HasColumnName("RoleID");
        //    entity.Property(e => e.IsActive)
        //        .HasDefaultValue((byte)0)
        //        .HasComment("IsActive(1-Active,0-InActive,2-Delete)");
        //    entity.Property(e => e.UserCode).HasMaxLength(50);
        //    entity.Property(e => e.UserName).HasMaxLength(512);
        //});

        //OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var activities = new List<SiteActivity>();
        var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var browser = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
        var url = _httpContextAccessor.HttpContext?.Request?.Path;

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();


        foreach (var entry in entries)
        {
            var tableName = entry.Metadata.GetTableName();
            var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString();
            var action = entry.State.ToString();

            activities.Add(new SiteActivity
            {
                ModuleName = tableName,
                TraceId = primaryKey,
                Ipaddress = ipAddress,
                Browser = browser,
                Action = action,
                Description = $"{action} on {tableName} (ID: {primaryKey})",
                WebUrl = url,
                ModifiedBy = user,
                ModifiedOn = DateTime.UtcNow
            });

        }
        var result = await base.SaveChangesAsync(cancellationToken);
        foreach (var entry in entries)
        {
            var tableName = entry.Metadata.GetTableName();
            var newPrimaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString();
            var action = entry.State.ToString();

            var activity = activities.FirstOrDefault(a => a.ModuleName == tableName);
            if (activity != null && activity.Action.Trim().ToLower() == "added")
            {
                activity.TraceId = newPrimaryKey;

            }
        }

        // Store in HttpContext.Items temporarily
        if (activities.Any(a => a.ModuleName != "SiteActivity" && a.ModuleName != "SiteError"))
        {
            _httpContextAccessor.HttpContext.Items["ActivityLog"] = activities;
        }
        else
        {
            _httpContextAccessor.HttpContext.Items.Remove("ActivityLog");
        }

        return result;
    }

}
