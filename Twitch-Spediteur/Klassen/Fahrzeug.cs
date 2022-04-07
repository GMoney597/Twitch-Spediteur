using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Spediteur.Klassen
{
    public class Fahrzeug
    {
        public string Typ { get; private set; }
        public decimal Zuladung { get; private set; }
        public decimal Tankvolumen { get; private set; }
        public decimal MietPreis { get; private set; }
        public decimal KaufPreis { get; private set; }
        public decimal VerkaufPreis { get; private set; }
        public DateTime AktionsDatum { get; private set; }
        public bool IsGekauft { get; private set; }

        public Fahrzeug(string type, decimal load, decimal tank, decimal rent, decimal buy)
        {
            Typ = type;
            Zuladung = load;
            Tankvolumen = tank;
            MietPreis = rent;
            KaufPreis = buy;
            VerkaufPreis = buy * 0.7m;
            AktionsDatum = DateTime.Now;
        }

        public decimal FahrzeugMieten()
        {
            AktionsDatum = DateTime.Now;
            IsGekauft = false;
            return MietPreis;
        }
    }
}
