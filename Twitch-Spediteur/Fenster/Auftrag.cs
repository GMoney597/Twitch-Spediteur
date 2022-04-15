namespace Twitch_Spediteur.Fenster
{
    internal class Auftrag
    {
        public Auftrag(int iD, string abholort, string lieferort, string bezeichnung, Angebot.Status zustand, string typ, bool hatAuftrag)
        {
            ID = iD;
            Abholort = abholort;
            Lieferort = lieferort;
            Bezeichnung = bezeichnung;
            Zustand = zustand;
            Typ = typ;
            HatAuftrag = hatAuftrag;
        }

        public int ID { get; }
        public string Abholort { get; }
        public string Lieferort { get; }
        public string Bezeichnung { get; }
        public Angebot.Status Zustand { get; }
        public string Typ { get; }
        public bool HatAuftrag { get; }
    }
}