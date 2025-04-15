using Microsoft.EntityFrameworkCore;
using MODUL_BUL.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODUL_BUL.Context
{
    public class MyDbContext:DbContext
    {
        public DbSet<ISEMIRLERI> ISEMIRLERI { get; set; }
        public DbSet<URETIM_MALZEME_PLANLAMA> URETIM_MALZEME_PLANLAMA { get; set; }
        public DbSet<ISEMIRLERI_USER> ISEMIRLERI_USER { get; set; }
        public DbSet<STOKLAR> STOKLAR { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Burada veritabanı bağlantı bilgilerini tanımlayın.
            // Örnek olarak SQL Server kullanalım:
            string connectionString = "Data Source=192.168.2.250;Initial Catalog=MikroDB_V16_ICM;Integrated Security=True;Connect Timeout=10;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tablo adını ve sütun adlarını özelleştirme
            modelBuilder.Entity<ISEMIRLERI>().ToTable("ISEMIRLERI").HasKey(x => x.is_Guid);
            modelBuilder.Entity<ISEMIRLERI>().Property(e => e.is_ProjeKodu).HasColumnName("is_ProjeKodu");
            // Tablo adını ve sütun adlarını özelleştirme
            modelBuilder.Entity<URETIM_MALZEME_PLANLAMA>().ToTable("URETIM_MALZEME_PLANLAMA").HasKey(x => x.upl_Guid);
            modelBuilder.Entity<ISEMIRLERI_USER>().ToTable("ISEMIRLERI_USER").HasKey(x => x.Record_uid);
            modelBuilder.Entity<STOKLAR>().ToTable("STOKLAR").HasKey(x => x.sto_Guid);
        }
    }
}
