using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Twitch_Spediteur.Fenster;
using Twitch_Spediteur.Klassen;

namespace Twitch_Spediteur
{
    internal class SQLite
    {
        static SQLiteConnection sqlCon = new SQLiteConnection(@"Data Source = e:\projects\twitch.db; Version=3;FailIfMissing=True", true);
        static SQLiteCommand sqlCom = new SQLiteCommand(sqlCon);
        static SQLiteDataAdapter sqlDA = new SQLiteDataAdapter();
        DataTable dtaTemp = new DataTable();
        SHA256 sha256 = SHA256.Create();

        public SQLite()
        {

        }

        internal bool RegistriereSpieler(Spieler spieler)
        {
            bool result = false;

            sqlCom.CommandText = ("INSERT INTO t_Spieler (Spielername, Mail, Passwort, Bargeld, Kontostand) " +
                "VALUES (@name, @mail, @pass, @bar, @konto)");
            sqlCom.Parameters.AddWithValue("@name", spieler.Spielername);
            sqlCom.Parameters.AddWithValue("@mail", spieler.Mail);
            sqlCom.Parameters.AddWithValue("@pass", spieler.Passwort);
            sqlCom.Parameters.AddWithValue("@bar", spieler.Bargeld);
            sqlCom.Parameters.AddWithValue("@konto", spieler.Konto);

            try
            {
                sqlCon.Open();
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCom.ExecuteNonQuery();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }

            return result;
        }

        internal void SpeichereEntfernung(string start, string ziel, int distanz)
        {
            int row = 0;

            SQLiteCommand sqlCom2 = new SQLiteCommand(sqlCon);
            sqlCom2.CommandText = "SELECT SZ_Bezeichnung FROM t_Entfernungen WHERE SZ_Bezeichnung = @sz OR SZ_Bezeichnung = @zs";
            sqlCom2.Parameters.AddWithValue(@"sz", start+ziel);
            sqlCom2.Parameters.AddWithValue(@"zs", ziel+start);

                sqlCom.CommandText = "INSERT INTO t_Entfernungen (Start, Ziel, Distanz, SZ_Bezeichnung) " +
                    "VALUES (@start, @ziel, @dist, @sz)";
                sqlCom.Parameters.AddWithValue(@"start", start);
                sqlCom.Parameters.AddWithValue(@"ziel", ziel);
                sqlCom.Parameters.AddWithValue(@"dist", distanz);
                sqlCom.Parameters.AddWithValue(@"sz", start + ziel);

            try
            {
                sqlCon.Open();
                row = Convert.ToInt32(sqlCom2.ExecuteNonQuery());
                if (row <= 1)
                {
                    sqlCom.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }
        }

        internal List<Ware> HoleWaren()
        {
            List<Ware> waren = new List<Ware>();

            sqlCom.CommandText = "SELECT * FROM t_Waren";
            sqlDA.SelectCommand = sqlCom;

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();
                sqlDA.Fill(dtaTemp);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }

            foreach (DataRow row in dtaTemp.Rows)
            {
                waren.Add(new Ware(row.ItemArray[1].ToString(), (Ware.Verladung)Convert.ToInt16(row.ItemArray[2]),
                    Convert.ToDecimal(row.ItemArray[3]), (Ware.Einheit)Convert.ToInt16(row.ItemArray[4]),
                    (Ware.Merkmal)Convert.ToInt16(row.ItemArray[5])));
            }

            return waren;
        }

        internal void BargeldUpdate(string spielername, decimal bargeld)
        {
            sqlCom.CommandText = "UPDATE t_Spieler SET Bargeld = @bar WHERE Spielername = @spieler";
            sqlCom.Parameters.AddWithValue("@bar", bargeld);
            sqlCom.Parameters.AddWithValue("@spieler", spielername);

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }
        }

