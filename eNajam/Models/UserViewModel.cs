using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNajam.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string OIB { get; set; }
        public string Mobitel { get; set; }
        public string Adresa { get; set; }    
        public string Lozinka { get; set; }       
        public string TipKorisnika { get; set; }

        public int NajmodavacId { get; set; }

        public int NekretninaId { get; set; }

        public List<DetaljiNekretnine> PopisNekretnina { get; set; }
    }
}