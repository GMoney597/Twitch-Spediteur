using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Twitch_Spediteur
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Spieler> spielerList = new List<Spieler>();
        // static string path = @"e:\projects\twitch.csv";
        //FileStream filestream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        SQLite sql = new SQLite();

        public MainWindow()
        {
            InitializeComponent();
            InitializeSpielerliste();
        }

        private void InitializeSpielerliste()
        {
            //StreamReader sr = new StreamReader(filestream);

            //while (!sr.EndOfStream)
            //{
            //    string[] player = sr.ReadLine().Split(';');
            //    Spieler spieler = new Spieler(player[0], player[1], player[2]);
            //    spielerList.Add(spieler);
            //}

            //sr.Close();
            //filestream.Close();

            spielerList = sql.HoleSpieler();

            tbkMessage.Text = spielerList.Count.ToString() + " Spieler bereits registriert.";

            SpielerListeAktualisieren();
        }

        private void SpielerListeAktualisieren()
        {
            lstSpieler.ItemsSource = null;
            lstSpieler.Items.Clear();
            lstSpieler.ItemsSource = spielerList;
        }

        private void cmdRegistrieren_Click(object sender, RoutedEventArgs e)
        {
            tbkMessage.Text = "";

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            // Prüfe, ob alle Felder ausgefüllt wurden
            if (String.IsNullOrEmpty(txtName.Text) ||
                String.IsNullOrEmpty(txtMail.Text) ||
                String.IsNullOrEmpty(pwdPasswort.Text))
            {
                tbkMessage.Foreground = Brushes.Red;
                tbkMessage.Text = "Keine Daten angebeben.";
            }
            // Prüfe, ob die angegebene Mail-Adresse gültig ist
            else if (regex.Match(txtMail.Text).Success)
            {
                tbkMessage.Foreground = Brushes.Black;

                // Lege einen neuen Spieler an
                Spieler spieler = new Spieler(txtName.Text, txtMail.Text, pwdPasswort.Text);
                if (spieler.Registrieren())
                {
                    spielerList.Add(spieler);
                }

                //StreamWriter sw = File.AppendText(path);
                //sw.WriteLine(spieler.HoleRegistrierdaten());
                //sw.Flush();
                //sw.Close();

                tbkMessage.Text = spielerList.Count.ToString() + " Spieler sind angelegt.";

                LeereFormular();
            }
            else
            {
                // Bringe eine Fehlermeldung, dass Mail-Adresse ungültig ist
                tbkMessage.Foreground = Brushes.Red;
                tbkMessage.Text = "Mail ist ungültig!";
            }

            SpielerListeAktualisieren();
        }

        private void cmdEinloggen_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtName.Text) &&
                !String.IsNullOrEmpty(pwdPasswort.Text))
            {
                PruefeEinloggen(txtName.Text, pwdPasswort.Text);
                LeereFormular();
            }
            else if (!String.IsNullOrEmpty(txtMail.Text) &&
                !String.IsNullOrEmpty(pwdPasswort.Text))
            {
                PruefeEinloggen(txtMail.Text, pwdPasswort.Text);
                LeereFormular();
            }
            else
            {
                // Bringe eine Fehlermeldung, dass Mail-Adresse oder Spielername fehlt
                tbkMessage.Foreground = Brushes.Red;
                tbkMessage.Text = "Spielername oder Mailadresse fehlen!";
            }
        }

        private void LeereFormular()
        {
            txtMail.Text = "";
            txtName.Text = "";
            pwdPasswort.Text = "";
        }

        private void PruefeEinloggen(string name_mail, string passwort)
        {
            foreach (Spieler sp in spielerList)
            {
                if (sp.Spielername == name_mail || sp.Mail == name_mail)
                {
                    if (sp.Einloggen(name_mail, passwort))
                    {
                        UserInterface user = new UserInterface(sp);
                        user.Show();
                    }

                    continue;
                }
            }
        }
    }
}