        internal void ParkeFahrzeug(string spielername, Fahrzeug temp)
        {
            SQLiteCommand sqlCom2 = new SQLiteCommand(sqlCon);
            sqlCom2.CommandText = "SELECT S_ID FROM t_Spieler WHERE Spielername = @spieler";
            sqlCom2.Parameters.AddWithValue("@spieler", spielername);
            int rowID = -1;

            sqlCom.CommandText = "INSERT INTO t_Fahrzeuge (Bezeichnung, Spieler_ID, IsGekauft, Erwerbdatum, Abgabedatum) " +
                "VALUES (@bez, @spieler, @gekauft, @erwerb, @abgabe)";
            sqlCom.Parameters.AddWithValue("@bez", temp.Typ);
            //sqlCom.Parameters.AddWithValue("@spieler", rowID);
            sqlCom.Parameters.AddWithValue("@gekauft", temp.IsGekauft);
            sqlCom.Parameters.AddWithValue("@erwerb", temp.AktionsDatum);
            if (!temp.IsGekauft)
            {
                sqlCom.Parameters.AddWithValue("@abgabe", temp.AktionsDatum.AddDays(7));
            }

            try
            {
                sqlCon.Open();
                rowID = Convert.ToInt16(sqlCom2.ExecuteScalar());
                sqlCom.Parameters.AddWithValue("@spieler", rowID);
                sqlCom.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }
        }

        internal bool SpeichereWare(Ware temp)
        {
            bool result = false;

            sqlCom.CommandText = ("INSERT INTO t_Waren (W_Bezeichnung, W_Verladung, W_Preis, W_Einheit, W_Merkmal) " +
                "VALUES (@bez, @verl, @preis, @ein, @merk)");
            sqlCom.Parameters.AddWithValue("@bez", temp.Bezeichnung);
            sqlCom.Parameters.AddWithValue("@verl", (int)temp.Ladung);
            sqlCom.Parameters.AddWithValue("@preis", temp.Preis);
            sqlCom.Parameters.AddWithValue("@ein", (int)temp.BasisEinheit);
            sqlCom.Parameters.AddWithValue("@merk", (int)temp.TransportMerkmal);

            try
            {
                sqlCon.Open();
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCom.ExecuteNonQuery();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }

            return result;
        }

        internal bool SpeichereStartort(Spieler sp, string ort)
        {
            bool result = false;

            sqlCom.CommandText = ("UPDATE t_Spieler SET Startort = @ort WHERE Spielername = @spieler");
            sqlCom.Parameters.AddWithValue("@ort", ort);
            sqlCom.Parameters.AddWithValue("@spieler", sp.Spielername.ToString());

            try
            {
                sqlCon.Open();
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCom.ExecuteNonQuery();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }

            return result;
        }

        internal List<Spieler> HoleSpieler()
        {
            sqlCom.CommandText = ("SELECT Spielername, Mail, Bargeld, Kontostand, Startort FROM t_Spieler");
            sqlDA.SelectCommand = sqlCom;
            sqlDA.Fill(dtaTemp);

            List<Spieler> list = new List<Spieler>();

            foreach (DataRow dr in dtaTemp.Rows)
            {
                list.Add(new Spieler(dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString(), 
                    Convert.ToDecimal(dr.ItemArray[2]), Convert.ToDecimal(dr.ItemArray[3]), 
                    dr.ItemArray[4].ToString()));
            }

            return list;
        }

        internal bool EinloggenSpieler(string name_mail, string passwort)
        {
            bool result = false;

            byte[] pwValid = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwort));
            string pwComp = Convert.ToBase64String(pwValid);

            sqlCom.CommandText = ("SELECT * FROM t_Spieler WHERE Spielername = @name OR Mail = @mail");
            sqlCom.Parameters.AddWithValue("@name", name_mail);
            sqlCom.Parameters.AddWithValue("@mail", name_mail);

            try
            {
                sqlCon.Open();
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlDA.SelectCommand = sqlCom;
                    sqlDA.Fill(dtaTemp);

                    if (dtaTemp.Rows[0].ItemArray[3].ToString() == pwComp)
                    {
                       result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }

            return result;
        }
    }
}
