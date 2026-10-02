namespace LIMSApi.Dtos
{
    public class CrmConsumptionDetailsDto
    {
        public CrmDetailsDto CrmDetails { get; set; }
        public CrmConsumptionHeaderDto? ConsumptionHeader { get; set; }
        public List<CrmConsumptionLogDto> Logs { get; set; } = new();
    }
}
