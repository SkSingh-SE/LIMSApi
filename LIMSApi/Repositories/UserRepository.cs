using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LIMSContext context;
        private LoggedInUserDTO loggedInUser;
        public UserRepository(LIMSContext _context)
        {
            this.context = _context;
            this.loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddUser(UserMaster user)
        {
            await context.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<UserMaster> GetUserByEmail(string email)
        {
            var user = await context.UserMasters.FirstOrDefaultAsync(x => x.EmailId == email);
            return user;
        }

        public async Task<List<UserMaster>> GetAllUserByRoleId(long Id)
        {
            return await context.UserMasters.Where(x => x.RoleID == Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode).ToListAsync();
        }
        public async Task UpdateUser(UserMaster user)
        {
            var userToUpdate = await context.UserMasters.FirstOrDefaultAsync(x => x.EmailId == user.EmailId);
            if (userToUpdate != null)
            {
                userToUpdate.UserName = user.UserName;
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("User not found");
            }
        }

        public async Task UpdateUsers(List<UserMaster> users)
        {
            foreach (var user in users)
            {
                var userToUpdate = await context.UserMasters.FirstOrDefaultAsync(x => x.EmailId == user.EmailId);
                if (userToUpdate != null)
                {
                    userToUpdate.UserName = user.UserName;
                    userToUpdate.RoleID = user.RoleID;
                    userToUpdate.RoleName = user.RoleName;
                    userToUpdate.IsAdmin = user.IsAdmin;
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUser(string email)
        {
            var user = await context.UserMasters.FirstOrDefaultAsync(x => x.EmailId == email);
            if (user != null)
            {
                user.IsActive = false;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<DropdwonSelector>> GetUserDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in context.UserMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.UserName != null && x.UserName.ToLower().Contains(search))
                || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.UserName,
            })).ToListAsync();

            return data;
        }
    }
}
