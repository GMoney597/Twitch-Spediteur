using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Twitch_Spediteur
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Spieler> spielerList = new List<Spieler>();
        static string path = @"e:\projects\twitch.csv";
        FileStream filestream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        public MainWindow()
        {
            InitializeComponent();
            InitializeSpielerliste(spielerList);
        }

        private void InitializeSpielerliste(List<Spieler> spielerList)
        {
            StreamReader sr = new StreamReader(filestream);

            while (!sr.EndOfStream)
            {
                string[] player = sr.ReadLine().Split(';');
                Spieler spieler = new Spieler(player[0], player[1], player[2]);
                spielerList.Add(spieler);
            }

            sr.Close();
            filestream.Close();

            tbkMessage.Text = spielerList.Count.ToString() + " Spieler bereits registriert.";
            
            lstSpieler.Items.Clear();
            lstSpieler.ItemsSource = spielerList;
        }

        private void cmdRegistrieren_Click(object sender, RoutedEventArgs e)
        {
            tbkMessage.Text = "";

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            // Prüfe, ob alle Felder ausgefüllt wurden
            if(String.IsNullOrEmpty(txtName.Text) || 
                String.IsNullOrEmpty(txtMail.Text) || 
                String.IsNullOrEmpty(pwdPasswort.Password))
            {
                tbkMessage.Foreground = Brushes.Red;
                tbkMessage.Text = "Keine Daten angebeben.";
            }
            // Prüfe, ob die angegebene Mail-Adresse gültig ist
            else if (regex.Match(txtMail.Text).Success)
            {
                tbkMessage.Foreground= Brushes.Black;

                // Lege einen neuen Spieler an
                Spieler spieler = new Spieler(txtName.Text, txtMail.Text, pwdPasswort.Password);
                spielerList.Add(spieler);

                StreamWriter sw = File.AppendText(path);
                sw.WriteLine(txtName.Text + ';' + txtMail.Text + ';' + pwdPasswort.Password);
                sw.Flush();
                sw.Close();

                tbkMessage.Text = spielerList.Count.ToString() + " Spieler sind angelegt.";

                txtName.Text = "";
                txtMail.Text = "";
                pwdPasswort.Password = "";
            }
            else
            {
                // Bringe eine Fehlermeldung, dass Mail-Adresse ungültig ist
                tbkMessage.Foreground = Brushes.Red;
                tbkMessage.Text = "Mail ist ungültig!";
            }
        }
    }
}

/* Regex-Expression RFC532
 * \A(?:[a-z0-9!#$%&'*+/=?^_‘{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_‘{|}~-]+)*
 |  "(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]
      |  \\[\x01-\x09\x0b\x0c\x0e-\x7f])*")
@ (?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?
  |  \[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}
       (?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:
          (?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]
          |  \\[\x01-\x09\x0b\x0c\x0e-\x7f])+)
     \])\z
*/

