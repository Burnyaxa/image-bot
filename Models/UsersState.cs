using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace image_bot.Models
{
    public class UsersState : DbContext
    {
        public DbSet<BotUser> BotUsers { get; set; }
        public DbSet<ApplyFilterRequest> ApplyFilterRequests { get; set; }
        public DbSet<ImageResizeRequest> ImageResizeRequests { get; set; }
        public DbSet<CreateMicroStickersRequest> CreateMicroStickersRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotUser>().Property(u => u.CurentCommand).HasDefaultValue(BotCommand.Start);
            modelBuilder.Entity<ApplyFilterRequest>().Property(a => a.Status).HasDefaultValue(ApplyFilterStus.AwaitingImage);
            modelBuilder.Entity<ImageResizeRequest>().Property(i => i.Width).HasDefaultValue(null);
            modelBuilder.Entity<ImageResizeRequest>().Property(i => i.Height).HasDefaultValue(null);
            modelBuilder.Entity<ImageResizeRequest>().Property(i => i.Status).HasDefaultValue(ImageResizeStatus.AwaitingSize);
            modelBuilder.Entity<CreateMicroStickersRequest>().Property(c => c.Status).HasDefaultValue(MicroStickersStatus.AwaitingName);
            modelBuilder.Entity<CreateMicroStickersRequest>().Property(c => c.Name).HasDefaultValue(null);
        }

        public UsersState(DbContextOptions<UsersState> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AppSettings.SQLServerConnection);
        }
    }
}
