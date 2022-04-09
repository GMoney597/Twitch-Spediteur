using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaktionslogik für Entfernungen.xaml
    /// </summary>
    public partial class Entfernungen : Window
    {
        List<Entfernung> distances = new List<Entfernung>();

        public Entfernungen()
        {
            InitializeComponent();
        }

        private void cmdImport_Click(object sender, RoutedEventArgs e)
        {
            FileStream file = new FileStream(@".\ExterneDateien\Entfernungen.csv", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            string[] staedte = sr.ReadLine().Split(';');

            while (!sr.EndOfStream)
            {
                string[] zeile = sr.ReadLine().Split(';');

                for (int i = 1; i < staedte.Length; i++)
                {
                    int dist = 0;

                    if (!String.IsNullOrEmpty(zeile[i]))
                    {
                        dist = Convert.ToInt32(zeile[i]); 
                    }

                    distances.Add(new Entfernung(zeile[0], staedte[i], dist));
                }
            }

            dtgEntfernungen.ItemsSource = null;
            dtgEntfernungen.Items.Clear();
            dtgEntfernungen.ItemsSource = distances;
        }
    }
}
