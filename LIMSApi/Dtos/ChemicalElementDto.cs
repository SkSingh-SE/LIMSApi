namespace LIMSApi.Dtos
{
    public class ChemicalElementDto
    {
        public long SpecificationLineID { get; set; }
        public long? ParameterID { get; set; }
        public string? ParameterName { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public long? ParameterUnitID { get; set; }
        public string? ParameterUnit { get; set; }
    }
}
