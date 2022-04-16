using System;
using System.Collections.Generic;
using System.Windows;
using Twitch_Spediteur.Fenster;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur
{
    /// <summary>
    /// Interaktionslogik für UserInterface.xaml
    /// </summary>
    public partial class SpielerFenster : Window
    {
        SQLite sql = new SQLite();
        //public static List<Ort> ortsListe = new List<Ort>();
        public List<Fahrzeug> fuhrpark = new List<Fahrzeug>();

        private Spieler sp;
        //FileStream file = new FileStream("")

        public SpielerFenster(Spieler spieler)
        {
            sp = spieler;
            InitializeComponent();
            InitializeOrteListe();
            PruefeSpielerStartort();
            PruefeSpielerFuhrpark();
            PruefeSpielerAuftraege();

            tbkSpieler.Text = sp.Spielername;
            txtStandort.Text = sp.Startort;
            txtBargeld.Text = Convert.ToDecimal(sp.Bargeld) + " €";
            txtKontostand.Text = Convert.ToDecimal(sp.Konto) + " €";
        }

        private void PruefeSpielerAuftraege()
        {
            sp.ResetAuftraege();
            sql.HoleAuftraege(sp);
            dtgSpielerAuftrage.ItemsSource = sp.Auftraege;
        }

        private void PruefeSpielerFuhrpark()
        {
            sp.ResetFuhrpark();
            sql.HoleFuhrpark(sp);
            dtgFuhrpark.ItemsSource = null;

            if (sp.Fuhrpark.Count > 0)
            {
                cmdFracht.Visibility = Visibility.Visible;
                dtgFuhrpark.ItemsSource = sp.Fuhrpark;
            }
        }

        private void PruefeSpielerStartort()
        {
            if (!String.IsNullOrEmpty(sp.Startort))
            {
                stackOrtWaehlen.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeOrteListe()
        {
            //Ort ort = new Ort("Bremen");
            //cboOrte.ItemsSource = ortsListe;
            cboOrte.Items.Add("Hannover");
            cboOrte.Items.Add("Berlin");
            cboOrte.Items.Add("München");
            cboOrte.Items.Add("Bonn");
            cboOrte.Items.Add("Stuttgart");
            cboOrte.Items.Add("Hamburg");
            cboOrte.Items.Add("Leverkusen");
        }

        private void cmdSpeichereStandort_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(sp.Startort))
            {
                SQLite sql = new SQLite();
                if (sql.SpeichereStartort(sp, cboOrte.Text))
                {
                    stackOrtWaehlen.Visibility = Visibility.Collapsed;
                    txtStandort.Text = cboOrte.Text;                    
                }                
            }
        }

        private void cmdFahrzeug_Click(object sender, RoutedEventArgs e)
        {
            FahrzeugFenster vehicle = new FahrzeugFenster(sp);
            vehicle.ShowDialog();
            txtBargeld.Text = sp.Bargeld.ToString();
        }

        private void cmdFracht_Click(object sender, RoutedEventArgs e)
        {
            AngebotFenster freight = new AngebotFenster(sp);
            freight.ShowDialog();
            PruefeSpielerAuftraege();
        }

        private void cmdAusfuehren_Click(object sender, RoutedEventArgs e)
        {
            Auftrag temp = (Auftrag)dtgSpielerAuftrage.SelectedItem;
            int row = dtgSpielerAuftrage.SelectedIndex;

            FahrzeugZuweisenMessageBox fzgmsgbox = new FahrzeugZuweisenMessageBox(sp.Fuhrpark, temp);
            fzgmsgbox.ShowDialog();

            PruefeSpielerFuhrpark();
            PruefeSpielerAuftraege();

        }
    }
}
