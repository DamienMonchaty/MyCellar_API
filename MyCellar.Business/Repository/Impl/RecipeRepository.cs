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
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ModelDbContext _context;

        public RecipeRepository()
        {
            _context = new ModelDbContext();
        }

        public async Task<Recipe> Add(Recipe o)
        {
            _context.Recipes.Add(o);
            await _context.SaveChangesAsync();
            return o;
        }

        public async Task<int> Count()
        {
            return await _context.Recipes.CountAsync();
        }

        public async Task Delete(Recipe o)
        {
            _context.Recipes.Remove(o);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Recipe>> GetAll()
        {
            return await _context.Recipes.Include(x => x.RecipeProducts).ThenInclude(x => x.Product).ToListAsync();
        }

        public async Task<PageResult<Recipe>> GetAllPaginate(int? page, int pagesize, string search)
        {
            var query = string.IsNullOrEmpty(search) ? await _context.Recipes.ToListAsync()
                                                      : await _context.Recipes.Where(e => e.Title.ToLower().Contains(search.ToLower())).ToListAsync();

            int total = query.Count();
            PageResult<Recipe> result = new PageResult<Recipe>
            {
                Count = total,
                PageIndex = page ?? 1,
                PageSize = 10,
                Items = query.Skip((page - 1 ?? 00) * pagesize).Take(pagesize).ToList()
            };
            return result;
        }

        public async Task<Recipe> GetById(int id)
        {
            return await _context.Recipes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Recipe> Update(Recipe o)
        {
            var emp = _context.Recipes.Update(o);
            await _context.SaveChangesAsync();
            return emp.Entity;
        }

        public async Task<List<Recipe>> GetAllRecipesByProducts(int[] ids)
        {
            List<int> myTags = new List<int>();
            myTags.AddRange(ids);
            int tagCount = myTags.Count;

            IQueryable<int> subquery =
              from tag in _context.RecipeProducts
              where myTags.Contains(tag.ProductId)
              group tag.ProductId by tag.RecipeId into g
              where g.Count() == tagCount
              select g.Key;

            IQueryable<Recipe> query = _context.Recipes
              .Where(c => subquery.Contains(c.Id));

            var recipes = await query.ToListAsync();

            return recipes;
        }
    }
}
    