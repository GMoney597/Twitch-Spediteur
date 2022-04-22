using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur.Fenster
{
    /// <summary>
    /// Interaktionslogik für FahrzeugZuweisenMessageBox.xaml
    /// </summary>
    public partial class FahrzeugZuweisenMessageBox : Window
    {
        SQLite sql = new SQLite();
        List<Fahrzeug> Fuhrpark;
        List<Fracht> Frachten;
        Fahrzeug gewaehltesFahrzeug;
        Auftrag gewaehlterAuftrag;
        List<Entfernung> vorhandeneRouten = new List<Entfernung>();
        Entfernung startRoute;

        public FahrzeugZuweisenMessageBox(List<Fahrzeug> fuhr, Auftrag auf, List<Fracht> fra)
        {
            InitializeComponent();
            Fuhrpark = fuhr;
            Frachten = fra;
            gewaehlterAuftrag = auf;

            cboFahrzeuge.ItemsSource = Fuhrpark;
        }

        private void cmdSend_Click(object sender, RoutedEventArgs e)
        {
            // Parameter: AngebotID, Abholort, Lieferort, Ware, AuftragStatus, FahrzeugID
            sql.AktualisiereAuftrag(gewaehlterAuftrag, gewaehltesFahrzeug);
            Frachten.Add(new Fracht(gewaehltesFahrzeug, gewaehlterAuftrag, startRoute));
            MessageBox.Show("Fahrzeug wurde mit einem Auftrag versendet", "Auftrag in Ausführung", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void cboFahrzeuge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gewaehltesFahrzeug = (Fahrzeug)cboFahrzeuge.SelectedItem;
            tbkGewaehltesFahrzeug.Text = "Fahrzeug: " + gewaehltesFahrzeug.Typ;
            tbkStandort.Text = "Standort: " + gewaehltesFahrzeug.Standort;

            if (gewaehltesFahrzeug.Standort != gewaehlterAuftrag.Abholort)
            {
                MessageBoxResult result = MessageBox.Show("Dieses Fahrzeug steht nicht am Abhol-Ort, willst Du wirklich dieses Fahrzeug wählen?", "ACHTUNG - Ortshinweis",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Yes)
                {
                    sql.HoleRouten(vorhandeneRouten);

                    var routeVorhanden = vorhandeneRouten.Find(rou => rou.Abholort == gewaehltesFahrzeug.Standort && rou.Lieferort == gewaehlterAuftrag.Abholort);

                    if (routeVorhanden != null)
                    {
                        startRoute = new Entfernung(routeVorhanden.Abholort, routeVorhanden.Lieferort, routeVorhanden.Route, routeVorhanden.Distanz);
                    }
                    else
                    {
                        MessageBoxResult routeAnlegen = MessageBox.Show("Soll die Route berechnet und erstellt werden?", "Route Berechnen",
                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (routeAnlegen == MessageBoxResult.Yes)
                        {
                            ErzeugeRoute(gewaehltesFahrzeug.Standort, gewaehlterAuftrag.Abholort);
                        }
                    }
                }
            }
            else
            {
                var routeVorhanden = vorhandeneRouten.Find(rou => rou.Abholort == gewaehltesFahrzeug.Standort && rou.Lieferort == gewaehlterAuftrag.Abholort);
                startRoute = new Entfernung(routeVorhanden.Abholort, routeVorhanden.Lieferort, routeVorhanden.Route, routeVorhanden.Distanz);
            }
        }

        private void ErzeugeRoute(string standort, string abholort)
        {
            string key = "";

            using FileStream file = new FileStream(Properties.Settings.Default["API_Key"].ToString(), FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file))
            {
                key = sr.ReadLine().ToString().Split(';')[1];
            }

            string startOrt = standort;
            string zielOrt = abholort;
            string abfrageUri = "";

            abfrageUri = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=" + startOrt + "&wp.1=" + zielOrt + "&key=" + key;

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

            startRoute = new Entfernung(startOrt, zielOrt, hin, entfernung);
        }
    }
}
