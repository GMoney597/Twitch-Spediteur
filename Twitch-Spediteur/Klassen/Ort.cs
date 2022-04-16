using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Spediteur.Klassen
{
    internal class Ort
    {
        public string Ortsname { get; private set; }

        public Ort(string ort) 
        {
            Ortsname = ort;
        }
    }
}
