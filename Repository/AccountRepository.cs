
using Microsoft.EntityFrameworkCore;

namespace Campus360.Services{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;
        public AccountRepository(AppDbContext context){
            _context = context;
        }
        public async Task<bool> CreateUser(User user)
        {
            await _context.Users.AddAsync(user);   
            return await Save();
        }

        public async Task<bool> DeleteUser(string id)
        {
            var user = await GetUserById(id);
            if(user == null)return false;
            _context.Users.Remove(user);
            return await Save();
        }

        public async Task<User> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByName(string name)
        {
            return await _context.Users.Where(u => u.UserName == name).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUser(User user)
        {
            _context.Users.Update(user);
            return await Save();
        }
        public async Task<bool> Save(){
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}