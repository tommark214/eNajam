using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNajam.Models
{
    public class NoteViewModel
    {
        public int Id { get; set; }

        public string Naziv { get; set; }

        public string DatumDogadaja { get; set; }

        public string Opis { get; set; }

        public int KorisnikId { get; set; }

        public int NekretninaId { get; set; }

        public string Nekretnina { get; set; }

    }
}