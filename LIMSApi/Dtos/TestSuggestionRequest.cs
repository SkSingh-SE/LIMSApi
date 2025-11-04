namespace LIMSApi.Dtos
{
    public class TestSuggestionRequest
    {
        public long? MetalClassificationID { get; set; }
        public long? ProductConditionID { get; set; }
        public string GradeIDs { get; set; }
    }
}
