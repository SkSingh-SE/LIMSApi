namespace LIMSApi.Repositories.Interface
{
    public interface IProformaInvoiceRepository
    {
        Task<long> GeneratePIAsync(long inwardId);
        Task<string> GeneratePINoAsync();
        Task<byte[]> GeneratePIPdfAsync(long piId);
    }
}
