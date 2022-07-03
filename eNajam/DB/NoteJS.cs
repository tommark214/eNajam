using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNajam
{
    public class NoteJS
    {
        [JsonProperty("naziv")]
        public string Naziv { get; set; }

        [JsonProperty("opis")]
        public string Opis { get; set; }

        [JsonProperty("datum_dogadaja")]
        public string DatumDogadaja { get; set; }

        [JsonProperty("korisnik_id")]
        public int KorisnikId { get; set; }

        [JsonProperty("nekretnina_id")]
        public int NekretninaId { get; set; }
    }
}