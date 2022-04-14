using System;

namespace Twitch_Spediteur.Fenster
{
    public class Fracht
    {
        public int ID { get; private set; }
        public string Abholort { get; private set; }
        public string Lieferort { get; private set; }
        public int Entfernung { get; private set; }
        public string Bezeichnung { get; private set; }
        public decimal Kapazitaet { get; private set; }
        public string Basiseinheit { get; private set; }
        public decimal Wert { get; private set; }
        public Status Ausfuehrung { get; private set; }
        public int FahrzeugID { get; private set; }

        public enum Status { 
            Offen,
            Abholung,
            Beladung,
            Zustellung,
            Entladung,
            Erledigt
        }

        // Aufbau für Fracht-Börse
        public Fracht(string abhol, string liefer, int entf, string bez, string einheit, decimal kap, decimal wert)
        {
            Abholort = abhol;
            Lieferort = liefer; 
            Entfernung = entf;
            Bezeichnung = bez;
            Kapazitaet = kap;
            Basiseinheit = einheit;
            Wert = wert;
            Ausfuehrung = Status.Offen;
        }

        // SQL-Konstruktor: ID, Startort, Zielort, Bezeichnung, Menge, Wert, Status, SpielerID, FahrzeugID
        public Fracht(int id, string abhol, string liefer, string bez, decimal kap, decimal wert, Status aus, int fahr)
        {
            ID = id;
            Abholort = abhol;
            Lieferort = liefer;
            Bezeichnung = bez;
            Kapazitaet = kap;
            Wert = wert;
            Ausfuehrung = aus;            
            FahrzeugID = fahr;
        }
    }
}