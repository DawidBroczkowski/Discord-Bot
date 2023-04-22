using DiscordBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscordBot.DataAccess
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions options) : base(options) { }
        public DbSet<ServerConfig> ServerConfigs { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> AllowedRoles { get; set; }
        public DbSet<SelfRole> SelfRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<ServerConfig>()
                .HasMany(x => x.Permissions)
                .WithOne(x => x.ServerConfig)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<Permission>()
                .HasMany(x => x.AllowedRoles)
                .WithOne(x => x.Permission)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<SelfRole>()
                .HasOne(x => x.ServerConfig)
                .WithMany(x => x.SelfRoles)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class YourFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BotContext>();
            string connStr = File.ReadAllText("ConnectionString.txt");
            optionsBuilder.UseSqlServer(connStr);

            return new BotContext(optionsBuilder.Options);
        }
    }
}
