using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SampleStatusRepository : ISampleStatusRepository
    {
        private readonly LIMSContext _db;

        public SampleStatusRepository(LIMSContext db)
        {
            _db = db;
        }

        public Task<SampleDetail> GetSample(long sampleId)
            => _db.SampleDetails.Include(x => x.SampleInward).FirstOrDefaultAsync(x => x.ID == sampleId);

        public Task<SampleInward> GetInward(long inwardId)
            => _db.SampleInwards.Include(x => x.SampleDetails).FirstOrDefaultAsync(x => x.ID == inwardId);

        public Task Save() => _db.SaveChangesAsync();
    }

}
