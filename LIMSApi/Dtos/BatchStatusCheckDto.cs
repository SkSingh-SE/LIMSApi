namespace LIMSApi.Dtos
{
    public class BatchStatusCheckDto
    {
        public List<long> EntityIds { get; set; } = new();
        public string EntityType { get; set; } = string.Empty;
    }
}
