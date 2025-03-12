using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LIMSContext context;
        public UserRepository(LIMSContext _context)
        {
            this.context = _context;
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
    }
}
