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

namespace Twitch_Spediteur
{
    /// <summary>
    /// Interaktionslogik für UserInterface.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private Spieler sp;
        //FileStream file = new FileStream("")

        public UserWindow(Spieler spieler)
        {
            sp = spieler;
            InitializeComponent();
            InitializeOrteListe();
            PruefeSpielerStartort();

            tbkSpieler.Text = sp.Spielername;
            txtStandort.Text = sp.Startort;
            txtBargeld.Text = Convert.ToDecimal(sp.Bargeld) + " €";
            txtKontostand.Text = Convert.ToDecimal(sp.Konto) + " €";
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
                if (sql.SpeichereStartort(sp, cboOrte.SelectedValue.ToString()))
                {
                    stackOrtWaehlen.Visibility = Visibility.Collapsed;
                    txtStandort.Text = cboOrte.SelectedValue.ToString();
                }                
            }
        }
    }
}
