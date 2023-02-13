using Microsoft.EntityFrameworkCore;
using MyCellar.Business.Context;
using MyCellar.Business.Wrappers;
using MyCellar.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCellar.Business.Repository.Impl
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly ModelDbContext _context;

        public CategoryRepository()
        {
            _context = new ModelDbContext();
        }

        public async Task<Category> Add(Category o)
        {
            _context.Categories.Add(o);
            await _context.SaveChangesAsync();
            return o;
        }

        public async Task<int> Count()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task Delete(Category o)
        {
            _context.Categories.Remove(o);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories.Include(x => x.Products).ToListAsync();
        }

        public async Task<PageResult<Category>> GetAllPaginate(int? page, int pagesize, string search)
        {
            var query = string.IsNullOrEmpty(search) ? await _context.Categories.ToListAsync()
                                                      : await _context.Categories.Where(e => e.Title.ToLower().Contains(search.ToLower())).ToListAsync();

            int total = query.Count();
            PageResult<Category> result = new PageResult<Category>
            {
                Count = total,
                PageIndex = page ?? 1,
                PageSize = 10,
                Items = query.Skip((page - 1 ?? 00) * pagesize).Take(pagesize).ToList()
            };
            return result;
        }

        public async Task<Category> GetById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Category> Update(Category o)
        {
            _context.Entry(o).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return o;
        }
    }
}
