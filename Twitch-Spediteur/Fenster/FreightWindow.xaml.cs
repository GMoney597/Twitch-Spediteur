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
        List<Fracht> frachten = new List<Fracht>();
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
                    //for (int val = 0; val <= 16; val++)
                    //    Console.WriteLine("{0,3} - {1:G}", val, (MultiHue)val);

                    // Wenn Spieler ein Fahrzeug hat X
                    // ladungsKeys.Add(fahrzeug.VerladeSchlüssel);
                    //var treffer = waren.FindAll(x => x.Ladung.HasFlag(fahrzeug.VerladeSchlüssel));
                    var treffer = waren.FindAll(x => ((Ware.Verladung)fahrzeug.VerladeSchlüssel).ToString().Contains(x.Ladung.ToString()));
                    foreach (var item in treffer)
                    {
                        moeglicheWaren.Add(item);
                    }
                }

                while (frachten.Count < 10)
                {
                    Ware w = moeglicheWaren[rand.Next(moeglicheWaren.Count)];

                    int menge = rand.Next(0, 500);
                    decimal summe = Math.Round(Convert.ToDecimal((rand.Next(50, 120) * (Double)w.Preis) * menge), 2);

                    frachten.Add(new Fracht(orte[rand.Next(orte.Count)].Start, orte[rand.Next(orte.Count)].Ziel,
                        w.Bezeichnung, menge, summe));
                }
            }

            dtgFracht.ItemsSource = frachten;
        }

        private void cmdAnnehmen_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgResult = MessageBox.Show("Du hast folgenden Auftrag angeklickt: " + 
                ((Fracht)dtgFracht.CurrentItem).Bezeichnung, "Fracht annehmen", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (msgResult == MessageBoxResult.Yes)
            {
                spieler.Auftraege.Add((Fracht)dtgFracht.CurrentItem);

                dtgFracht.ItemsSource = null;
                frachten.Remove((Fracht)dtgFracht.CurrentItem);
                dtgFracht.ItemsSource = frachten;
                //this.Close();
            }
        }
    }
}
