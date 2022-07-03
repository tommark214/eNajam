using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eNajam
{
    public class EstateJS
    {
        [JsonProperty("naziv")]
        public string Naziv { get; set; }        

        [JsonProperty("datum_kreiranja")]
        public string DatumKreiranja { get; set; }

        [JsonProperty("lokacija", NullValueHandling = NullValueHandling.Ignore)]
        public string Lokacija { get; set; }

        [JsonProperty("povrsina", NullValueHandling = NullValueHandling.Ignore)]
        public string Povrsina { get; set; }

        [JsonProperty("sobe", NullValueHandling = NullValueHandling.Ignore)]
        public string Sobe { get; set; }

        [JsonProperty("ostalo")]
        public string Ostalo { get; set; }

        [JsonProperty("vlasnik_id")]
        public int VlasnikId { get; set; }
    }
}