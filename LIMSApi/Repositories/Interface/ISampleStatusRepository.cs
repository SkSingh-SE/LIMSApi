
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISampleStatusRepository
    {
        Task<SampleDetail> GetSample(long sampleId);
        Task<SampleInward> GetInward(long inwardId);
        Task Save();
    }
}
