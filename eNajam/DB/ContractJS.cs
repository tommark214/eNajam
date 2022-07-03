using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNajam
{
    public class ContractJS
    {
        [JsonProperty("naziv")]
        public string Naziv { get; set; }

        [JsonProperty("tip")]
        public string Tip { get; set; }

        [JsonProperty("datum_kreiranja")]
        public string DatumKreiranja { get; set; }

        [JsonProperty("datum_isteka", NullValueHandling = NullValueHandling.Ignore)]
        public string DatumIsteka { get; set; }

        [JsonProperty("najmodavac_id")]
        public int NajmodavacId { get; set; }

        [JsonProperty("najmoprimac_id")]
        public int NajmoprimacId { get; set; }

        [JsonProperty("detalji")]
        public ContractItemJs Detalji { get; set; }
    }

    public class ContractItemJs
    {

        [JsonProperty("najmodavac", NullValueHandling = NullValueHandling.Ignore)]
        public string Najmodavac { get; set; }

        [JsonProperty("najmoprimac", NullValueHandling = NullValueHandling.Ignore)]
        public string Najmoprimac { get; set; }

        [JsonProperty("nekretnina", NullValueHandling = NullValueHandling.Ignore)]
        public string Nekretnina { get; set; }

        [JsonProperty("detalji_nekretnine", NullValueHandling = NullValueHandling.Ignore)]
        public string DetaljiNekretnine { get; set; }

        [JsonProperty("clanovi", NullValueHandling = NullValueHandling.Ignore)]
        public string Članovi { get; set; }

        [JsonProperty("detalji_najamnine", NullValueHandling = NullValueHandling.Ignore)]
        public string DetaljiNajamnine { get; set; }

        [JsonProperty("opis_troskova", NullValueHandling = NullValueHandling.Ignore)]
        public string OpisRežijaITroškova { get; set; }

        [JsonProperty("trajanje_ugovora", NullValueHandling = NullValueHandling.Ignore)]
        public string TrajanjeUgovora { get; set; }

        [JsonProperty("uvjeti_raskida", NullValueHandling = NullValueHandling.Ignore)]
        public string UvjetiRaskidaUgovora { get; set; }
    }
}