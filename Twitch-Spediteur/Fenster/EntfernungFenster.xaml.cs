using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur.Fenster
{
    /// <summary>
    /// Interaktionslogik für Entfernungen.xaml
    /// </summary>
    public partial class EntfernungFenster : Window
    {
        List<Entfernung> distances = new List<Entfernung>();
        List<Ort> orte = new List<Ort>();
        List<string> routen = new List<string>();
        List<Ort> fehlendeRouten = new List<Ort>();
        DispatcherTimer abfrageTimer = new DispatcherTimer();
        static int verbindungsZaehler = 0;
        int fehlenderOrt = 0;
        int vorhandenerOrt = 0;

        SQLite sql = new SQLite();
        string apikey = "";

        public EntfernungFenster()
        {
            InitializeComponent();
            InitializeRoutenUndOrte();

            apikey = holeApiKey();
        }

        private void InitializeRoutenUndOrte()
        {
            sql.HoleRouten(distances);
            sql.HoleOrte(orte);

            dtgEntfernungen.ItemsSource = distances;
        }

        private void cmdImport_Click(object sender, RoutedEventArgs e)
        {
            distances = new List<Entfernung>();

            FileStream file = new FileStream(@".\ExterneDateien\Entfernungen.csv", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file, Encoding.UTF7);

            string[] staedte = sr.ReadLine().Split(';');

            while (!sr.EndOfStream)
            {
                string route = "";
                string[] zeile = sr.ReadLine().Split(';');

                for (int i = 1; i < staedte.Length; i++)
                {
                    // Nullentfernungen übergehen
                    if (!String.IsNullOrEmpty(zeile[i]))
                    {
                        route = zeile[0] + "-" + staedte[i];

                        distances.Add(new Entfernung(zeile[0], staedte[i], route, Convert.ToInt32(zeile[i])));
                        sql.SpeichereEntfernung(route, Convert.ToInt32(zeile[i]));
                    }
                }
            }

            dtgEntfernungen.ItemsSource = null;
            dtgEntfernungen.Items.Clear();
            dtgEntfernungen.ItemsSource = distances;
        }

        private void cmdRouteCheck_Click(object sender, RoutedEventArgs e)
        {
            int zaehleRoute = 0;

            foreach (Ort ort in orte)
            {
                foreach (Entfernung entfernung in distances)
                {
                    if (entfernung.Abholort == ort.Ortsname || entfernung.Lieferort == ort.Ortsname)
                    {
                        zaehleRoute++;
                    }
                }

                if (zaehleRoute == 0)
                {
                    fehlendeRouten.Add(new Ort(ort.ID, ort.Ortsname));
                }

                string route = zaehleRoute + " Routen(mit " + ort.Ortsname + ")vorhanden";
                routen.Add(route);
                zaehleRoute = 0;
            }

            fehlenderOrt = fehlendeRouten.Count;
            vorhandenerOrt = orte.Count;

            //MessageBox.Show("Orte ohne Verbindungen: " + fehlendeRouten.Count, "Fehlende Stadtverbingen", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            lstNachricht.ItemsSource = routen;

            if (fehlendeRouten.Count != 0)
            {
                cmdErstelleRouten.IsEnabled = true;
            }
        }

        private void cmdErstelleRouten_Click(object sender, RoutedEventArgs e)
        {
            abfrageTimer.Interval = new TimeSpan(0, 0, 5);
            abfrageTimer.Tick += AbfrageTimer_Tick;
            abfrageTimer.Start();
        }

        private void AbfrageTimer_Tick(object? sender, EventArgs e)
        {
            ErstelleNaechsteRoute();
        }

        private void ErstelleNaechsteRoute()
        {
            if (vorhandenerOrt == 0)
            {
                vorhandenerOrt = orte.Count;
                fehlenderOrt--;
            }

            if (fehlenderOrt == 0)
            {
                abfrageTimer.Stop();
                return;
            }

            string startOrt = fehlendeRouten[fehlenderOrt - 1].Ortsname;
            string zielOrt = orte[vorhandenerOrt - 1].Ortsname;
            string abfrageUri = "";

            if (startOrt != zielOrt)
            {
                verbindungsZaehler++;

                abfrageUri = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=" + startOrt + "&wp.1=" + zielOrt + "&key=" + apikey;

                var client = new HttpClient();
                var anfrage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(abfrageUri)
                };

                string[] ergebnis = client.GetStringAsync(abfrageUri).Result.ToString().Split(',');
                List<string> list = ergebnis.ToList();
                var final = list.FindLast(trav => trav.Contains("travelDistance"));
                int entfernung = Convert.ToInt32(Convert.ToDouble(final.ToString().Split(':')[1]) / 1000);

                string hin = startOrt + "-" + zielOrt;
                string her = zielOrt + "-" + startOrt;

                sql.SpeichereEntfernung(hin, entfernung);
                sql.SpeichereEntfernung(her, entfernung);

                //MessageBox.Show("Verbindung-#: " + verbindungsZaehler + "\n" + startOrt + " ==> " + zielOrt + ": " + entfernung.ToString() + " km", "Abfrageergebnis erhalten",
                //    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            vorhandenerOrt--;
        }

        private string holeApiKey()
        {
            string key = "";

            using FileStream file = new FileStream(@"d:\projekte\api-keys.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file))
            {
                key = sr.ReadLine().ToString().Split(';')[1];
            }

            return key;
        }
    }
}
