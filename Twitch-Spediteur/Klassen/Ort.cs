using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Spediteur.Klassen
{
    public class Ort
    {
        public int ID { get; private set; }
        public string Ortsname { get; private set; }

        public Ort(int id, string ort) 
        {
            ID = id;
            Ortsname = ort;
        }
        
        public Ort(string ort)
        {
            Ortsname = ort;
        }
    }
}
