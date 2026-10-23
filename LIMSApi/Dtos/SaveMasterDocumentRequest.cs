namespace LIMSApi.Dtos
{
    public class SaveMasterDocumentRequest
    {
        public string Body { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
    }
}
