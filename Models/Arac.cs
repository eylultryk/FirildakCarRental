namespace FirildakAracKiralama.Models
{
    public class Arac
    {
        public int Id { get; set; }
        public string MarkaModel { get; set; }
        public string Plaka { get; set; }
        public decimal GunlukUcret { get; set; }
        public string Durum { get; set; } = "Uygun";
    }
}
