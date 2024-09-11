using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB.Entity;

namespace WWC._240711.ASPNETCore.Extensions.Logging.Custom.Db
{
    public class LoggerDbContext : DbContext
    {
        public LoggerDbContext()
        {
            
        }

        public LoggerDbContext(DbContextOptions<LoggerDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// 日志记录表
        /// </summary>
        public virtual DbSet<CustomLogEntry> CustomLogEntrys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=.;uid=sa;pwd=123456;database=CustomConfiguration;Integrated Security=True;TrustServerCertificate=True");
            base.OnConfiguring(optionsBuilder);
        }

    }
}
