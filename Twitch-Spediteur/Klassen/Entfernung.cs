namespace Twitch_Spediteur.Fenster
{
    internal class Entfernung
    {
        internal string Start { get; private set; }
        internal string Ziel { get; private set; }
        internal int Distanz { get; private set; }

        internal Entfernung(string start, string ziel, int dist)
        {
            Start = start;
            Ziel = ziel;
            Distanz = dist;
        }
    }
}