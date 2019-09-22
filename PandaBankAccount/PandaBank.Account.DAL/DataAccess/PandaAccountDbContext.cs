using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PandaBank.Account.DAL.Models;
using PandaBank.SharedService.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.Account.DAL.DataAccess
{
    // 1> Change Start up project from [multi start up project] to [this project] "PandaBank.Account"
    // 2> Change your target project to the migrations project by using the drop-down list to "PandaBank.Account.DAL"
    // 3> Run command below 

    // Add-Migration PandaBankAccount -Context PandaAccountDbContext
    // Update-Database PandaBankAccount -Context PandaAccountDbContext
    
    // # NOTE
    // < PandaBankAccount > It's mean store in custom directory Name
    // < PandaAccountDbContext > It's mean use this file target by class Name

    // Must have this in IOC
    // services.AddScoped(typeof(IEntityFrameworkRepository<,>), typeof(EntityFrameworkRepository<,>));
    // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    public class PandaAccountDbContext : DbContext
    {
        private readonly string _connectionString;
        public PandaAccountDbContext(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<DbContextOptions<PandaAccountDbContext>>();
            _connectionString = options.FindExtension<SqlServerOptionsExtension>().ConnectionString;
        }


        public virtual DbSet<PandaStatement> PandaStatement { get; set; }
        public virtual DbSet<UserAccount> UserAccount { get; set; }
        public virtual DbSet<PandaAccount> PandaAccount { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PandaStatement>()
                .HasKey(c => new { c.PandaAccountId, c.Id });
            builder.Entity<UserAccount>()
               .HasKey(c => new { c.PandaAccountId, c.PandaUserId });

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

#if DEBUG
            var logEntitySql = new LoggerFactory();
            logEntitySql.AddProvider(new SqlLoggerProvider());
            optionsBuilder.UseLoggerFactory(logEntitySql).UseSqlServer(_connectionString);
#else
            if (optionsBuilder.IsConfigured)
            {
                return;
            }
            optionsBuilder.UseSqlServer(_connectionString  );
#endif

        }

    }
}
