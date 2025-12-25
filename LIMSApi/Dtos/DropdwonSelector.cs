namespace LIMSApi.Dtos
{
    public class DropdwonSelector
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object>? AdditionalValues { get; set; }
    }

}
