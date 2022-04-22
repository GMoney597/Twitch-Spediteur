using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
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
        DispatcherTimer timer = new DispatcherTimer();
        public List<Ort> ortsListe = new List<Ort>();
        public List<Fahrzeug> fuhrpark = new List<Fahrzeug>();
        public List<int> fahrzeugAbgeben = new List<int>();
        private Spieler sp;

        public SpielerFenster(Spieler spieler)
        {
            sp = spieler;
            InitializeComponent();
            InitializeOrteListe();
            PruefeSpielerDaten();
            PruefeSpielerStartort();
            PruefeSpielerFuhrpark();
            PruefeSpielerAuftraege();
            PruefeSpielerFrachten();
            InitializeSpielZeit();
        }

        private void PruefeSpielerDaten()
        {
            sql.HoleSpielerFinanzen(sp);
            tbkSpieler.Text = sp.Spielername;
            txtStandort.Text = sp.Startort;
            txtBargeld.Text = Convert.ToDecimal(sp.Bargeld) + " €";
            txtKontostand.Text = Convert.ToDecimal(sp.Konto) + " €";
        }

        private void PruefeSpielerFrachten()
        {
            dtgFrachten.ItemsSource = null;
            List<Fracht> loeschbareFrachten = new List<Fracht>();

            foreach (Fracht fracht in sp.Frachten)
            {
                fracht.AktualisiereErfuellung();

                if (fracht.IsErledigt())
                {
                    AuftragErledigt(fracht);
                    loeschbareFrachten.Add(fracht);
                }
                //else if (fracht.Zustand != Fracht.Status.Offen )
                //{
                //    var bearbeiteterAuftrag = sp.Auftraege.FindAll(auf => auf.Zustand == Auftrag.Status.Offen);

                //    foreach (Auftrag auftrag in sp.Auftraege)
                //    {
                //        auftrag.Zustand = Auftrag.Status.Zustellung;
                //    }
                //}
            }

            foreach (Fracht loeschen in loeschbareFrachten)
            {
                sp.Frachten.Remove(loeschen);
            }

            dtgFrachten.ItemsSource = sp.Frachten;
        }

        private void AuftragErledigt(Fracht fracht)
        {
            var FahrzeugeUnterwegs = sp.Fuhrpark.FindAll(fuhr => fuhr.HatAuftrag == true);
            var genutztesFahrzeug = FahrzeugeUnterwegs.Find(fahr => int.Parse(fahr.AuftragsNummer) == fracht.AuftragID);
            genutztesFahrzeug.Freigeben();

            var erfuellterAuftrag = sp.Auftraege.Find(auf => auf.Auftragsnummer == fracht.AuftragID);
            erfuellterAuftrag.Zustand = Auftrag.Status.Erledigt;
            sp.KontoTransaktion(erfuellterAuftrag.Auftragssumme);

            sql.ErledigeAuftrag(fracht);
            PruefeSpielerDaten();
            PruefeSpielerAuftraege();
            PruefeSpielerFuhrpark();
        }

        private void InitializeSpielZeit()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            PruefeSpielerFrachten();
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
            if (String.IsNullOrEmpty(sp.Startort))
            {
                stackOrtWaehlen.Visibility = Visibility.Visible;
                uniAktivitaet.Visibility = Visibility.Hidden;
                stackUebersicht.Visibility = Visibility.Hidden;
            }
        }

        private void InitializeOrteListe()
        {
            cboOrte.ItemsSource = null;
            sql.HoleOrte(ortsListe);
            cboOrte.ItemsSource = ortsListe;            
        }

        private void cmdSpeichereStandort_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(sp.Startort))
            {
                if (sql.SpeichereStartort(sp, cboOrte.Text))
                {
                    stackOrtWaehlen.Visibility = Visibility.Collapsed;
                    stackUebersicht.Visibility = Visibility.Visible;
                    uniAktivitaet.Visibility = Visibility.Visible;
                    txtStandort.Text = cboOrte.Text;
                    PruefeSpielerStartort();
                }
            }
        }

        private void cmdFahrzeug_Click(object sender, RoutedEventArgs e)
        {
            FahrzeugFenster vehicle = new FahrzeugFenster(sp);
            vehicle.ShowDialog();
            PruefeSpielerFuhrpark();
        }

        private void cmdFracht_Click(object sender, RoutedEventArgs e)
        {
            AngebotFenster freight = new AngebotFenster(sp);
            freight.ShowDialog();
            PruefeSpielerAuftraege();
        }

        private void dtgSpielerAuftrage_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Auftrag gewaehlterAuftrag = (Auftrag)dtgSpielerAuftrage.SelectedItem;

            if (gewaehlterAuftrag != null)
            {
                if (gewaehlterAuftrag.FahrzeugID == "keins")
                {
                    List<Fahrzeug> moeglicheFahrzeuge = new List<Fahrzeug>();

                    var treffer = sp.Fuhrpark.FindAll(x => x.HatAuftrag == false);

                    if (treffer.Count > 0)
                    {
                        foreach (var item in treffer)
                        {
                            moeglicheFahrzeuge.Add(item);
                        }

                        FahrzeugZuweisenMessageBox fzgmsgbox = new FahrzeugZuweisenMessageBox(moeglicheFahrzeuge, gewaehlterAuftrag, sp.Frachten);
                        fzgmsgbox.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Du hast aktuell kein verfügbares Fahrzeug für einen Transport", "Ohne Fahrzeug kein Transport",
                            MessageBoxButton.OK, MessageBoxImage.Stop);
                    }

                    PruefeSpielerFuhrpark();
                    PruefeSpielerAuftraege();
                }
            }
        }

        private void cmdBank_Click(object sender, RoutedEventArgs e)
        {
            BankFenster bank = new BankFenster(sp);
            bank.ShowDialog();
            PruefeSpielerDaten();
        }
    }
}
