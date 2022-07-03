using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNajam
{
    public class BillJs
    {
        [JsonProperty("naziv")]
        public string Naziv { get; set; }

        [JsonProperty("tip")]
        public string Tip { get; set; }

        [JsonProperty("datum_kreiranja")]
        public string DatumKreiranja { get; set; }

        [JsonProperty("datum_isteka", NullValueHandling = NullValueHandling.Ignore)]
        public string DatumIsteka { get; set; }

        [JsonProperty("korisnik_id")]
        public int KorisnikId { get; set; }

        [JsonProperty("nekretnina_id")]
        public int NekretninaId { get; set; }

        [JsonProperty("detalji")]
        public BillItemJs Detalji { get; set; }
    }

    public class BillItemJs
    {
        [JsonProperty("oznaka")]
        public string Oznaka { get; set; }

        [JsonProperty("iznos")]
        public decimal Iznos { get; set; }

        [JsonProperty("datum_dospijeca", NullValueHandling = NullValueHandling.Ignore)]
        public string DatumDospijeca { get; set; }

        [JsonProperty("datum_placanja", NullValueHandling = NullValueHandling.Ignore)]
        public string DatumPlacanja { get; set; }

        [JsonProperty("status_placanja", NullValueHandling = NullValueHandling.Ignore)]
        public bool StatusPlacanja { get; set; }
    }
}