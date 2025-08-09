using FirildakAracKiralama.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FirildakAracKiralama.Data
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Arac> Araclar { get; set; }
        public DbSet<Kiralama> Kiralamalar { get; set; }
    }
}
