using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirildakAracKiralama.Data;
using FirildakAracKiralama.Models;

namespace FirildakAracKiralama.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KiralamaController : ControllerBase
    {
        private readonly UygulamaDbContext _db;
        public KiralamaController(UygulamaDbContext db) => _db = db;

       
        public class KiralamaCreateDto
        {
            public int KullaniciId { get; set; }
            public int AracId { get; set; }
            public DateTime BaslangicTarihi { get; set; }
            public DateTime BitisTarihi { get; set; }
        }

       
        [HttpGet]
        public IActionResult TumKiralamalar()
        {
            var tum = _db.Kiralamalar
                .Include(k => k.Kullanici)
                .Include(k => k.Arac)
                .OrderByDescending(k => k.Id)
                .Select(k => new
                {
                    k.Id,
                    Kullanici = k.Kullanici != null ? k.Kullanici.AdSoyad : "(silinmiş)",
                    Arac = k.Arac != null ? k.Arac.MarkaModel : "(silinmiş)",
                    k.BaslangicTarihi,
                    k.BitisTarihi,
                    k.ToplamUcret,
                    k.IadeTarihi,
                    k.CezaUcreti
                })
                .ToList();

            return Ok(tum);
        }

       
        [HttpGet("kullanici/{id}")]
        public IActionResult KullaniciKiralamalari(int id, [FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis)
        {
            var q = _db.Kiralamalar
                .Include(k => k.Arac)
                .Where(k => k.KullaniciId == id)
                .AsQueryable();

            if (baslangic.HasValue) q = q.Where(k => k.BaslangicTarihi >= baslangic.Value);
            if (bitis.HasValue) q = q.Where(k => k.BitisTarihi <= bitis.Value);

            var sonuc = q.OrderByDescending(k => k.Id)
                .Select(k => new
                {
                    k.Id,
                    Arac = k.Arac != null ? k.Arac.MarkaModel : "(silinmiş)",
                    k.BaslangicTarihi,
                    k.BitisTarihi,
                    k.ToplamUcret,
                    k.IadeTarihi,
                    k.CezaUcreti
                })
                .ToList();

            return Ok(sonuc);
        }

       
        [HttpPost]
        public IActionResult Kirala([FromBody] KiralamaCreateDto dto)
        {
            
            if (dto.BitisTarihi.Date < dto.BaslangicTarihi.Date)
                return BadRequest("Geçersiz tarih aralığı.");

            var arac = _db.Araclar.FirstOrDefault(a => a.Id == dto.AracId);
            var kullanici = _db.Kullanicilar.FirstOrDefault(u => u.Id == dto.KullaniciId);

            if (arac is null || kullanici is null)
                return BadRequest("Araç veya kullanıcı bulunamadı.");

            if (!string.Equals(arac.Durum ?? "Uygun", "Uygun", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Araç uygun değil.");

            var gun = Math.Max(1, (dto.BitisTarihi.Date - dto.BaslangicTarihi.Date).Days);
            var toplam = gun * arac.GunlukUcret;

            var kiralama = new Kiralama
            {
                KullaniciId = dto.KullaniciId,
                AracId = dto.AracId,
                BaslangicTarihi = dto.BaslangicTarihi.Date,
                BitisTarihi = dto.BitisTarihi.Date,
                ToplamUcret = toplam
            };

            
            arac.Durum = "Kiralandı";
            _db.Kiralamalar.Add(kiralama);
            _db.SaveChanges();

            return Ok(new
            {
                mesaj = "Araç kiralandı!",
                kiralamaId = kiralama.Id,
                toplamUcret = kiralama.ToplamUcret
            });
        }

        
        [HttpPost("iade/{kiralamaId}")]
        public IActionResult Iade(int kiralamaId)
        {
            var k = _db.Kiralamalar
                .Include(x => x.Arac)
                .FirstOrDefault(x => x.Id == kiralamaId);

            if (k is null) return NotFound("Kiralama bulunamadı.");
            if (k.Arac is null) return NotFound("Araç bulunamadı.");

            
            k.Arac.Durum = "Uygun";

            
            var bugun = DateTime.Today;
            k.IadeTarihi = bugun;

            var gecikmeGun = (bugun - k.BitisTarihi.Date).Days;
            decimal ceza = 0;
            if (gecikmeGun > 0)
            {
                ceza = gecikmeGun * k.Arac.GunlukUcret * 0.5m; 
                k.CezaUcreti = ceza;
            }

            _db.SaveChanges();

            return Ok(new { mesaj = "Araç iade edildi.", gecikmeGun, ceza });
        }
    }
}
