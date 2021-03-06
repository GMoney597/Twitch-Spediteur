using System;

namespace Twitch_Spediteur.Fenster
{
    public class Ware
    {
        public string Bezeichnung { get; private set; } = "";
        public Verladung Ladung { get; private set; }
        public Einheit BasisEinheit { get; private set; }
        public Merkmal TransportMerkmal { get; private set; }
        public decimal Preis { get; private set; }
        
        [Flags]
        public enum Verladung
        {
            Tank = 1,
            Container = 2,
            Pritsche = 4,
            Kühl = 8,
            Tief = 16,
            Fahrzeug = 32,
            Tier = 64,
            Silo = 128,
            Offen = 256
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