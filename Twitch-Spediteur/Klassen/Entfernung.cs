namespace Twitch_Spediteur.Fenster
{
    public class Entfernung
    {
        public string Abholort { get; private set; }
        public string Lieferort { get; private set; }
        public string Route { get; private set; }
        public int Distanz { get; private set; }

        public Entfernung(string start, string ziel, string rou, int dist)
        {
            Abholort = start;
            Lieferort = ziel;
            Route = rou;
            Distanz = dist;
        }
    }
}