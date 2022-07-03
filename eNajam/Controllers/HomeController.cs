using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Npgsql;
using Newtonsoft.Json;
using eNajam.Models;

namespace eNajam.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            var dgdd = Session["email"];
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            UserViewModel uvm = new UserViewModel();

            return View(uvm);
        }

        [HttpPost]
        public ActionResult Login(UserViewModel uvm)
        {
            if (ModelState.IsValid)
            {
                UserJS userJs = null;

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Global.CONNECTION_STRING;
                    connection.Open();

                    //var query = "SELECT * FROM korisnici WHERE korisnik ->> 'email'='" + uvm.Email + "' and korisnik ->> 'lozinka'='" + uvm.Lozinka + "';";

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "prijavi_korisnika";
                    cmd.Parameters.AddWithValue("@email", NpgsqlTypes.NpgsqlDbType.Text, uvm.Email);
                    cmd.Parameters.AddWithValue("@lozinka", NpgsqlTypes.NpgsqlDbType.Text, uvm.Lozinka);

                     using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                     {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                userJs = JsonConvert.DeserializeObject<UserJS>(rdr["korisnik"].ToString());
                                Global.USER_ID = (int)rdr["id"];
                            }

                            Session["FullName"] = userJs.ime;
                            Session["Email"] = userJs.email;
                            Session["korisnik_id"] = Global.USER_ID;
                            Session["nekretnina_id"] = userJs.NekretninaId;
                            Global.NEKRETNINA_ID = userJs.NekretninaId;
                            Global.TIP_KORISNIKA = userJs.tipKorisnika;                            

                            cmd.Dispose();
                            connection.Close();

                            return RedirectToAction("BillList", "Document");
                        }
                        else
                        {
                            RedirectToAction("Login", "Home");
                        }
                         

                         cmd.Dispose();
                     }
                    connection.Close();
                }

                //if()


                /*var f_password = GetMD5(password);
                var data = _db.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().idUser;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }*/
            }

            return RedirectToAction("BillList","Document");
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            return RedirectToAction("Login");
        }
    }
}