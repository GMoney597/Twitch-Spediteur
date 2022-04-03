using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Spediteur.Klassen
{
    internal class Fahrzeug
    {
        public string Typ { get; private set; }
        public decimal Zuladung { get; private set; }
        public decimal Tankvolumen { get; private set; }
        public decimal Preis { get; private set; }

        Fahrzeug(string type, decimal load, decimal tank, decimal price)
        {
            Typ = type;
            Zuladung = load;
            Tankvolumen = tank;
            Preis = price;
        }
    }
}
