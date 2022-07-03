using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eNajam.Models;
using Npgsql;
using Newtonsoft.Json;
using System.Data;

namespace eNajam.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult UserList()
        {
            List<UserViewModel> uvmList = new List<UserViewModel>();
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();


                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "dohvati_korisnike";
                cmd.Parameters.AddWithValue("@najmodavac_id", NpgsqlTypes.NpgsqlDbType.Integer, Global.USER_ID);

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {                        
                        UserJS userJs = JsonConvert.DeserializeObject<UserJS>(rdr["korisnik"].ToString());

                        UserViewModel uvm = new UserViewModel();
                        uvm.Id = (int)rdr["id"];
                        uvm.Ime = userJs.ime;
                        uvm.Prezime = userJs.prezime;
                        uvm.Email = userJs.email;
                        uvm.OIB = userJs.OIB;
                        uvm.Adresa = userJs.adresa;
                        uvm.TipKorisnika = userJs.tipKorisnika;
                        uvm.Mobitel = userJs.mobitel;
                        uvm.NajmodavacId = userJs.NajmodavacId;
                        uvm.NekretninaId = userJs.NekretninaId;

                        uvmList.Add(uvm);
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }           

            return View(uvmList);
        }

        public ActionResult UserCreate()
        {           
            UserViewModel uvm = new UserViewModel();
            uvm.PopisNekretnina = new List<DetaljiNekretnine>();
            uvm.NajmodavacId = Global.USER_ID;
            uvm.TipKorisnika = "NAJMOPRIMAC";

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = connection;
                cmd.CommandText = "dohvati_nekretnine";
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlTypes.NpgsqlDbType.Integer, Global.USER_ID);

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        EstateJS estateJs = JsonConvert.DeserializeObject<EstateJS>(rdr["nekretnina"].ToString());
                        //dvm.PopisNekretnina.Add(estateJs.Naziv);
                        uvm.PopisNekretnina.Add(new DetaljiNekretnine()
                        {
                            Id = (int)rdr["id"],
                            Naziv = estateJs.Naziv
                        });
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }

            return View(uvm);
        }

        [HttpPost]
        public ActionResult UserCreate(UserViewModel uvm)
        {
            if (ModelState.IsValid)
            {
                UserJS newUser = new UserJS()
                {
                    ime = uvm.Ime,
                    prezime = uvm.Prezime,
                    email = uvm.Email,
                    lozinka = uvm.Lozinka,
                    OIB = uvm.OIB,
                    mobitel = uvm.Mobitel,
                    adresa = uvm.Adresa,
                    tipKorisnika = uvm.TipKorisnika,
                    NajmodavacId = uvm.NajmodavacId,
                    NekretninaId = uvm.NekretninaId
                };

                var korisnikJs = JsonConvert.SerializeObject(newUser, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dodaj_korisnika";

                    //cmd.CommandText = "INSERT INTO dokumenti (dokument) values(@dokument);";                    
                    cmd.Parameters.AddWithValue("@korisnik", NpgsqlTypes.NpgsqlDbType.Json, korisnikJs);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }

            return RedirectToAction("UserList");
        }

        public ActionResult UserEdit(int id)
        {
            UserViewModel uvm = new UserViewModel();
            uvm.PopisNekretnina = new List<DetaljiNekretnine>();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dohvati_korisnika";
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlTypes.NpgsqlDbType.Integer, id);

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        UserJS userJs = JsonConvert.DeserializeObject<UserJS>(rdr["korisnik"].ToString());
                                               
                        uvm.Id = (int)rdr["id"];
                        uvm.Ime = userJs.ime;
                        uvm.Prezime = userJs.prezime;
                        uvm.Email = userJs.email;
                        uvm.OIB = userJs.OIB;
                        uvm.Adresa = userJs.adresa;
                        uvm.TipKorisnika = userJs.tipKorisnika;
                        uvm.Lozinka = userJs.lozinka;
                        uvm.Mobitel = userJs.mobitel;
                        uvm.NajmodavacId = userJs.NajmodavacId;
                        uvm.NekretninaId = userJs.NekretninaId;
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dohvati_nekretnine";
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlTypes.NpgsqlDbType.Integer, Global.USER_ID);

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        EstateJS estateJs = JsonConvert.DeserializeObject<EstateJS>(rdr["nekretnina"].ToString());
                        //dvm.PopisNekretnina.Add(estateJs.Naziv);
                        uvm.PopisNekretnina.Add(new DetaljiNekretnine()
                        {
                            Id = (int)rdr["id"],
                            Naziv = estateJs.Naziv
                        });
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }

            return View(uvm);
        }

        [HttpPost]
        public ActionResult UserEdit(UserViewModel uvm)
        {
            if (ModelState.IsValid)
            {
                UserJS newUser = new UserJS()
                {
                    ime = uvm.Ime,
                    prezime = uvm.Prezime,
                    email = uvm.Email,
                    lozinka = uvm.Lozinka,
                    OIB = uvm.OIB,
                    mobitel = uvm.Mobitel,
                    adresa = uvm.Adresa,
                    tipKorisnika = uvm.TipKorisnika,
                    NajmodavacId = uvm.NajmodavacId,
                    NekretninaId = uvm.NekretninaId
                };

                var korisnikJs = JsonConvert.SerializeObject(newUser, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "azuriraj_korisnika";
                    cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlTypes.NpgsqlDbType.Integer, uvm.Id);
                    cmd.Parameters.AddWithValue("@korisnik", NpgsqlTypes.NpgsqlDbType.Json, korisnikJs);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }

            return RedirectToAction("UserList");
        }
    }
}