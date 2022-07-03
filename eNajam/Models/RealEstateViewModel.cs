using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNajam.Models
{
    public class RealEstateViewModel
    {
        public int Id { get; set; }

        public string Naziv { get; set; }

        public string Lokacija { get; set; }

        public string Sobe { get; set; }

        public string Povrsina { get; set; }

        public string Ostalo { get; set; }

        public int VlasnikId { get; set; }

        public string DatumKreiranja { get; set; }
    }
}