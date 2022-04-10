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
        Spieler spieler;
        SQLite sql = new SQLite();

        public FreightWindow(Spieler sp)
        {
            InitializeComponent();
            InitializeFrachtBoerse();

            spieler = sp;
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

                while(frachten.Count < 10)
                {
                    Ware w = waren[rand.Next(waren.Count)];

                    frachten.Add(new Fracht(orte[rand.Next(orte.Count)].Start, orte[rand.Next(orte.Count)].Ziel,
                        w.Bezeichnung, rand.Next(500, 1000), Convert.ToDecimal(rand.NextDouble() * (Double)w.Preis)));
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
