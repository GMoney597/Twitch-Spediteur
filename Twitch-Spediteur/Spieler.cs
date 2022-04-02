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
        private string Passwort_neu;


        public Spieler(string Name, string Mail, string Passwort)
        {
            // byte[] pwValue = sHA256.ComputeHash(Encoding.UTF8.GetBytes(Passwort));
            Spielername = Name;
            this.Mail = Mail;
            Passwort_neu = Passwort;
            // Passwort_alt = "";
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
            throw new System.NotImplementedException();
        }

        public bool Einloggen()
        {
            throw new System.NotImplementedException();
        }

        public bool Nachricht_schreiben()
        {
            throw new System.NotImplementedException();
        }

        internal string HoleRegistrierdaten()
        {
            // gib mit gehashtem Passwort zurück
            // return Spielername + ";" + Mail + ";" + Encoding.Default.GetString(Passwort_neu);
            return Spielername + ";" + Mail + ";" + Passwort_neu;
        }

        internal bool PruefePasswort(string passwort)
        {
            if (Passwort_neu == passwort)
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
