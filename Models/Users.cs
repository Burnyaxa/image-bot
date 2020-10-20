using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace image_bot.Models
{
    public class UsersState : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ApplyFilterRequest> ApplyFilterRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(u => u.CurentCommand).HasDefaultValue(BotCommand.Start);
            modelBuilder.Entity<ApplyFilterRequest>().Property(a => a.Status).HasDefaultValue(ApplyFilterStus.Inicial);
        }

        public UsersState()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AppSettings.SQLServerConnection);
        }
    }
}
