using MyCellar.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCellar.Business.Repository
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        Task<List<Recipe>> GetAllRecipesByProducts(int[] ids);
    }
}
