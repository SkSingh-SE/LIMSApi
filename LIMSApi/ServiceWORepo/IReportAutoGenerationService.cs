namespace LIMSApi.ServiceWORepo
{
    public interface IReportAutoGenerationService
    {
        Task GenerateAsync(long sampleId);
        Task<byte[]> GeneratePreviewAsync(long sampleId, long? formatId = null);
    }
}
