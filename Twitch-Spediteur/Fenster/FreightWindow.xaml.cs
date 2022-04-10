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
    /// Interaktionslogik für FreightWindow.xaml
    /// </summary>
    public partial class FreightWindow : Window
    {
        List<Fracht> frachten = new List<Fracht>();
        Spieler spieler;

        public FreightWindow(Spieler sp)
        {
            InitializeComponent();
            InitializeFrachtBoerse();

            spieler = sp;
        }

        private void InitializeFrachtBoerse()
        {
            Fracht fracht = new Fracht("München", "Leverkusen", "Magazine", 500, 2500);
            frachten.Add(fracht);

            dtgFracht.ItemsSource = frachten;
        }

        private void cmdAnnehmen_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgResult = MessageBox.Show("Du hast folgenden Auftrag angeklickt: " + 
                ((Fracht)dtgFracht.CurrentItem).Bezeichnung, "Fracht annehmen", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (msgResult == MessageBoxResult.Yes)
            {
                spieler.Auftraege.Add((Fracht)dtgFracht.CurrentItem);

                dtgFracht.ItemsSource = null;
                frachten.Remove((Fracht)dtgFracht.CurrentItem);
                dtgFracht.ItemsSource = frachten;
                //this.Close();
            }
        }
    }
}
