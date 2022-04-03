using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
            sqlCom.Parameters.AddWithValue("@pass", spieler.Passwort_neu);
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
            sqlCom.CommandText = ("SELECT Spielername, Mail, Bargeld, Kontostand, Fuhrpark, Nachrichten, Startort FROM t_Spieler");
            sqlDA.SelectCommand = sqlCom;
            sqlDA.Fill(dtaTemp);

            List<Spieler> list = new List<Spieler>();

            foreach (DataRow dr in dtaTemp.Rows)
            {
                list.Add(new Spieler(dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString(), 
                    Convert.ToDecimal(dr.ItemArray[2]), Convert.ToDecimal(dr.ItemArray[3]), 
                    Convert.ToInt16(dr.ItemArray[4]), Convert.ToInt16(dr.ItemArray[5]), dr.ItemArray[6].ToString()));
            }

            return list;
        }

        internal bool EinloggenSpieler(string name_mail, string passwort)
        {
            bool result = false;

            // byte[] pwValid = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwort));

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

                    if (dtaTemp.Rows[0].ItemArray[3].ToString() == passwort)
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
