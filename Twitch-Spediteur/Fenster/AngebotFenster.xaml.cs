using System;
using System.Collections.Generic;
using System.Windows;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur.Fenster
{
    /// <summary>
    /// Interaktionslogik für FreightWindow.xaml
    /// </summary>
    public partial class AngebotFenster : Window
    {
        public static Random rand = new Random();
        List<Ort> orte = new List<Ort>();
        List<Entfernung> routen = new List<Entfernung>();
        List<Ware> waren = new List<Ware>();
        List<Angebot> frachten = new List<Angebot>();
        List<Ware> moeglicheWaren = new List<Ware>();
        List<int> ladungsKeys = new List<int>();
        Spieler spieler;
        SQLite sql = new SQLite();

        public AngebotFenster(Spieler sp)
        {
            InitializeComponent();
            spieler = sp;
            InitializeFrachtBoerse();
        }

        private void InitializeFrachtBoerse()
        {
            // Hole aktuelle Frachtaufträge aus der DB und weise sie der Börse oder dem Spieler zu
            // frachten = sql.HoleAuftraege(frachten, spieler);

            if (frachten.Count < 10)
            {
                // Erstelle Frachtaufträge bis Börse mindestens 10 Aufträge anbietet
                sql.HoleOrte(orte);
                sql.HoleWaren(waren);
                sql.HoleRouten(routen);

                foreach (Fahrzeug fahrzeug in spieler.Fuhrpark)
                {
                    var treffer = waren.FindAll(x => ((Ware.Verladung)fahrzeug.VerladeSchlüssel).HasFlag(x.Ladung));
                    foreach (var item in treffer)
                    {
                        moeglicheWaren.Add(item);
                    }
                }

                // Wenn weniger als 10 Frachtangebote vorhanden sind, ergänze, bis 10 Angebote
                while (frachten.Count < 10)
                {
                    Ware w = moeglicheWaren[rand.Next(moeglicheWaren.Count)];

                    int menge = rand.Next(0, 500);
                    
                    Entfernung ent = (Entfernung)routen[rand.Next(routen.Count)];
                    decimal summe = ent.Distanz * 0.5m;

                    frachten.Add(new Angebot(ent.Abholort, ent.Lieferort, ent.Distanz, w.Bezeichnung, w.BasisEinheit.ToString(), menge, summe));
                }
            }

            dtgAngebot.ItemsSource = frachten;
        }

        private void cmdAnnehmen_Click(object sender, RoutedEventArgs e)
        {
            Angebot gewaehlterAuftrag = (Angebot)dtgAngebot.CurrentItem;

            MessageBoxResult msgResult = MessageBox.Show("Auftrag: \t\t" + gewaehlterAuftrag.Bezeichnung + 
                "\nBezahlung: \t" + String.Format("{0:C2}", gewaehlterAuftrag.Wert) +
                "\nAbhol-Ort: \t" + gewaehlterAuftrag.Abholort + 
                "\nLiefer-Ort: \t" + gewaehlterAuftrag.Lieferort, "Fracht annehmen", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (msgResult == MessageBoxResult.Yes)
            {
                spieler.GebeAuftrag(new Auftrag(gewaehlterAuftrag));
                
                dtgAngebot.ItemsSource = null;
                frachten.Remove(gewaehlterAuftrag);
                dtgAngebot.ItemsSource = frachten;
            }
        }
    }
}
