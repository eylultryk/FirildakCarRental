using Microsoft.AspNetCore.Mvc;
using FirildakAracKiralama.Data;
using FirildakAracKiralama.Models;
using System.Linq;

namespace FirildakAracKiralama.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KullaniciController : ControllerBase
    {
        private readonly UygulamaDbContext _db;

        public KullaniciController(UygulamaDbContext db)
        {
            _db = db;
        }

        // Kayıt
        [HttpPost("kayit")]
        public IActionResult Kayit([FromBody] Kullanici kullanici)
        {
            if (_db.Kullanicilar.Any(x => x.KullaniciAdi == kullanici.KullaniciAdi))
                return BadRequest("Bu kullanıcı adı zaten alınmış.");

            _db.Kullanicilar.Add(kullanici);
            _db.SaveChanges();

            // istersen sadece mesaj dönebilirsin; ben id'yi de ekledim
            return Ok(new { mesaj = "Kayıt başarılı!", id = kullanici.Id });
        }

        // Giriş
        [HttpPost("giris")]
        public IActionResult Giris([FromBody] GirisModel model)
        {
            var kullanici = _db.Kullanicilar
                .FirstOrDefault(x => x.KullaniciAdi == model.KullaniciAdi && x.Sifre == model.Sifre);

            if (kullanici == null)
                return BadRequest("Kullanıcı adı veya şifre hatalı.");

            // login.html bu JSON'u localStorage'a yazacak
            return Ok(new
            {
                id = kullanici.Id,
                adSoyad = kullanici.AdSoyad,
                kullaniciAdi = kullanici.KullaniciAdi
            });
        }
    }

    public class GirisModel
    {
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
    }
}
