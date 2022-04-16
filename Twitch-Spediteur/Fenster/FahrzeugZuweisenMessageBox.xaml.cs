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
    /// Interaktionslogik für FahrzeugZuweisenMessageBox.xaml
    /// </summary>
    public partial class FahrzeugZuweisenMessageBox : Window
    {
        SQLite sql = new SQLite();
        List<Fahrzeug> Fuhrpark = new List<Fahrzeug>();
        Fahrzeug gewaehltesFahrzeug;
        Auftrag gewaehlterAuftrag;

        public FahrzeugZuweisenMessageBox(List<Fahrzeug> fuhr, Auftrag auf)
        {
            InitializeComponent();
            Fuhrpark = fuhr;
            gewaehlterAuftrag = auf;

            cboFahrzeuge.ItemsSource = Fuhrpark;
        }

        private void cmdSend_Click(object sender, RoutedEventArgs e)
        {
            // Parameter: AngebotID, Abholort, Lieferort, Ware, AuftragStatus, FahrzeugID
            sql.AktualisiereAuftrag(gewaehlterAuftrag, gewaehltesFahrzeug);
            MessageBox.Show("Fahrzeug wurde mit einem Auftrag versendet", "Auftrag in Ausführung", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void cboFahrzeuge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gewaehltesFahrzeug = (Fahrzeug)cboFahrzeuge.SelectedItem;
            tbkGewaehltesFahrzeug.Text = "Fahrzeug: " + gewaehltesFahrzeug.Typ;
            tbkStandort.Text = "Standort: " + gewaehltesFahrzeug.Standort;
        }
    }
}
