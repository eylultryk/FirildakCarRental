namespace FirildakAracKiralama.Models
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; } 
        public string Rol { get; set; } = "kullanici";
    }
}
