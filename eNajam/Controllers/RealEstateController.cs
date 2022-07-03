using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Npgsql;
using eNajam.Models;
using Newtonsoft.Json;
using NpgsqlTypes;

namespace eNajam.Controllers
{
    public class RealEstateController : Controller
    {
        // GET: RealEstate
        public ActionResult RealEstateList()
        {
            IList<RealEstateViewModel> realEstateList = new List<RealEstateViewModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "dohvati_nekretnine";
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlDbType.Integer, Global.USER_ID);

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        EstateJS estateJs = JsonConvert.DeserializeObject<EstateJS>(rdr["nekretnina"].ToString());

                        RealEstateViewModel revm = new RealEstateViewModel();
                        revm.Id = (int)rdr["id"];
                        revm.Naziv = estateJs.Naziv;
                        revm.Lokacija = estateJs.Lokacija;

                        realEstateList.Add(revm);
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }


            return View(realEstateList);
        }

        public ActionResult RealEstateCreate()
        {
            RealEstateViewModel revm = new RealEstateViewModel();

            return View(revm);
        }

        [HttpPost]
        public ActionResult RealEstateCreate(RealEstateViewModel revm)
        {
            if (ModelState.IsValid)
            {
                EstateJS newEstate = new EstateJS()
                {
                    Naziv = revm.Naziv,
                    Lokacija = revm.Lokacija,
                    Povrsina = revm.Povrsina,
                    Sobe = revm.Sobe,
                    DatumKreiranja = DateTime.Now.ToString("dd.MM.yyyy"),
                    Ostalo = revm.Ostalo,
                    VlasnikId = Global.USER_ID
                };

                var estateJs = JsonConvert.SerializeObject(newEstate, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();                  

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dodaj_nekretninu";
                    cmd.Parameters.AddWithValue("@nekretnina", NpgsqlDbType.Json, estateJs);                   

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }

            return RedirectToAction("RealEstateList","RealEstate");
        }

        public ActionResult RealEstateEdit(int id)
        {
            RealEstateViewModel rvm = new RealEstateViewModel();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = connection;
                cmd.CommandText = "dohvati_nekretninu";
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlDbType.Integer, Global.USER_ID);
                cmd.Parameters.AddWithValue("@nekretnina_id", NpgsqlDbType.Integer, id);

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        EstateJS estateJs = JsonConvert.DeserializeObject<EstateJS>(rdr["nekretnina"].ToString());

                        rvm.Id = (int)rdr["id"];
                        rvm.Naziv = estateJs.Naziv;
                        rvm.Lokacija = estateJs.Lokacija;
                        rvm.Povrsina = estateJs.Povrsina;
                        rvm.Ostalo = estateJs.Ostalo;
                        rvm.VlasnikId = estateJs.VlasnikId;
                        rvm.DatumKreiranja = estateJs.DatumKreiranja;
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }

            return View(rvm);
        }

        [HttpPost]
        public ActionResult RealEstateEdit(RealEstateViewModel revm)
        {
            if (ModelState.IsValid)
            {
                EstateJS estateEdit = new EstateJS()
                {
                    Naziv = revm.Naziv,                    
                    DatumKreiranja = revm.DatumKreiranja,
                    Lokacija = revm.Lokacija,
                    Sobe = revm.Sobe,
                    Povrsina = revm.Povrsina,
                    Ostalo = revm.Ostalo,
                    VlasnikId = revm.VlasnikId
                };

                var estateJs = JsonConvert.SerializeObject(estateEdit, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();                  

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "azuriraj_nekretninu";
                    cmd.Parameters.AddWithValue("@nekretnina_id", NpgsqlDbType.Integer, revm.Id);
                    cmd.Parameters.AddWithValue("@nekretnina", NpgsqlTypes.NpgsqlDbType.Json, estateJs);                    

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }

            return RedirectToAction("RealEstateList");
        }
    }
}