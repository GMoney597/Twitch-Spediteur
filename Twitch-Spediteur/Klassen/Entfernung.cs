namespace Twitch_Spediteur.Fenster
{
    public class Entfernung
    {
        public string Start { get; private set; }
        public string Ziel { get; private set; }
        public int Distanz { get; private set; }

        public Entfernung(string start, string ziel, int dist)
        {
            Start = start;
            Ziel = ziel;
            Distanz = dist;
        }
    }
}