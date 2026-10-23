namespace LIMSApi.Dtos;

/// <summary>
/// A selectable unit option for a parameter: the base unit (EquivalentId = null)
/// or one of its equivalents (EquivalentId = ParameterUnitEquivalent.ID).
/// BaseUnitId is always the canonical ParameterUnitMaster.ID.
/// </summary>
public class EquivalentUnitOption
{
    public long? EquivalentId { get; set; }
    public long BaseUnitId { get; set; }
    public required string Name { get; set; }
    public decimal? ConversionFactor { get; set; }
    public bool IsBase { get; set; }
}
