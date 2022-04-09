namespace Twitch_Spediteur.Fenster
{
    public class Ware
    {
        public string Bezeichnung { get; private set; } = "";
        public Verladung Ladung { get; private set; }
        public Einheit BasisEinheit { get; private set; }
        public Merkmal TransportMerkmal { get; private set; }
        public decimal Preis { get; private set; }

        public enum Verladung
        {
            Tank,
            Container,
            Pritsche,
            Kühl,
            Tief,
            Fahrzeug,
            Tier,
            Silo,
            Offen
        }

        public enum Einheit
        {
            Liter,
            Palette,
            Kilo,
            Tonne,
            Ster,
            Stück,
            Kubik
        }

        public enum Merkmal
        {
            Normal,
            Lebend,
            Gefahr,
            Eilig
        }

        public Ware(string bezeich, Verladung ladung, decimal preis, Einheit einheit, Merkmal merk)
        {
            Bezeichnung = bezeich;
            Ladung = ladung;
            Preis = preis;
            BasisEinheit = einheit;
            TransportMerkmal = merk;
        }
    }
}