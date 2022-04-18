﻿using System;
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
            PruefeSpielerFrachten();
            InitializeSpielZeit();

            tbkSpielZeit.Text = DateTime.Now.ToShortTimeString();
            tbkSpieler.Text = sp.Spielername;
            txtStandort.Text = sp.Startort;
            txtBargeld.Text = Convert.ToDecimal(sp.Bargeld) + " €";
            txtKontostand.Text = Convert.ToDecimal(sp.Konto) + " €";
        }

        // PRUEFE SPIELER-DATEN Bargeld und Kontostand abrufen, Fahrzeugstandort !!!

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
                else if (fracht.Zustand != Fracht.Status.Offen)
                {
                    var bearbeiteterAuftrag = sp.Auftraege.FindAll(auf => auf.Zustand == Auftrag.Status.Offen);

                    foreach (Auftrag auftrag in sp.Auftraege)
                    {
                        auftrag.Zustand = Auftrag.Status.Zustellung;
                    }
                }
            }

            foreach (Fracht loeschen in loeschbareFrachten)
            {
                sp.Frachten.Remove(loeschen);
            }

            dtgFrachten.ItemsSource = sp.Frachten;
        }

        private void AuftragErledigt(Fracht fracht)
        {
            var genutztesFahrzeug = sp.Fuhrpark.Find(fuhr => int.Parse(fuhr.AuftragsNummer) == fracht.AuftragID);
            genutztesFahrzeug.Freigeben();

            var erfuellterAuftrag = sp.Auftraege.Find(auf => auf.Auftragsnummer == fracht.AuftragID);
            erfuellterAuftrag.Zustand = Auftrag.Status.Erledigt;
            sp.KontoTransaktion(erfuellterAuftrag.Auftragssumme);
        }

        private void InitializeSpielZeit()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            tbkSpielZeit.Text = (DateTime.Parse(tbkSpielZeit.Text)).AddMinutes(1).ToShortTimeString();
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
            Auftrag temp = (Auftrag)dtgSpielerAuftrage.SelectedItem;

            if (temp != null)
            {
                if (temp.FahrzeugID == "keins")
                {
                    List<Fahrzeug> moeglicheFahrzeuge = new List<Fahrzeug>();

                    var treffer = sp.Fuhrpark.FindAll(x => x.HatAuftrag == false);

                    if (treffer.Count > 0)
                    {
                        foreach (var item in treffer)
                        {
                            moeglicheFahrzeuge.Add(item);
                        }

                        FahrzeugZuweisenMessageBox fzgmsgbox = new FahrzeugZuweisenMessageBox(moeglicheFahrzeuge, temp, sp.Frachten);
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
    }
}
