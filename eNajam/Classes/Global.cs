using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace eNajam
{
    public class Global
    {
        static int _userId;

        public static int USER_ID
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }

        static int _nekretninaId;
        public static int NEKRETNINA_ID
        {
            get
            {
                return _nekretninaId;
            }
            set
            {
                _nekretninaId = value;
            }
        }

        static string _tipKorisnika;
        public static string TIP_KORISNIKA
        {
            get
            {
                return _tipKorisnika;
            }
            set
            {
                _tipKorisnika = value;
            }
        }

        static string _connectionString;
        public static string CONNECTION_STRING
        {
            get => WebConfigurationManager.ConnectionStrings["eNajamConnection"].ConnectionString;
            set => _connectionString = value;            
        }
    }
}