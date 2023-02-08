
using Microsoft.EntityFrameworkCore;
using System;


namespace MyCellar.Business.Context
{
    public class ModelDbContext : DbContext
    {
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connetionString = "server=localhost:3307;database=mybase;user=root;password=root";
            optionsBuilder.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
    }
}
