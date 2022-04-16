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
    public partial class EntfernungFenster : Window
    {
        List<Entfernung> distances = new List<Entfernung>();
        SQLite sql = new SQLite();

        public EntfernungFenster()
        {
            InitializeComponent();
        }

        private void cmdImport_Click(object sender, RoutedEventArgs e)
        {
            FileStream file = new FileStream(@".\ExterneDateien\Entfernungen.csv", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file, Encoding.UTF7);

            string[] staedte = sr.ReadLine().Split(';');

            while (!sr.EndOfStream)
            {
                string route = "";
                string[] zeile = sr.ReadLine().Split(';');

                for (int i = 1; i < staedte.Length; i++)
                {
                    // Nullentfernungen übergehen
                    if (!String.IsNullOrEmpty(zeile[i]))
                    {
                        route = zeile[0] + "-" + staedte[i];

                        distances.Add(new Entfernung(zeile[0], staedte[i], route, Convert.ToInt32(zeile[i])));
                        sql.SpeichereEntfernung(route, Convert.ToInt32(zeile[i]));
                    }
                }
            }

            dtgEntfernungen.ItemsSource = null;
            dtgEntfernungen.Items.Clear();
            dtgEntfernungen.ItemsSource = distances;
        }
    }
}
