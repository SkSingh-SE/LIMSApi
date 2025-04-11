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
        public async Task<UploadFile> UploadFile(UploadFile file)
        {
            await _context.UploadFiles.AddAsync(file);
            await _context.SaveChangesAsync();
            return file;
        }
        public async Task<UploadFile> GetUploadFile(long id)
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
        public async Task RemoveFile(UploadFile file)
        {
            file.IsActive = false;
            _context.UploadFiles.Update(file);
            await _context.SaveChangesAsync();
        }
    }
}
