namespace LIMSApi.Repositories.Interface
{
    public interface IProformaInvoiceRepository
    {
        Task<long> GeneratePIAsync(long inwardId, bool applyGST, bool isInterState);
        Task<string> GeneratePINoAsync();
        Task<byte[]> GeneratePIPdfAsync(long piId);
    }
}
