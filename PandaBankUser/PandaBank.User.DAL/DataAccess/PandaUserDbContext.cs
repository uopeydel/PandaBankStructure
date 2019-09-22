using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PandaBank.SharedService.DataAccess;
using PandaBank.User.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.User.DAL.DataAccess
{
    // 1> Change Start up project from [multi start up project] to [this project] "PandaBank.User"
    // 2> Change your target project to the migrations project by using the drop-down list to "PandaBank.User.DAL"
    // 3> Run command below

    // Add-Migration PandaBankUser -Context PandaUserDbContext
    // Update-Database PandaBankUser -Context PandaUserDbContext

    // # NOTE
    // < PandaBankUser > It's mean store in custom directory Name
    // < PandaUserDbContext > It's mean use this file target by class Name

    // Must have this in IOC
    // services.AddScoped(typeof(IEntityFrameworkRepository<,>), typeof(EntityFrameworkRepository<,>));
    // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    public class PandaUserDbContext :
        IdentityDbContext<PandaUser, PandaRole, long>
    {
        private readonly string _connectionString;
        public PandaUserDbContext(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<DbContextOptions<PandaUserDbContext>>();
            _connectionString = options.FindExtension<SqlServerOptionsExtension>().ConnectionString;

        }

        // Add DbSet<TTable> here
        public virtual DbSet<PandaUser> PandaUser { get; set; }
        public virtual DbSet<PandaRole> Role { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PandaUser>().ToTable("PandaUser");
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
