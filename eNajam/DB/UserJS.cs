using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNajam
{
    public class UserJS
    {
        [JsonProperty("ime")]
        public string ime { get; set; }

        [JsonProperty("prezime")]
        public string prezime { get; set; }

        [JsonProperty("lozinka", NullValueHandling = NullValueHandling.Ignore)]
        public string lozinka { get; set; }

        [JsonProperty("OIB")]
        public string OIB { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string email { get; set; }

        [JsonProperty("mobitel")]
        public string mobitel { get; set; }

        [JsonProperty("adresa")]
        public string adresa { get; set; }

        [JsonProperty("tip_korisnika")]
        public string tipKorisnika { get; set; }

        [JsonProperty("nekretnina_id")]
        public int NekretninaId { get; set; }

        [JsonProperty("najmodavac_id")]
        public int NajmodavacId { get; set; }
    }
}