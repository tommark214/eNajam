using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eNajam.Models
{
    public class DocumentViewModel
    {
        public int Id { get; set; }

        public string Naziv { get; set; }

        public string Tip { get; set; }

        public string DatumKreiranja { get; set; }

        public string Oznaka { get; set; }

        public string Iznos { get; set; }

        public string DatumDospijeca { get; set; }

        public DateTime DatumPlacanja { get; set; }

        public bool StatusPlacanja { get; set; }

        public string Najmodavac { get; set; }

        public string Najmoprimac { get; set; }

        public string Nekretnina { get; set; }

        public string DetaljiNekretnine { get; set; }

        public string Članovi { get; set; }

        public string DetaljiNajamnine { get; set; }

        public string OpisRežijaITroškova { get; set; }

        public string TrajanjeUgovora { get; set; }

        public string UjetiRaskidaUgovora { get; set; }

        public string DatumIsteka { get; set; }

        public int KorisnikId { get; set; }

        public int NekretninaId { get; set; }

        public List<DetaljiNekretnine> PopisNekretnina { get; set; }

        public List<DetaljiKorisnika> PopisKorisnika { get; set; }

    }      
    
    public class DetaljiNekretnine
    {
        public int Id { get; set; }
        public string Naziv { get; set; }

        public string Detalji { get; set; }
    }

    public class DetaljiKorisnika
    {
        public int Id { get; set; }

        public string Naziv { get; set; }
    }
}