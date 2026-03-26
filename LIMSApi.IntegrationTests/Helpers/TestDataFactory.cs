using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Helpers;

/// <summary>
/// Generates valid test payloads for each entity type with unique names to avoid conflicts.
/// </summary>
public static class TestDataFactory
{
    private static string Unique(string name) => $"{TestConstants.TestPrefix}{name}";

    // ───────── Master Data ─────────

    public static object Department(string? name = null) => new
    {
        name = name ?? Unique("Dept"),
        code = Unique("D"),
        description = "Test department",
        isActive = true
    };

    public static object Designation(string? name = null) => new
    {
        name = name ?? Unique("Desig"),
        code = Unique("DG"),
        description = "Test designation",
        isActive = true
    };

    public static object Employee(long departmentId, long designationId, string? name = null) => new
    {
        name = name ?? Unique("Emp"),
        code = Unique("E"),
        departmentID = departmentId,
        designationID = designationId,
        email = $"{Unique("emp")}@test.com",
        phone = "9876543210",
        isActive = true
    };

    public static object Customer(string? name = null) => new
    {
        name = name ?? Unique("Cust"),
        code = Unique("C"),
        email = $"{Unique("cust")}@test.com",
        phone = "9876543210",
        address = "Test Address",
        city = "Test City",
        state = "Test State",
        pinCode = "400001",
        country = "India",
        gstNo = "22AAAAA0000A1Z5",
        customerType = "Regular",
        tallyLedgerName = Unique("TL"),
        isActive = true
    };

    public static object EquipmentType(string? name = null) => new
    {
        name = name ?? Unique("EqType"),
        code = Unique("ET"),
        isActive = true
    };

    public static object Equipment(long equipmentTypeId, string? name = null) => new
    {
        name = name ?? Unique("Equip"),
        code = Unique("EQ"),
        equipmentNo = Unique("EN"),
        equipmentTypeID = equipmentTypeId,
        make = "Test Make",
        model = "Test Model",
        serialNo = Unique("SN"),
        isActive = true
    };

    public static object Parameter(string? name = null) => new
    {
        name = name ?? Unique("Param"),
        code = Unique("P"),
        unit = "mm",
        isActive = true
    };

    public static object Tax(string? name = null) => new
    {
        name = name ?? Unique("Tax"),
        percentage = 18.0,
        isActive = true
    };

    public static object Bank(string? name = null) => new
    {
        name = name ?? Unique("Bank"),
        accountNo = Unique("ACC"),
        ifscCode = "TEST0001234",
        branch = "Test Branch",
        isActive = true
    };

    public static object Courier(string? name = null) => new
    {
        name = name ?? Unique("Courier"),
        code = Unique("CR"),
        isActive = true
    };

    public static object Area(string? name = null) => new
    {
        name = name ?? Unique("Area"),
        code = Unique("AR"),
        isActive = true
    };

    public static object State(string? name = null) => new
    {
        name = name ?? Unique("State"),
        code = Unique("ST"),
        isActive = true
    };

    public static object City(long stateId, string? name = null) => new
    {
        name = name ?? Unique("City"),
        code = Unique("CT"),
        stateID = stateId,
        isActive = true
    };

    public static object Country(string? name = null) => new
    {
        name = name ?? Unique("Country"),
        code = Unique("CN"),
        isActive = true
    };

    public static object Group(string? name = null) => new
    {
        name = name ?? Unique("Group"),
        code = Unique("GR"),
        isActive = true
    };

    public static object SubGroup(long groupId, string? name = null) => new
    {
        name = name ?? Unique("SubGrp"),
        code = Unique("SG"),
        groupID = groupId,
        isActive = true
    };

    public static object StandardOrganization(string? name = null) => new
    {
        name = name ?? Unique("StdOrg"),
        code = Unique("SO"),
        isActive = true
    };

    public static object DimensionalFactor(string? name = null) => new
    {
        name = name ?? Unique("DimFact"),
        code = Unique("DF"),
        isActive = true
    };

    public static object HeatTreatment(string? name = null) => new
    {
        name = name ?? Unique("HT"),
        code = Unique("HT"),
        isActive = true
    };

    public static object ProductCondition(string? name = null) => new
    {
        name = name ?? Unique("ProdCond"),
        code = Unique("PC"),
        isActive = true
    };

    public static object SpecimenOrientation(string? name = null) => new
    {
        name = name ?? Unique("SpecOrient"),
        code = Unique("SO"),
        isActive = true
    };

    public static object MetalClassification(string? name = null) => new
    {
        name = name ?? Unique("Metal"),
        code = Unique("MC"),
        isActive = true
    };

    public static object CompanyCategory(string? name = null) => new
    {
        name = name ?? Unique("CompCat"),
        code = Unique("CC"),
        isActive = true
    };

    public static object Discipline(string? name = null) => new
    {
        name = name ?? Unique("Disc"),
        code = Unique("DC"),
        isActive = true
    };

    public static object Industry(string? name = null) => new
    {
        name = name ?? Unique("Ind"),
        code = Unique("IN"),
        isActive = true
    };

    // ───────── Page Filters ─────────

    public static object DefaultPageFilter() => new
    {
        pageNumber = 1,
        pageSize = 10,
        searchTerm = (string?)null,
        sortByColumn = "ID",
        sortOrder = "desc",
        filter = Array.Empty<object>()
    };

    public static object SearchPageFilter(string term) => new
    {
        pageNumber = 1,
        pageSize = 10,
        searchTerm = term,
        sortByColumn = "ID",
        sortOrder = "desc",
        filter = Array.Empty<object>()
    };
}
