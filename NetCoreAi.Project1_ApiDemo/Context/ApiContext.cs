using Microsoft.EntityFrameworkCore;
using NetCoreAi.Project1_ApiDemo.Entities;
using System.Data.Common;

namespace NetCoreAi.Project1_ApiDemo.Context
{
    public class ApiContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ABDULLAH;initial catalog=NetCoreAiDb;integrated security=true;trustservercertificate=true");
        }
            public DbSet<Customer> Customers { get; set; }
    }
}
