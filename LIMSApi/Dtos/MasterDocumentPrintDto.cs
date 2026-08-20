namespace LIMSApi.Dtos
{
    public class MasterDocumentPrintDto
    {
        public string? DocumentCode { get; set; }
        public string? DocumentTitle { get; set; }
        public string? DocumentNo { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentOwner { get; set; }
        public string? IssueNo { get; set; }
        public string? RevNo { get; set; }
        public string? CopyHolders { get; set; }
    }

    public class ControlledCopyPrintDto
    {
        public string? HolderName { get; set; }
    }
}