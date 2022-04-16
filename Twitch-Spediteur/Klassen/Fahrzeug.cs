using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Spediteur.Klassen
{
    public class Fahrzeug
    {
        public int ID { get; private set; }
        public string Typ { get; private set; }
        public decimal Zuladung { get; private set; }
        public decimal Tankvolumen { get; private set; }
        public decimal MietPreis { get; private set; }
        public decimal KaufPreis { get; private set; }
        public decimal VerkaufPreis { get; private set; }
        public DateTime AktionsDatum { get; private set; }
        public DateTime AbgabeDatum { get; private set; }
        public bool IsGekauft { get; set; }
        public int VerladeSchlüssel { get; private set; }
        public bool HatAuftrag { get; set; }
        public int AuftragsNummer { get; private set; }
        public string Standort { get; set; }
        public Status Ladung { get; set; }

        public enum Status
        {
            leer,
            beladen,
            voll
        }

        public Fahrzeug(string type, decimal load, decimal tank, decimal rent, decimal buy, int flag)
        {
            Typ = type;
            Zuladung = load;
            Tankvolumen = tank;
            MietPreis = rent;
            KaufPreis = buy;
            VerkaufPreis = buy * 0.7m;
            AktionsDatum = DateTime.Now;
        }

        // Konstruktor für Fuhrparkabfrage SQL
        public Fahrzeug(int id, string type, decimal load, decimal tank, decimal rent, decimal buy, int flag, bool gekauft, 
            DateTime erwerb, DateTime abgabe, string stand, bool auftrag, int auftragnummer)
        {
            ID = id;
            Typ = type;
            Zuladung = load;
            Tankvolumen = tank;
            MietPreis = rent;
            KaufPreis = buy;
            VerkaufPreis = buy * 0.7m;
            VerladeSchlüssel = flag;
            IsGekauft = gekauft;
            AktionsDatum = erwerb;
            AbgabeDatum = abgabe;
            HatAuftrag = auftrag;
            AuftragsNummer = auftragnummer;
            Standort = stand;
        }

        /*
         * Kombi:       mieten 400      kaufen 40000
         * Transporter: mieten 900      kaufen 60000
         * Mini-Truck:  mieten 1350     kaufen 75000
         * LKW 7.5t:    mieten 2000     kaufen 120000
         * LKW 12t:     mieten 4500     kaufen 150000
         * Sattelzug:   mieten 15000    kaufen 300000
         */

        public void FahrzeugMieten()
        {
            AktionsDatum = DateTime.Now;
            IsGekauft = false;
        }

        internal void WurdeGekauft()
        {
            this.IsGekauft = true;
        }

        internal void GebeAuftrag(int auftrag)
        {
            HatAuftrag = true;
            AuftragsNummer = auftrag;
            Standort = "==> Abfahrt";
        }
    }
}
