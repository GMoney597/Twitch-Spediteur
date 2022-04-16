using System;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur.Fenster
{
    public class Auftrag
    {
        public int Auftragsnummer { get; private set; }
        public string Abholort { get; private set; }
        public string Lieferort { get; private set; }
        public int Entfernung { get; set; }
        public int Erfuellungsgrad { get; private set; }
        public string Bezeichnung { get; set; }
        public decimal Auftragssumme { get; private set; }
        public Status Zustand { get; set; }
        public int SpielerID { get; private set; }
        public int FahrzeugID { get; set; }
        public DateTime AuftragsDatum { get; private set; }
        public enum Status
        {
            Offen,
            Abholung,
            Beladung,
            Zustellung,
            Entladung,
            Erledigt
        }

        // Auftrag erstellt aus dem Angebotsfenster
        public Auftrag(Angebot ang)
        {
            Auftragsnummer = ang.ID;
            Abholort = ang.Abholort;
            Lieferort = ang.Lieferort;
            Entfernung = ang.Entfernung;
            Bezeichnung = ang.Bezeichnung;
            Auftragssumme = ang.Wert;
            Zustand = Status.Offen;
        }

        // Konstruktor für SQL-Abfrage mit Start-Datum (nicht DB-Null)
        public Auftrag(int auftragID, string abhol, string liefer, int entf, string bez, decimal wert,
            Status stat, int spID, int fzgID, DateTime start)
        {
            Auftragsnummer = auftragID;
            Abholort = abhol;
            Lieferort = liefer;
            Entfernung = entf;
            Bezeichnung = bez;
            Auftragssumme = wert;
            Zustand = stat;
            SpielerID = spID;
            FahrzeugID = fzgID;
            AuftragsDatum = start;
        }

        // Konstruktor für SQL-Abfrage ohne Start-Datum = DB-Null
        public Auftrag(int auftragID, string abhol, string liefer, int entf, string bez, decimal wert,
            Status stat, int spID, int fzgID)
        {
            Auftragsnummer = auftragID;
            Abholort = abhol;
            Lieferort = liefer;
            Entfernung = entf;
            Bezeichnung = bez;
            Auftragssumme = wert;
            Zustand = stat;
            SpielerID = spID;
            FahrzeugID = fzgID;
        }
    }
}