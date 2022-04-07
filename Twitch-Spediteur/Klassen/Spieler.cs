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
        public string Passwort { get; private set; }
        public decimal Bargeld { get; private set; }
        public decimal Konto { get; private set; }
        public int Fuhrpark { get; private set; }
        public int Nachrichten { get; private set; }
        public string Startort { get; private set; }


        public Spieler(string name, string mail, string passwort)
        {
            byte[] pwValue = sHA256.ComputeHash(Encoding.UTF8.GetBytes(passwort));
            string pwHash = Convert.ToBase64String(pwValue);
            Spielername = name;
            this.Mail = mail;
            this.Passwort = pwHash;
            // Passwort_alt = "";

            Bargeld = 1000.0M;
            Konto = 0.0M;
        }

        public Spieler(string Name, string Mail, decimal Bar, decimal Kontostand, int Fuhr, int Nach, string Ort)
        {
            Spielername = Name;
            this.Mail = Mail;
            Bargeld = Bar;
            Konto = Kontostand;
            Fuhrpark = Fuhr;
            Nachrichten = Nach;
            Startort = Ort;
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

        public void FahrzeugMieten(decimal mietpreis)
        {
            Bargeld -= mietpreis;
        }

        public bool Nachricht_schreiben()
        {
            throw new System.NotImplementedException();
        }

        internal void FahrzeugKaufen(decimal kaufpreis)
        {
            Bargeld -= kaufpreis;
        }
    }
}
