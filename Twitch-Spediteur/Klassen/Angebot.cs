using System;
using System.Collections.Generic;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur.Fenster
{
    public class Angebot
    {
        public int ID { get; private set; }
        public string Abholort { get; private set; }
        public string Lieferort { get; private set; }
        public int Entfernung { get; private set; }
        public string Bezeichnung { get; private set; }
        public decimal Kapazitaet { get; private set; }
        public string Basiseinheit { get; private set; }
        public decimal Wert { get; private set; }

        // Aufbau für Fracht-Börse
        public Angebot(string abhol, string liefer, int entf, string bez, string einheit, decimal kap, decimal wert)
        {
            Abholort = abhol;
            Lieferort = liefer; 
            Entfernung = entf;
            Bezeichnung = bez;
            Kapazitaet = kap;
            Basiseinheit = einheit;
            Wert = wert;
        }

        // SQL-Konstruktor: ID, Startort, Zielort, Bezeichnung, Menge, Wert, Status, SpielerID, FahrzeugID
        public Angebot(int id, string abhol, string liefer, string bez, decimal kap, decimal wert)
        {
            ID = id;
            Abholort = abhol;
            Lieferort = liefer;
            Bezeichnung = bez;
            Kapazitaet = kap;
            Wert = wert;
        }
    }
}