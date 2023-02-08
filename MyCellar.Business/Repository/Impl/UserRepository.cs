using Microsoft.EntityFrameworkCore;
using MyCellar.Business.Context;
using MyCellar.Business.Wrappers;
using MyCellar.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace MyCellar.Business.Repository.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly ModelDbContext _context;

        public UserRepository()
        {
            _context = new ModelDbContext();
        }

        public async Task<User> Add(User o)
        {
            User uToSave = new User
            {
                UserName = o.UserName,
                Email = o.Email,
                Password = BC.HashPassword(o.Password),
                Sexe = o.Sexe,
            };
            _context.Users.Add(uToSave);
            await _context.SaveChangesAsync();
            return o;
        }

        public async Task Delete(User o)
        {
            _context.Users.Remove(o);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.Include("Address").ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetByUserNameAndPassword(string userName, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName && x.Password == password);
        }

        public async Task<User> GetByUserName(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<User> Update(User o)
        {
            var user = _context.Users.Update(o);
            await _context.SaveChangesAsync();
            return user.Entity;
        }

        public async Task<string> GetEmailByUserId(int id)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            return u.Email;
        }

        public async Task<int> Count()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<PageResult<User>> GetAllPaginate(int? page, int pagesize, string search)
        {
            var query = string.IsNullOrEmpty(search) ? await _context.Users.ToListAsync()
                                                      : await _context.Users.Where(e => e.UserName.ToLower().Contains(search.ToLower())).ToListAsync();

            int total = query.Count();
            PageResult<User> result = new PageResult<User>
            {
                Count = total,
                PageIndex = page ?? 1,
                PageSize = 10,
                Items = query.Skip((page - 1 ?? 00) * pagesize).Take(pagesize).ToList()
            };
            return result;
        }
    }
}

