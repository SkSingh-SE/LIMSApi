using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly LIMSContext _context;
        public FileUploadRepository(LIMSContext context)
        {
            _context = context;
        }
        public async Task<UploadFile> UploadFileAsync(UploadFile file)
        {
            await _context.UploadFiles.AddAsync(file);
            await _context.SaveChangesAsync();
            return file;
        }
        public async Task<UploadFile> GetFileAsync(long id)
        {
            if (LoggedInUserProvider.CurrentUser == null)
            {
                return await _context.UploadFiles.FirstOrDefaultAsync(f => f.ID == id);
            }
            else
            {
                var companyCode = LoggedInUserProvider.CurrentUser.CompanyCode;
                return await _context.UploadFiles.FirstOrDefaultAsync(f => f.ID == id && companyCode == f.CompanyCode);
            }
            
        }
    }
}
