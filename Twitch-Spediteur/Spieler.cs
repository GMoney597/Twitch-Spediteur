using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Spediteur
{
    internal class Spieler
    {
        //public string Spielername;
        private string Mail;
        private string Passwort_alt;
        private string Passwort_neu;


        public Spieler(string Name, string Mail, string Passwort)
        {
            Spielername = Name;
            this.Mail = Mail;
            Passwort_neu = Passwort;
            Passwort_alt = "";
        }

        public string Spielername { get; private set; }

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
    }
}
