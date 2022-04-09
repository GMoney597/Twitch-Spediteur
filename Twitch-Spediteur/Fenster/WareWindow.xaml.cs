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
            InitializeWarenKatalog();

            cboVerladung.ItemsSource = Enum.GetValues(typeof(Ware.Verladung));
            cboEinheit.ItemsSource = Enum.GetValues(typeof(Ware.Einheit));
            cboMerkmal.ItemsSource = Enum.GetValues(typeof(Ware.Merkmal));
        }

        private void InitializeWarenKatalog()
        {
            waren = sql.HoleWaren();
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
