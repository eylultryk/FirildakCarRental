using FirildakAracKiralama.Models;
using System.ComponentModel.DataAnnotations;

public class Kiralama
{
    public int Id { get; set; }
    [Required] public int KullaniciId { get; set; }
    public Kullanici? Kullanici { get; set; }
    [Required] public int AracId { get; set; }
    public Arac? Arac { get; set; }
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public decimal ToplamUcret { get; set; }
    public DateTime? IadeTarihi { get; set; }
    public decimal? CezaUcreti { get; set; }
}
