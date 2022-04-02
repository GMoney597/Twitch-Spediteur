using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Spediteur
{
    public class Spieler
    {
        SHA256 sHA256 = SHA256.Create();

        public string Spielername { get; private set; }
        public string Mail { get; private set; }   
        private byte[] Passwort_alt;
        public byte[] Passwort_neu { get; private set; }


        public Spieler(string Name, string Mail, string Passwort)
        {
            byte[] pwValue = sHA256.ComputeHash(Encoding.UTF8.GetBytes(Passwort));
            Spielername = Name;
            this.Mail = Mail;
            Passwort_neu = pwValue;
            // Passwort_alt = "";

            Bargeld = 1000.0M;
            Konto = 0.0M;
        }

        public Spieler(string Name)
        {
            Spielername = Name;
        }

        public decimal Bargeld
        {
            get => default;
            set
            {
            }
        }

        public decimal Konto
        {
            get => default;
            set
            {
            }
        }

        public int Fuhrpark
        {
            get => default;
            set
            {
            }
        }

        public int Nachrichten
        {
            get => default;
            set
            {
            }
        }

        public bool Registrieren()
        {
            SQLite sql = new SQLite();
            bool register = sql.RegistriereSpieler(this);
            return register;
        }

        public bool Einloggen(string name_mail, string passwort)
        {
            SQLite sql = new SQLite();
            bool login = sql.EinloggenSpieler(name_mail, passwort);
            return login;
        }

        public bool Nachricht_schreiben()
        {
            throw new System.NotImplementedException();
        }

        //internal string HoleRegistrierdaten()
        //{
        //    // gib mit gehashtem Passwort zurück
        //    // return Spielername + ";" + Mail + ";" + Encoding.Default.GetString(Passwort_neu);
        //    // return Spielername + ";" + Mail + ";" + Passwort_neu;
        //}

        internal bool PruefePasswort(string passwort)
        {
            byte[] pwValid = sHA256.ComputeHash(Encoding.UTF8.GetBytes(passwort));

            if (Passwort_neu == pwValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
