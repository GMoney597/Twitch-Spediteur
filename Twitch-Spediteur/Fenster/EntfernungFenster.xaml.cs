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
        List<Ort> fehlendeVerbindungen = new List<Ort>();
        DispatcherTimer abfrageTimer = new DispatcherTimer();
        static int verbindungsZaehler = 0;
        int moeglicheRouten = 0;
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
            distances.Clear();
            orte.Clear();
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
            lstNachricht.ItemsSource = null;
            fehlendeVerbindungen.Clear();
            routen.Clear();

            foreach (Ort ort in orte)
            {
                foreach (Entfernung entfernung in distances)
                {
                    if (entfernung.Abholort == ort.Ortsname || entfernung.Lieferort == ort.Ortsname)
                    {
                        zaehleRoute++;
                    }
                }

                if (zaehleRoute < moeglicheRouten)
                {
                    fehlendeVerbindungen.Add(new Ort(ort.ID, ort.Ortsname));
                }

                string route = zaehleRoute + " Routen(mit " + ort.Ortsname + ")vorhanden";
                routen.Add(route);
                moeglicheRouten = zaehleRoute;
                zaehleRoute = 0;
            }

            fehlenderOrt = fehlendeVerbindungen.Count;
            vorhandenerOrt = orte.Count;

            lstNachricht.ItemsSource = routen;

            if (fehlendeVerbindungen.Count != 0)
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
                abfrageTimer = null;
                MessageBox.Show("Alle fehlenden Routen wurden ergänzt.", "Routen aktualisiert", MessageBoxButton.OK, MessageBoxImage.Information);
                cmdRouteCheck_Click(this, null);
                return;
            }

            string startOrt = fehlendeVerbindungen[fehlenderOrt - 1].Ortsname;
            string zielOrt = orte[vorhandenerOrt - 1].Ortsname;
            string abfrageUri = "";

            if (startOrt != zielOrt)
            {
                verbindungsZaehler++;
                tbkImportStand.Text = "Aktuelle Route: " + verbindungsZaehler + " von " + (orte.Count() - 1) + " wird erstellt.";

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

                var vorhandeneRoute = distances.Find(rou => rou.Route == her || rou.Route == hin);

                if (vorhandeneRoute == null)
                {
                    distances.Add(new Entfernung(startOrt, zielOrt, hin, entfernung));
                    distances.Add(new Entfernung(zielOrt, startOrt, her, entfernung));
                    sql.SpeichereEntfernung(hin, entfernung);
                    sql.SpeichereEntfernung(her, entfernung);
                }

                //MessageBox.Show("Verbindung-#: " + verbindungsZaehler + "\n" + startOrt + " ==> " + zielOrt + ": " + entfernung.ToString() + " km", "Abfrageergebnis erhalten",
                //    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            vorhandenerOrt--;
        }

        private string holeApiKey()
        {
            string key = "";

            using FileStream file = new FileStream(Properties.Settings.Default["API_Key"].ToString(), FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file))
            {
                key = sr.ReadLine().ToString().Split(';')[1];
            }

            return key;
        }
    }
}
