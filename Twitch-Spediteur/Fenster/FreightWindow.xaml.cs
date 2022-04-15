using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur.Fenster
{
    /// <summary>
    /// Interaktionslogik für FreightWindow.xaml
    /// </summary>
    public partial class FreightWindow : Window
    {
        public static Random rand = new Random();
        List<Entfernung> orte = new List<Entfernung>();
        List<Ware> waren = new List<Ware>();
        List<Angebot> frachten = new List<Angebot>();
        List<Ware> moeglicheWaren = new List<Ware>();

        List<int> ladungsKeys = new List<int>();
        Spieler spieler;
        SQLite sql = new SQLite();

        public FreightWindow(Spieler sp)
        {
            InitializeComponent();
            spieler = sp;
            InitializeFrachtBoerse();
        }

        private void InitializeFrachtBoerse()
        {

            // Hole aktuelle Frachtaufträge aus der DB und weise sie der Börse oder dem Spieler zu
            frachten = sql.HoleAuftraege(frachten, spieler);

            if (frachten.Count < 10)
            {
                // Erstelle Frachtaufträge bis Börse mindestens 10 Aufträge anbietet
                orte = sql.HoleOrte(orte);
                waren = sql.HoleWaren(waren);

                foreach (Fahrzeug fahrzeug in spieler.Fuhrpark)
                {
                    var treffer = waren.FindAll(x => ((Ware.Verladung)fahrzeug.VerladeSchlüssel).HasFlag(x.Ladung));
                    foreach (var item in treffer)
                    {
                        moeglicheWaren.Add(item);
                    }
                }

                while (frachten.Count < 10)
                {
                    Ware w = moeglicheWaren[rand.Next(moeglicheWaren.Count)];

                    int menge = rand.Next(0, 500);
                    // decimal summe = Math.Round(Convert.ToDecimal((rand.Next(50, 120) * (Double)w.Preis) * menge), 2);

                    Entfernung ort = orte[rand.Next(orte.Count)];
                    decimal summe = ort.Distanz * 0.5m;

                    frachten.Add(new Angebot(ort.Start, ort.Ziel, ort.Distanz, w.Bezeichnung, w.BasisEinheit.ToString(), menge, summe));
                }
            }

            dtgAngebot.ItemsSource = frachten;
        }

        private void cmdAnnehmen_Click(object sender, RoutedEventArgs e)
        {
            Angebot temp = (Angebot)dtgAngebot.CurrentItem;
            //MessageBoxButton accept = new MessageBoxButton();
            //MessageBoxButton decline = new MessageBoxButton();

            MessageBoxResult msgResult = MessageBox.Show("Auftrag: \t\t" + temp.Bezeichnung + 
                "\nBezahlung: \t" + String.Format("{0:C2}", temp.Wert) +
                "\nAbhol-Ort: \t" + temp.Abholort + 
                "\nLiefer-Ort: \t" + temp.Lieferort, "Fracht annehmen", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (msgResult == MessageBoxResult.Yes)
            {
                spieler.Auftraege.Add(temp);

                dtgAngebot.ItemsSource = null;
                frachten.Remove(temp);
                dtgAngebot.ItemsSource = frachten;
            }
        }
    }
}
