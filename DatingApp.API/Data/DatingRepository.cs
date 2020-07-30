using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext dataContext;
        public DatingRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;

        }
        public void Add<T>(T entity) where T : class
        {
            dataContext.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            dataContext.Remove(entity);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await dataContext.Users.Include(u => u.Photos).ToListAsync();
            return users;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await dataContext.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<bool> SaveAll()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await dataContext.Photos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await dataContext.Photos.FirstOrDefaultAsync(p => p.UserId == userId && p.IsMain);
        }
    }
}