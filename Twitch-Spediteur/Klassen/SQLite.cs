﻿using System;
using System.Collections;
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
        Random random = new Random();

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

        internal void AktualisiereAuftrag(Auftrag gewaehlterAuftrag, Fahrzeug gewaehltesFahrzeug)
        {
            //Auftrag.FahrzeugID = ((Fahrzeug)cboFahrzeuge.SelectedItem).ID;
            //Auftrag.AendereAusfuehrung(Angebot.Status.Abholung);
            //((Fahrzeug)cboFahrzeuge.SelectedItem).GebeAuftrag(Auftrag.Auftragsnummer);

            sqlCom.CommandText = "UPDATE t_Auftraege SET Status = @status, " +
                "Fahrzeug_ID = @fzgID, AuftragsStart = @startZeit " +
                "WHERE A_ID = @auftragsNummer";
            sqlCom.Parameters.AddWithValue("@status", Auftrag.Status.Abholung);
            sqlCom.Parameters.AddWithValue("@fzgID", gewaehltesFahrzeug.ID);
            sqlCom.Parameters.AddWithValue("@startZeit", DateTime.Now.ToString());
            sqlCom.Parameters.AddWithValue("@auftragsNummer", gewaehlterAuftrag.Auftragsnummer);

            SQLiteCommand sqlCom2 = new SQLiteCommand(sqlCon);
            sqlCom2.CommandText = "UPDATE t_Fahrzeuge SET HatAuftrag = 1, Auftragsnummer = @aID";
            sqlCom2.Parameters.AddWithValue("@aID", gewaehlterAuftrag.Auftragsnummer);

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();
                sqlCom2.ExecuteNonQuery();
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

        internal void HoleRouten(List<Entfernung> routen)
        {
            sqlCom.CommandText = "SELECT * FROM t_Routen";
            sqlDA.SelectCommand = sqlCom;

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();
                sqlDA.Fill(dtaTemp = new DataTable());
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
                string[] routenOrte = row.ItemArray[1].ToString().Split('-');
                int start = random.Next(routenOrte.Length);
                string abhol = routenOrte[start];
                string end = routenOrte[routenOrte.Length - start - 1];
                routen.Add(new Entfernung(abhol, end, abhol + " ==> " + end, Convert.ToInt32(row.ItemArray[2])));
            }
        }

        internal void HoleFuhrpark(Spieler spieler)
        {
            sqlCom.CommandText = "SELECT * FROM t_Fahrzeuge WHERE Spieler_ID = @sid";
            sqlCom.Parameters.AddWithValue("@sid", spieler.ID);
            sqlDA.SelectCommand = sqlCom;

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();
                sqlDA.Fill(dtaTemp = new DataTable());
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
                // 
                spieler.Fuhrpark.Add(new Fahrzeug(Convert.ToInt32(row.ItemArray[0]), "Kombi", 0.5m, 60, 400, 4000, 6,
                    Convert.ToBoolean(row.ItemArray[3]), DateTime.Parse(row.ItemArray[4].ToString()),
                    DateTime.Parse(row.ItemArray[5].ToString()), row.ItemArray[6].ToString(),
                    Convert.ToBoolean(row.ItemArray[7]), Convert.ToInt32(row.ItemArray[8])));
            }
        }

        internal void SpeichereAuftrag(Auftrag auftrag, int spielerID)
        {
            sqlCom.CommandText = "INSERT INTO t_Auftraege (Abhol, Liefer, Entfernung, Bezeichnung, Wert, Status, Spieler_ID) " +
                "VALUES (@abhol, @liefer, @entf, @bez, @wert, @status, @spID)";
            sqlCom.Parameters.AddWithValue("@abhol", auftrag.Abholort);
            sqlCom.Parameters.AddWithValue("@liefer", auftrag.Lieferort);
            sqlCom.Parameters.AddWithValue("@entf", auftrag.Entfernung);
            sqlCom.Parameters.AddWithValue("@bez", auftrag.Bezeichnung);
            sqlCom.Parameters.AddWithValue("@wert", auftrag.Auftragssumme);
            sqlCom.Parameters.AddWithValue("@status", auftrag.Zustand);
            sqlCom.Parameters.AddWithValue("@spID", spielerID);

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

        internal void HoleOrte(List<Ort> orte)
        {
            sqlCom.CommandText = "SELECT * FROM t_Orte";
            sqlDA.SelectCommand = sqlCom;

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();
                sqlDA.Fill(dtaTemp = new DataTable());
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
                orte.Add(new Ort(row.ItemArray[1].ToString()));
            }
        }

        internal void HoleAuftraege(Spieler sp)
        {
            Spieler spieler = sp;

            sqlCom.CommandText = "SELECT * FROM t_Auftraege";
            sqlDA.SelectCommand = sqlCom;

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();
                sqlDA.Fill(dtaTemp = new DataTable());
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                sqlCon.Close();
            }

            // Werte: ID, Startort, Zielort, Entfernung, Bezeichnung, Wert, Status, SpielerID, FahrzeugID, AuftragsDatum
            foreach (DataRow row in dtaTemp.Rows)
            {
                if (row.ItemArray[9] == DBNull.Value)
                {
                    sp.Auftraege.Add(new Auftrag(Convert.ToInt32(row.ItemArray[0]), row.ItemArray[1].ToString(), row.ItemArray[2].ToString(),
                        Convert.ToInt32(row.ItemArray[3]), row.ItemArray[4].ToString(), Convert.ToDecimal(row.ItemArray[5]),
                        (Auftrag.Status)Convert.ToInt16(row.ItemArray[6]), Convert.ToInt32(row.ItemArray[7]),
                        Convert.ToInt32(row.ItemArray[8])));
                }
                else
                {
                    sp.Auftraege.Add(new Auftrag(Convert.ToInt32(row.ItemArray[0]), row.ItemArray[1].ToString(), row.ItemArray[2].ToString(),
                        Convert.ToInt32(row.ItemArray[3]), row.ItemArray[4].ToString(), Convert.ToDecimal(row.ItemArray[5]),
                        (Auftrag.Status)Convert.ToInt16(row.ItemArray[6]), Convert.ToInt32(row.ItemArray[7]),
                        Convert.ToInt32(row.ItemArray[8]), DateTime.Parse(row.ItemArray[9].ToString())));
                }
            }
        }

        internal void SpeichereEntfernung(string route, int distanz)
        {
            sqlCom.CommandText = "INSERT INTO t_Route (Route, Distanz) " +
                "VALUES (@route, @dist)";
            sqlCom.Parameters.AddWithValue(@"route", route);
            sqlCom.Parameters.AddWithValue(@"dist", distanz);

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

        internal void HoleWaren(List<Ware> waren)
        {
            sqlCom.CommandText = "SELECT * FROM t_Waren";
            sqlDA.SelectCommand = sqlCom;

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();
                sqlDA.Fill(dtaTemp = new DataTable());
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
            sqlCom.CommandText = ("SELECT S_ID, Spielername, Mail, Bargeld, Kontostand, Startort FROM t_Spieler");
            sqlDA.SelectCommand = sqlCom;
            sqlDA.Fill(dtaTemp = new DataTable());

            List<Spieler> list = new List<Spieler>();

            foreach (DataRow dr in dtaTemp.Rows)
            {
                list.Add(new Spieler(Convert.ToInt16(dr[0]), dr.ItemArray[1].ToString(), dr.ItemArray[2].ToString(),
                    Convert.ToDecimal(dr.ItemArray[3]), Convert.ToDecimal(dr.ItemArray[4]),
                    dr.ItemArray[5].ToString()));
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
                    sqlDA.Fill(dtaTemp = new DataTable());

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
