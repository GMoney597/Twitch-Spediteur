using System;
using System.Windows;
using Twitch_Spediteur.Fenster;

namespace Twitch_Spediteur.Klassen
{
    public class Fracht
    {
        public int AuftragID { get; private set; }
        public string Abholort { get; private set; }
        public string Lieferort { get; private set; }
        public int Entfernung { get; private set; }
        public int Erfuellungsgrad { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime Ende { get; private set; }
        public string Typ { get; private set; }
        public Status Zustand { get; private set; }
        public string Standort { get; private set; }
        public bool IsAbgeholt { get; private set; }
        public bool IsZugestellt { get; private set; }
        public int ZeitZaehler { get; private set; } 

        public enum Status
        {
            Offen,
            Abholung,
            Beladung,
            Zustellung,
            Entladung,
            Erledigt,
            Loeschen
        }

        //enum FahrzeugTyp
        //{
        //    Kombi,
        //    Transporter,
        //    MiniTruck,
        //    Lkw75,
        //    Lkw12,
        //    Sattelzug
        //}

        public Fracht(Fahrzeug fahr, Auftrag auf)
        {
            AuftragID = auf.Auftragsnummer;
            Abholort = auf.Abholort;
            Lieferort = auf.Lieferort;
            Standort = fahr.Standort;
            Entfernung = auf.Entfernung;
            Start = auf.AuftragsDatum;
            Typ = fahr.Typ;
            Zustand = Status.Offen;
            ZeitZaehler = 0;
        }

        // Standort-Ideen
        /* Fahrzeug steht ohne Auftrag:     Standort        (z.B. Nürnberg)
         * Fahrzeug fährt zum Abholort:     --> Abholort    (z.B. --> Nürnberg)
         * Fahrzeug steht zur Beladung:     -> Abholort <-  (z.B. -> Nürnberg <-)
         * Fahrzeug fährt zum Lieferort:    --> Lieferort   (z.B. --> Aachen)
         * Fahrzeug fährt zur Entladung:    <- Lieferort -> (z.B. <- Aachen ->)
         * Fahrzeug wartet auf Erledigung:  ! Lieferort     (z.B. ! Aachen)
         */

        // Erfüllungsgrad
        /* Kombi fährt im Abholort zur Beladung ca. 15 Minuten
         * Kombi wartet 30 Minuten bis Beladung-Ende
         * Kombi fährt zum Lieferort mit durchschnittlich 70km/h
         * Kombi fährt im Zielort zur Entladung ca. 15 Minuten
         * Kombi wartet 30 Minuten auf Entladung-Ende
         * Kombi wartet 10 Minuten bis Erledigung
         */

        public void AktualisiereErfuellung()
        {
            ZeitZaehler++;

            if (Erfuellungsgrad == Entfernung)
            {
                Zustand = Status.Zustellung;
            }

            switch (Typ)
            {
                case "Kombi":
                    // Entfernungs-Summe =  Standort -> Abholort -> Lieferort
                    // Zeitfluss-Summe=     Standort -> Abholort -> Beladen -> Lieferort -> Entladen -> Erledigung
                    // pro 70 km = 1 Stunde real => 15 Minuten virtuell
                    // 15 Minuten Be-/Entlade-Anfahrt = 3.45 virtuell
                    // 30 Minuten Be-/Entladung = 7.30 virtuell
                    // 10 Minuten bis Erledigung = 2.30 virtuell
                    switch (ZeitZaehler)
                    {
                        // 60 Minuten = 70 km
                        // 6 Minuten = 7 km => 6 Ticks = 6 Minuten

                        case 6:
                            if (Zustand == Status.Offen && Standort == Abholort)
                            {
                                Standort = "--> " + Abholort;
                                Zustand = Status.Abholung;
                                ZeitZaehler = 0;
                            }
                            else if (Zustand == Status.Zustellung && Entfernung > Erfuellungsgrad)
                            {
                                Standort = "--> " + Lieferort;
                                Erfuellungsgrad += 7;
                                ZeitZaehler = 0;
                            }
                            break;
                        case 10:
                            if (Zustand == Status.Erledigt)
                            {
                                MessageBox.Show("Du hast den Auftrag erfolgreich abgeschlossen.", "Ladung übergeben", MessageBoxButton.OK, MessageBoxImage.Information);
                                Zustand = Status.Loeschen;
                            }
                            break;
                        case 15:
                            if (Zustand == Status.Abholung)
                            {
                                Standort = "-> " + Abholort + " <-";
                                Zustand = Status.Beladung;
                                ZeitZaehler = 0;
                            }
                            else if (Zustand == Status.Zustellung)
                            {
                                Standort = "<- " + Lieferort + " ->";
                                Zustand = Status.Entladung;
                                ZeitZaehler = 0;
                            }
                            break;
                        case 30:
                            if (Zustand == Status.Entladung)
                            {
                                Standort = "! " + Lieferort;
                                Zustand = Status.Erledigt;
                                IsZugestellt = true;
                                Ende = DateTime.Now;
                                ZeitZaehler = 0;
                            }
                            else if (Zustand == Status.Beladung)
                            {
                                Standort = "--> " + Lieferort;
                                Zustand = Status.Zustellung;
                                IsAbgeholt = true;
                                ZeitZaehler = 0;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "Transporter":
                    //Erfuellungsgrad += 65;
                    break;
                case "Mini-Truck":
                    //Erfuellungsgrad += 60;
                    break;
                case "LKW 7.5t":
                    //Erfuellungsgrad += 55;
                    break;
                case "LKW 12t":
                    //Erfuellungsgrad += 50;
                    break;
                case "Sattelzug":
                    //Erfuellungsgrad += 45;
                    break;
                default:
                    break;
            }
        }

        public bool IsErledigt()
        {
            return (Zustand == Status.Loeschen);
        }
    }
}