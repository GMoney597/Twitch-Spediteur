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
    /// Interaktionslogik für VehicleWindow.xaml
    /// </summary>
    public partial class VehicleWindow : Window
    {
        List<Fahrzeug> neuFahrzeuge = new List<Fahrzeug>();
        List<Fahrzeug> gebrauchtFahrzeuge = new List<Fahrzeug>();

        Spieler sp;
        Fahrzeug temp;

        public VehicleWindow(Spieler spieler)
        {
            InitializeComponent();

            sp = spieler;

            neuFahrzeuge.Add(new Fahrzeug("Kombi", 0.5m, 50, 400, 40000));
            neuFahrzeuge.Add(new Fahrzeug("Transporter", 1.5m, 60, 900, 60000));
            neuFahrzeuge.Add(new Fahrzeug("Mini-Truck", 2.8m, 70, 1350, 75000));
            neuFahrzeuge.Add(new Fahrzeug("LKW 7.5t", 7.5m, 100, 2000, 100000));
            neuFahrzeuge.Add(new Fahrzeug("LKW 12t", 12m, 120, 4000, 150000));
            neuFahrzeuge.Add(new Fahrzeug("Sattelzug", 30m, 200, 15000, 300000));

            cboNeuFahrzeuge.ItemsSource = neuFahrzeuge;
        }

        private void cboNeuFahrzeuge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            temp = (Fahrzeug)cboNeuFahrzeuge.SelectedItem;

            if (sp.Bargeld > temp.KaufPreis)
            {
                // Bereich Mieten
                txtMietpreis.Text = temp.MietPreis.ToString();
                txtMietpreis.Visibility = Visibility.Visible;
                cmdMieten.Visibility = Visibility.Visible;
                // Bereich Kaufen
                txtKaufpreis.Text = temp.KaufPreis.ToString();
                txtKaufpreis.Visibility = Visibility.Visible;
                cmdKaufen.Visibility = Visibility.Visible;
            }
            else if (sp.Bargeld > temp.MietPreis)
            {
                // Bereich Mieten
                txtMietpreis.Text = temp.MietPreis.ToString();
                txtMietpreis.Visibility = Visibility.Visible;
                cmdMieten.Visibility = Visibility.Visible;
                // Bereich Kaufen
                txtKaufpreis.Visibility = Visibility.Hidden;
                cmdKaufen.Visibility = Visibility.Hidden;
            }
            else
            {
                txtMietpreis.Visibility = Visibility.Hidden;
                cmdMieten.Visibility = Visibility.Hidden;
                txtKaufpreis.Visibility = Visibility.Hidden;
                cmdKaufen.Visibility = Visibility.Hidden; 
            }

            if (sp.Fuhrpark != null && sp.Fuhrpark.Count > 0)
            {
                foreach (Fahrzeug fz in sp.Fuhrpark)
                {
                    if (fz.Typ == temp.Typ && fz.IsGekauft)
                    {
                        txtVerkaufpreis.Visibility = Visibility.Visible;
                        cmdVerkaufen.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        txtVerkaufpreis.Visibility = Visibility.Hidden;
                        cmdVerkaufen.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmdMieten_Click(object sender, RoutedEventArgs e)
        {
            if (temp != null)
            {
                MessageBoxResult result = MessageBox.Show("Willst Du das Fahrzeug zu einem Preis von " + temp.MietPreis + " für eine" +
                    " Woche mieten?\nAbgabe ist am: " + DateTime.Today.AddDays(7).ToShortDateString(), "Bestätige den Mietvertrag", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    sp.Fuhrpark.Add(temp);
                    sp.GeldTransaktion(temp.MietPreis);
                    cmdClose_Click(sender, e);
                }
            }
        }

        private void cmdKaufen_Click(object sender, RoutedEventArgs e)
        {
            if (temp != null)
            {
                temp.WurdeGekauft();
                sp.Fuhrpark.Add(temp);
                sp.GeldTransaktion(temp.KaufPreis);
                cmdClose_Click(sender, e);
            }
        }

        private void cmdVerkaufen_Click(object sender, RoutedEventArgs e)
        {
            if (temp != null)
            {
                //sp.Fuhrpark.Add(temp);
                //sp.GeldTransaktion(temp.VerkaufPreis);
                //cmdClose_Click(sender, e);
            }
        }
    }
}
