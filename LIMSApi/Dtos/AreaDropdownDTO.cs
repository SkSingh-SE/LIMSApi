namespace LIMSApi.Dtos
{
    public class AreaDropdownDTO
    {
        public long AreaId { get; set; }
        public long? CityId { get; set; }
        public long? StateId { get; set; }
        public long? CountryId { get; set; }
        public string? AreaName { get; set; }
        public string? CityName { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }    
    }
}
