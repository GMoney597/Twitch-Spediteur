using System;

namespace Twitch_Spediteur.Fenster
{
    public class Fracht
    {
        public int ID { get; private set; }
        public string Abholort { get; private set; }
        public string Lieferort { get; private set; }
        public string Bezeichnung { get; private set; }
        public decimal Kapazitaet { get; private set; }
        public decimal Wert { get; private set; }
        public Status Ausfuehrung { get; private set; }
        public int FahrzeugID { get; private set; }

        public enum Status { 
            offen,
            aktiv,
            erledigt
        }

        public Fracht(string abhol, string liefer, string bez, decimal kap, decimal wert)
        {
            Abholort = abhol;
            Lieferort = liefer; 
            Bezeichnung = bez;
            Kapazitaet = kap;
            Wert = wert;
            Ausfuehrung = Status.offen;
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