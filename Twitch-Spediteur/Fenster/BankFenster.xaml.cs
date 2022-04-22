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

namespace Twitch_Spediteur.Fenster
{
    /// <summary>
    /// Interaktionslogik für BankFenster.xaml
    /// </summary>
    public partial class BankFenster : Window
    {
        Spieler spieler;

        public BankFenster(Spieler sp)
        {
            InitializeComponent();
            spieler = sp;

            InitializeTransferSlider();
        }

        private void InitializeTransferSlider()
        {
            sliTransfer.Minimum = 0;
            sliTransfer.Maximum = (Double)spieler.Konto;

            txtBargeld.Text = String.Format("{0:C2}", spieler.Bargeld);
            txtKonto.Text = String.Format("{0:C2}", spieler.Konto);
        }

        private void cmdSpeichern_Click(object sender, RoutedEventArgs e)
        {
            spieler.GeldTransaktion(-(Convert.ToDecimal(sliTransfer.Value)));
            spieler.KontoTransaktion(-(Convert.ToDecimal(sliTransfer.Value)));

            txtBargeld.Text = String.Format("{0:C2}", spieler.Bargeld);
            txtKonto.Text = String.Format("{0:C2}", spieler.Konto);
        }

        private void cmdSchließen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
