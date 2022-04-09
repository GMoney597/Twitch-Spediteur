﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur
{
    public class Spieler
    {
        SQLite sql = new SQLite();
        SHA256 sHA256 = SHA256.Create();

        public string Spielername { get; private set; }
        public string Mail { get; private set; }
        public string Passwort { get; private set; }
        public decimal Bargeld { get; private set; }
        public decimal Konto { get; private set; }
        public List<Fahrzeug> Fuhrpark { get; private set; } = new List<Fahrzeug>();
        public List<Nachricht> Nachrichten { get; private set; } = new List<Nachricht>();
        public string Startort { get; private set; }


        public Spieler(string name, string mail, string passwort)
        {
            byte[] pwValue = sHA256.ComputeHash(Encoding.UTF8.GetBytes(passwort));
            string pwHash = Convert.ToBase64String(pwValue);
            Spielername = name;
            Mail = mail;
            Passwort = pwHash;

            Bargeld = 1000.0M;
            Konto = 0.0M;
        }

        public Spieler(string Name, string Mail, decimal Bar, decimal Kontostand, string Ort)
        {
            Spielername = Name;
            this.Mail = Mail;
            Bargeld = Bar;
            Konto = Kontostand;
            Startort = Ort;
        }

        public bool Registrieren()
        {
            bool register = sql.RegistriereSpieler(this);
            return register;
        }

        public bool Einloggen(string name_mail, string passwort)
        {
            bool login = sql.EinloggenSpieler(name_mail, passwort);
            return login;
        }

        public bool Nachricht_schreiben()
        {
            throw new System.NotImplementedException();
        }

        internal void GeldTransaktion(decimal preis)
        {
            Bargeld -= preis;
            sql.BargeldUpdate(this.Spielername, Bargeld);
        }

        internal void ParkeFahrzeug(Fahrzeug temp)
        {
            Fuhrpark.Add(temp);
            sql.ParkeFahrzeug(this.Spielername, temp);
        }
    }
}
