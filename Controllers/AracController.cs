using FirildakAracKiralama.Data;
using FirildakAracKiralama.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FirildakAracKiralama.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AracController : ControllerBase
    {
        private readonly UygulamaDbContext _db;

        public AracController(UygulamaDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAraclar()
        {
            var araclar = _db.Araclar.ToList();
            return Ok(araclar);
        }

        [HttpPost]
        public IActionResult AracEkle([FromBody] Arac arac)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _db.Araclar.Add(arac);
            _db.SaveChanges();

            return Ok("Araç eklendi!");
        }

        [HttpPut("{id}")]
        public IActionResult DurumGuncelle(int id, [FromBody] JsonElement veri)
        {
            var arac = _db.Araclar.FirstOrDefault(a => a.Id == id);
            if (arac == null)
                return NotFound("Araç bulunamadı.");

            if (!veri.TryGetProperty("durum", out var durumProp))
                return BadRequest("Durum bilgisi eksik.");

            arac.Durum = durumProp.GetString();
            _db.SaveChanges();

            return Ok("Durum güncellendi!");
        }
    }
}
