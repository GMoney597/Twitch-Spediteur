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
        List<Fahrzeug> fuhrpark = new List<Fahrzeug>();
        Spieler sp;

        public VehicleWindow(List<Fahrzeug> fuhrpark, Spieler spieler)
        {
            InitializeComponent();
            InitializeFahrzeugmarkt();

            this.fuhrpark = fuhrpark;
            sp = spieler;
        }

        private void InitializeFahrzeugmarkt()
        {

        }

        private void cmdMieteKombi_Click(object sender, RoutedEventArgs e)
        {
            if (sp.Bargeld > 400)
            {
                fuhrpark.Add(new Fahrzeug("Kombi", 0.5m, 50, 400, 40000));
                sp.FahrzeugMieten(400);
            }

            MessageBox.Show("Du hat erfolgreich einen Kombi gemietet", "Transaktion abgeschlossen", MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
        }

        private void cmdKaufKombi_Click(object sender, RoutedEventArgs e)
        {
            if (sp.Bargeld > 40000)
            {
                fuhrpark.Add(new Fahrzeug("Kombi", 0.5m, 50, 400, 40000));
                sp.FahrzeugKaufen(40000);
            }
        }
    }
}
