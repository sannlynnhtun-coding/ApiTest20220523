using ApiTest20220523.Models;
using ApiTest20220523.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest20220523.Context
{
    public class AppDbContext :DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(StaticConfigService.Configuration.GetSection("ConnectionStrings:DbStr").Value);
        }

        public DbSet<UserModel> Users { get; set; }
    }
}
