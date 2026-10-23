namespace LIMSApi.Dtos;

public class GroupedUnitDropdownOption
{
    public long Id { get; set; }
    public long? EquivalentId { get; set; }
    public required string Name { get; set; }
    public string? GroupName { get; set; }
    public bool IsHeader { get; set; }
    public bool IsChild { get; set; }
    public bool IsBase { get; set; }
    public decimal? ConversionFactor { get; set; }
}
