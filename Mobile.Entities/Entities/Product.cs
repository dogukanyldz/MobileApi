using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Entities.Entities
{
   public class Product
    {
        public Product()
        {
            OlusturmaTarihi = DateTime.Now;
        }
       
        public int Id { get; set; }
        public int DepoId { get; set; }

        public string Marka { get; set; }
        public string Model { get; set; }
        public string Isim { get; set; }

        public string AnaKategori { get; set; }
        public string UrunKodu { get; set; }
        public string Durum { get; set; }

        public string AltKategori { get; set; }
        public string Barkod { get; set; }
        public string Birim { get; set; }
        public decimal Kdv { get; set; }
        public decimal BirimFiyatı { get; set; }
        public string BirimFiyatiParaBirimi { get; set; }

        public decimal BirimMaliyeti { get; set; }

        public string BirimMaliyetiParaBirimi { get; set; }
        public string Detaylar { get; set; }
        public string Etiketler { get; set; }
        public decimal Stok { get; set; }
        public decimal RezervStok { get; set; }
        public decimal FinalStok { get; set; }
        public decimal TedarikSiparisStok { get; set; }
        public string Favori { get; set; }

        public string YetkiSahibi { get; set; }
        public string Olusturan { get; set; }

        public DateTime OlusturmaTarihi { get; set; }
        public string Guncelleyen { get; set; }
        public DateTime GuncellemeTarihi { get; set; }

        public DateTime? SonIslem { get; set; }


    }
}
