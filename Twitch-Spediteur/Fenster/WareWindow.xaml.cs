using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static Twitch_Spediteur.Fenster.Ware;

namespace Twitch_Spediteur.Fenster
{
    /// <summary>
    /// Interaktionslogik für WareWindow.xaml
    /// </summary>
    public partial class WareWindow : Window
    {
        List<Ware> waren = new List<Ware>();
        SQLite sql = new SQLite();

        public WareWindow()
        {
            InitializeComponent();

            Ware benzin = new Ware("Benzin", Verladung.Tank, 0.5m, Einheit.Liter, Merkmal.Gefahr);
            Ware öl = new Ware("Öl", Verladung.Tank, 1.2m, Einheit.Liter, Merkmal.Gefahr);

            waren.Add(benzin);
            waren.Add(öl);

            cboVerladung.ItemsSource = Enum.GetValues(typeof(Ware.Verladung));
            cboEinheit.ItemsSource = Enum.GetValues(typeof(Ware.Einheit));
            cboMerkmal.ItemsSource = Enum.GetValues(typeof(Ware.Merkmal));

            AktualisiereWarenKatalog();
        }

        private void cmdSpeichern_Click(object sender, RoutedEventArgs e)
        {
            string bezeichnung = txtBezeichnung.Text;
            Verladung lad = (Verladung)cboVerladung.SelectedItem;
            decimal preis = Convert.ToDecimal(txtPreis.Text);
            Einheit ein = (Einheit)cboEinheit.SelectedItem;
            Merkmal merkmal = (Merkmal)cboMerkmal.SelectedItem;

            Ware temp = new Ware(bezeichnung, lad, preis, ein, merkmal);
            waren.Add(temp);

            if (sql.SpeichereWare(temp))
            {
                tbkMeldung.Text = "Eine neue Ware wurde im Katalog hinterlegt.";
            }

            AktualisiereWarenKatalog();
            LeereFormular();
        }

        private void LeereFormular()
        {
            txtBezeichnung.Text = "";
            cboVerladung.Text = "";
            txtPreis.Text = "";
            cboEinheit.Text = "";
            cboMerkmal.Text = "";
        }

        private void AktualisiereWarenKatalog()
        {
            dtgWaren.ItemsSource = null;
            dtgWaren.Items.Clear();
            dtgWaren.ItemsSource = waren;
        }
    }
}
