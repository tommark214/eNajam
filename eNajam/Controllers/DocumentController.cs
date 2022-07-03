using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using eNajam.Models;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace eNajam.Controllers
{
    public class DocumentController : Controller
    {
        // GET: Document
        public ActionResult ContractList()
        {
            IList<DocumentViewModel> contractList = new List<DocumentViewModel>();            

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                var query = "SELECT * FROM dokumenti WHERE dokument ->> 'tip' ='UGV'";

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "dohvati_ugovore";
                cmd.Parameters.AddWithValue("@tip_dokumenta", NpgsqlTypes.NpgsqlDbType.Text, "UGV");
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlTypes.NpgsqlDbType.Integer, Global.USER_ID);


                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ContractJS contractJs = JsonConvert.DeserializeObject<ContractJS>(rdr["dokument"].ToString());

                        DocumentViewModel dvm = new DocumentViewModel();
                        dvm.Id = (int)rdr["id"];
                        dvm.Naziv = contractJs.Naziv;
                        dvm.Najmodavac = contractJs.Detalji.Najmodavac;
                        dvm.Najmoprimac = contractJs.Detalji.Najmoprimac;
                        dvm.Nekretnina = contractJs.Detalji.Nekretnina;
                        dvm.TrajanjeUgovora = contractJs.Detalji.TrajanjeUgovora;
                        dvm.DatumIsteka = contractJs.DatumIsteka;

                        contractList.Add(dvm);
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }
            return View(contractList);
        }

        public ActionResult ContractCreate()
        {
            DocumentViewModel dvm = new DocumentViewModel();
            dvm.Tip = "UGV";
            dvm.PopisNekretnina = new List<DetaljiNekretnine>();
            dvm.PopisKorisnika = new List<DetaljiKorisnika>();

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
                        dvm.PopisNekretnina.Add(new DetaljiNekretnine()
                        {
                            Id = (int)rdr["id"],
                            Naziv = estateJs.Naziv
                        });
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
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = connection;
                cmd.CommandText = "dohvati_korisnike";
                cmd.Parameters.AddWithValue("@najmodavac_id", NpgsqlTypes.NpgsqlDbType.Integer, Global.USER_ID);

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        UserJS userJs = JsonConvert.DeserializeObject<UserJS>(rdr["korisnik"].ToString());
                        //dvm.PopisNekretnina.Add(estateJs.Naziv);
                        dvm.PopisKorisnika.Add(new DetaljiKorisnika()
                        {
                            Id = (int)rdr["id"],
                            Naziv = userJs.ime + " " + userJs.prezime
                        });
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }

            return View(dvm);
        }

        [HttpPost]
        public ActionResult ContractCreate(DocumentViewModel dvm)
        {
            if (ModelState.IsValid)
            {
                //string dokumentJs = String.Empty;

                ContractJS newDocument = new ContractJS()
                {
                    Naziv = "Ugovor o najmu sa slobodno ugovorenom najamninom",
                    Tip = dvm.Tip,
                    DatumKreiranja = DateTime.Now.ToString("dd.MM.yyyy"),
                    DatumIsteka = dvm.DatumIsteka,
                    NajmodavacId = Global.USER_ID,
                    NajmoprimacId = dvm.KorisnikId,
                    Detalji = new ContractItemJs()
                    {
                       Najmodavac = dvm.Najmodavac,
                       Najmoprimac = dvm.Najmoprimac,
                       Nekretnina = dvm.Nekretnina,
                       DetaljiNekretnine = dvm.DetaljiNekretnine,
                       Članovi = dvm.Članovi,
                       DetaljiNajamnine = dvm.DetaljiNajamnine,
                       OpisRežijaITroškova = dvm.OpisRežijaITroškova,
                       TrajanjeUgovora = dvm.TrajanjeUgovora,
                       UvjetiRaskidaUgovora = dvm.UjetiRaskidaUgovora
                    }
                };

                var dokumentJs = JsonConvert.SerializeObject(newDocument, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dodaj_dokument";

                    //cmd.CommandText = "INSERT INTO dokumenti (dokument) values(@dokument);";                    
                    cmd.Parameters.AddWithValue("@dokument", NpgsqlTypes.NpgsqlDbType.Json, dokumentJs);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }

            return RedirectToAction("ContractList");
        }

        public ActionResult EstateDetails(int id)
        {
            StringBuilder detalji = new StringBuilder();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = connection;
                cmd.CommandText = "dohvati_nekretninu";
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlTypes.NpgsqlDbType.Integer, Global.USER_ID);
                cmd.Parameters.AddWithValue("@nekretnina_id", NpgsqlTypes.NpgsqlDbType.Integer, id);

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        EstateJS estateJs = JsonConvert.DeserializeObject<EstateJS>(rdr["nekretnina"].ToString());

                        detalji.Append("Najmodavac daje najmoprimcu u najam stan u " + estateJs.Lokacija + ".");
                        detalji.AppendLine();
                        detalji.Append("Stan se sastoji od: " + estateJs.Sobe + ".");
                        detalji.AppendLine();
                        detalji.Append("Stan je ukupne površine: " + estateJs.Povrsina + " m2.");
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }

            return Json(detalji.ToString(),JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContractEdit(int id)
        {



            return View();

        }

        [HttpPost]
        public ActionResult ContractEdit(DocumentViewModel dvm)
        {
            return View();

        }

        public ActionResult BillList()
        {
            IList<DocumentViewModel> billList = new List<DocumentViewModel>();            

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();
                
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;

                if (Global.TIP_KORISNIKA == "NAJMOPRIMAC")
                {
                    cmd.CommandText = "dohvati_rezije_najmoprimca";
                }
                else
                {
                    cmd.CommandText = "dohvati_rezije_nekretnine";                       
                }

                cmd.Parameters.AddWithValue("@tip_dokumenta", NpgsqlTypes.NpgsqlDbType.Text, "REZ");
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlTypes.NpgsqlDbType.Integer, Global.USER_ID);


                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {

                        DocumentViewModel dvm = new DocumentViewModel();

                        if(Global.TIP_KORISNIKA == "NAJMOPRIMAC")
                        {
                            BillJs billJs = JsonConvert.DeserializeObject<BillJs>(rdr["dokument"].ToString());

                            dvm.Id = (int)rdr["id"];
                            dvm.Naziv = billJs.Naziv;
                            dvm.Iznos = billJs.Detalji.Iznos.ToString();
                            dvm.DatumDospijeca = billJs.Detalji.DatumDospijeca;
                            dvm.StatusPlacanja = billJs.Detalji.StatusPlacanja;                            
                        }
                        else
                        {
                            dvm.Id = (int)rdr["dokument_id"];
                            dvm.Naziv = (string)rdr["naziv"];
                            dvm.Iznos = ((decimal)rdr["iznos"]).ToString();
                            dvm.DatumDospijeca = ((DateTime)rdr["datum_dospijeca"]).ToString("dd.MM.yyyy");
                            dvm.StatusPlacanja = (bool)rdr["status_placanja"];
                            dvm.Nekretnina = (string)rdr["naziv_nekretnine"];
                        }

                        billList.Add(dvm);
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }
            return View(billList);
        }

        public ActionResult BillCreate()
        {
            DocumentViewModel dvm = new DocumentViewModel();
            dvm.Tip = "REZ";

            return View(dvm);
        }

        [HttpPost]
        public ActionResult BillCreate(DocumentViewModel dvm)
        {
            if (ModelState.IsValid)
            {
                //string dokumentJs = String.Empty;

                BillJs newDocument = new BillJs()
                {
                    Naziv = dvm.Naziv,
                    Tip = dvm.Tip,
                    KorisnikId = Global.USER_ID,
                    NekretninaId = Global.NEKRETNINA_ID,
                    DatumKreiranja = DateTime.Now.ToString("dd.MM.yyyy"),
                    Detalji = new BillItemJs()
                    {
                        DatumDospijeca = dvm.DatumDospijeca,
                        DatumPlacanja = dvm.DatumPlacanja.ToString("dd.MM.yyyy"),
                        Iznos = decimal.Parse(dvm.Iznos),
                        StatusPlacanja = dvm.StatusPlacanja
                    }
                };

                var dokumentJs = JsonConvert.SerializeObject(newDocument, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.CommandText = "dodaj_dokument";
                    cmd.Parameters.AddWithValue("@dokument", NpgsqlTypes.NpgsqlDbType.Json, dokumentJs);                   
                    
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();                    
                    connection.Close();
                }
            }

            return RedirectToAction("BillList");
        }

        public ActionResult BillEdit(int id)
        {
            DocumentViewModel dvm = new DocumentViewModel();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dohvati_dokument";
                cmd.Parameters.AddWithValue("@dokument_id", NpgsqlTypes.NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("@tip", NpgsqlTypes.NpgsqlDbType.Text, "REZ");

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        BillJs billJs = JsonConvert.DeserializeObject<BillJs>(rdr["dokument"].ToString());

                        dvm.Id = (int)rdr["id"];
                        dvm.Naziv = billJs.Naziv;
                        dvm.Iznos = billJs.Detalji.Iznos.ToString();
                        dvm.DatumDospijeca = billJs.Detalji.DatumDospijeca;
                        dvm.DatumPlacanja = DateTime.ParseExact(billJs.Detalji.DatumPlacanja,"dd.MM.yyyy",null);
                        dvm.StatusPlacanja = billJs.Detalji.StatusPlacanja;
                        dvm.KorisnikId = billJs.KorisnikId;
                        dvm.NekretninaId = billJs.NekretninaId;
                        dvm.Tip = billJs.Tip;                       
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }

            return View(dvm);
        }

        [HttpPost]
        public ActionResult BillEdit(DocumentViewModel dvm)
        {
            var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();

            if (ModelState.IsValid)
            {
                BillJs billEdit = new BillJs()
                {
                    Naziv = dvm.Naziv,
                    Tip = dvm.Tip,
                    DatumKreiranja = DateTime.Now.ToString("dd.MM.yyyy"),
                    KorisnikId = dvm.KorisnikId,
                    NekretninaId = dvm.NekretninaId,
                    Detalji = new BillItemJs()
                    {
                        DatumDospijeca = dvm.DatumDospijeca,
                        DatumPlacanja = dvm.DatumPlacanja.ToString("dd.MM.yyyy"),
                        Iznos = decimal.Parse(dvm.Iznos),
                        StatusPlacanja = dvm.StatusPlacanja
                    }
                };

                var dokumentJs = JsonConvert.SerializeObject(billEdit, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.CommandText = "azuriraj_dokument";
                    cmd.Parameters.AddWithValue("@dokument_id", NpgsqlTypes.NpgsqlDbType.Integer, dvm.Id);
                    cmd.Parameters.AddWithValue("@dokument", NpgsqlTypes.NpgsqlDbType.Json, dokumentJs);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }

            return RedirectToAction("BillList");
        }

        public ActionResult NoteList()
        {
            IList<NoteViewModel> noteList = new List<NoteViewModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;

                if (Global.TIP_KORISNIKA == "NAJMOPRIMAC")
                {
                    cmd.CommandText = "dohvati_obavijesti_najmoprimca";
                }
                else
                {
                    cmd.CommandText = "dohvati_obavijesti_nekretnine";
                }
                
                cmd.Parameters.AddWithValue("@korisnik_id", NpgsqlTypes.NpgsqlDbType.Integer, Global.USER_ID);


                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {

                        NoteViewModel nvm = new NoteViewModel();

                        if (Global.TIP_KORISNIKA == "NAJMOPRIMAC")
                        {
                            NoteJS noteJs = JsonConvert.DeserializeObject<NoteJS>(rdr["obavijest"].ToString());

                            nvm.Id = (int)rdr["id"];
                            nvm.Naziv = noteJs.Naziv;
                            nvm.Opis = noteJs.Opis;
                            nvm.DatumDogadaja = noteJs.DatumDogadaja;                          
                        }
                        else
                        {
                            nvm.Id = (int)rdr["id"];
                            nvm.Naziv = (string)rdr["naziv"];
                            nvm.Opis = (string)rdr["opis"];
                            nvm.DatumDogadaja = ((DateTime)rdr["datum"]).ToString("dd.MM.yyyy");
                            nvm.Nekretnina = (string)rdr["nekretnina"];
                        }

                        noteList.Add(nvm);
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }
            return View(noteList);
        }

        public ActionResult NoteCreate()
        {
            NoteViewModel nvm = new NoteViewModel();   

            return View(nvm);
        }

        [HttpPost]
        public ActionResult NoteCreate(NoteViewModel nvm)
        {
            if (ModelState.IsValid)
            {
                NoteJS newNote = new NoteJS()
                {
                    Naziv = nvm.Naziv,
                    Opis = nvm.Opis,
                    DatumDogadaja = nvm.DatumDogadaja,
                    KorisnikId = (int)Session["korisnik_id"],
                    NekretninaId = (int)Session["nekretnina_id"]
                };

                var noteJs = JsonConvert.SerializeObject(newNote, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.CommandText = "dodaj_obavijest";
                    cmd.Parameters.AddWithValue("@obavijest", NpgsqlTypes.NpgsqlDbType.Json, noteJs);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }

            return RedirectToAction("NoteList");
        }

        public ActionResult NoteEdit(int id)
        {
            NoteViewModel nvm = new NoteViewModel();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = Global.CONNECTION_STRING;
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dohvati_obavijest";
                cmd.Parameters.AddWithValue("@obavijest_id", NpgsqlTypes.NpgsqlDbType.Integer, id);                

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        NoteJS noteJs = JsonConvert.DeserializeObject<NoteJS>(rdr["obavijest"].ToString());

                        nvm.Id = (int)rdr["id"];
                        nvm.Naziv = noteJs.Naziv;
                        nvm.Opis = noteJs.Opis;
                        nvm.DatumDogadaja = noteJs.DatumDogadaja;
                        nvm.KorisnikId = noteJs.KorisnikId;
                        nvm.NekretninaId = noteJs.NekretninaId;
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }

            return View(nvm);
        }

        [HttpPost]
        public ActionResult NoteEdit(NoteViewModel nvm)
        {
            if (ModelState.IsValid)
            {
                NoteJS newNote = new NoteJS()
                {
                    Naziv = nvm.Naziv,
                    Opis = nvm.Opis,
                    DatumDogadaja = nvm.DatumDogadaja,
                    KorisnikId = nvm.KorisnikId,
                    NekretninaId = nvm.NekretninaId
                };

                var noteJs = JsonConvert.SerializeObject(newNote, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.CommandText = "azuriraj_obavijest";
                    cmd.Parameters.AddWithValue("@obavijest_id", NpgsqlTypes.NpgsqlDbType.Text, nvm.Id);
                    cmd.Parameters.AddWithValue("@obavijest", NpgsqlTypes.NpgsqlDbType.Json, noteJs);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }
            return RedirectToAction("NoteList");
        }
    }
}