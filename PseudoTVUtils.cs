using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NLog;
using MySql.Data;

namespace PseudoTV_Manager
{
    public enum KodiVersion
    {
        Gotham = 13,
        Helix = 14,
        Isengard = 15,
        Jarvis = 16
    }

    public static class PseudoTvUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static object GetKodiVersion(string videoDbFileName)
        {
            Regex regex = new Regex("(MyVideos)(\\d+).db");
            Match match = regex.Match(videoDbFileName);
            if (match.Success)
            {
                int dbNumber = Convert.ToInt32(match.Groups[2].Value);
                if ((dbNumber < 90))
                {
                    return KodiVersion.Gotham;
                }
                else if ((dbNumber < 91))
                {
                    return KodiVersion.Helix;
                }
                else if ((dbNumber < 94))
                {
                    return KodiVersion.Isengard;
                }
                else if ((dbNumber > 93))
                {
                    return KodiVersion.Jarvis;
                }
            }
            return KodiVersion.Gotham;
        }

        public static string ReadFile(string filePath)
        {
            //Reads the file and returns it back as a string variable.
            string fileText = File.ReadAllText(filePath);
            return fileText;
        }


        public static void SaveFile(string filePath, string Data)
        {
            if (System.IO.File.Exists(filePath) == true)
            {
                System.IO.StreamWriter objWriter2 = new System.IO.StreamWriter(filePath);
                objWriter2.Write(Data);
                objWriter2.Dispose();
                objWriter2.Close();
            }

        }

        public static string[] ReadPluginRecord(string dbLocation, string sqlStatement, int[] columnArray)
        {
            //Connect to the data-base

            string PluginDatabaseData = "Data Source=" + MainWindow.AddonDatabaseLocation;

            string[] ArrayResponse = { "" };
            //This is a standard, SQLite database.
            var sqlConnect = new SQLiteConnection();
            SQLiteCommand sqlCommand = null;
            sqlConnect.ConnectionString = "Data Source=" + MainWindow.AddonDatabaseLocation;

            try
            {
                sqlConnect.Open();
                sqlCommand = sqlConnect.CreateCommand();
                sqlCommand.CommandText = sqlStatement;
                var sqlReader = sqlCommand.ExecuteReader();

                int x = 0;

                while (sqlReader.Read())
                {
                    Array.Resize(ref ArrayResponse, x + 1);
                    var stringResponse = "";


                    for (var y = 0; y <= columnArray.Length; y++)
                    {
                        if (y > 0)
                        {
                            stringResponse = stringResponse + "~" + sqlReader[columnArray[y]];
                        }
                        else
                        {
                            stringResponse = sqlReader[columnArray[y]].ToString();
                        }
                    }

                    ArrayResponse[x] = stringResponse;

                    x = x + 1;

                }

            }
            catch (SQLiteException myerror)
            {
                Logger.Error(myerror.Message);
                MessageBox.Show("Error Connecting to Database: " + myerror.Message);

            }
            finally
            {
                sqlCommand.Dispose();
                sqlConnect.Close();

            }

            return ArrayResponse;
        }


        public static void PluginExecute(string SQLQuery)
        {
            string PluginDatabaseData = "Data Source=" + MainWindow.AddonDatabaseLocation;

            //These are standard SQLite databases.
            //Open the connection.
            SQLiteConnection sqlConnect = new SQLiteConnection();
            SQLiteCommand sqlCommand = null;

            sqlConnect.ConnectionString = "Data Source=" + MainWindow.AddonDatabaseLocation;

            try
            {
                sqlConnect.Open();

                //Set the command.
                sqlCommand = sqlConnect.CreateCommand();

                //Execute the command.
                sqlCommand.CommandText = SQLQuery;
                sqlCommand.ExecuteNonQuery();

            }
            catch (SQLiteException myerror)
            {
                Logger.Error(myerror.Message);
                MessageBox.Show("Error Connecting to Database: " + myerror.Message);
            }
            finally
            {
                //Dispose of and close the connection.
                sqlCommand.Dispose();
                sqlConnect.Close();
            }

        }

        public static string[] DbReadRecord(string dbLocation, string sqlStatement, int[] ColumnArray)
        {
            //Connect to the data-base

            //Dim VideoDatabaseData As String = "Data Source=" & MainWindow.VideoDatabaseLocation
            //Dim PluginDatabaseData As String = "Data Source=" & MainWindow.AddonDatabaseLocation

            string[] arrayResponse = { "" };


            if (MainWindow.DatabaseType == 0)
            {
                //This is a standard, SQLite database.
                SQLiteConnection sqlConnect = new SQLiteConnection();
                SQLiteCommand sqlCommand = null;
                sqlConnect.ConnectionString = "Data Source=" + MainWindow.VideoDatabaseLocation;

                try
                {
                    sqlConnect.Open();
                    sqlCommand = sqlConnect.CreateCommand();
                    sqlCommand.CommandText = sqlStatement;
                    var sqlReader = sqlCommand.ExecuteReader();

                    var x = 0;

                    while (sqlReader.Read())
                    {
                        Array.Resize(ref arrayResponse, x + 1);
                        string StringResponse = "";


                        for (var y = 0; y <= ColumnArray.Length; y++)
                        {
                            if (y > 0)
                            {
                                StringResponse = StringResponse + "~" + sqlReader[ColumnArray[y]];
                            }
                            else
                            {
                                StringResponse = sqlReader[ColumnArray[y]].ToString();
                            }
                        }

                        arrayResponse[x] = StringResponse;

                        x = x + 1;

                    }

                }
                catch (MySqlException myerror)
                {
                    Logger.Error(myerror.Message);
                    MessageBox.Show("Error Connecting to Database: " + myerror.Message);

                }
                finally
                {
                    sqlCommand.Dispose();
                    sqlConnect.Close();

                }

            }
            else
            {
                //This is for MySQL Databases, just slight syntax differences.
                var sqlConnect = new MySqlConnection();
                MySqlCommand sqlCommand = null;
                sqlConnect.ConnectionString = MainWindow.MySQLConnectionString;
                try
                {
                    sqlConnect.Open();
                    sqlCommand = sqlConnect.CreateCommand();
                    sqlCommand.CommandText = sqlStatement;
                    MySqlDataReader sqlReader = sqlCommand.ExecuteReader();

                    int x = 0;

                    while (sqlReader.Read())
                    {
                        Array.Resize(ref arrayResponse, x + 1);
                        string StringResponse = "";


                        for (var y = 0; y <= ColumnArray.Length; y++)
                        {
                            if (y > 0)
                            {
                                StringResponse = StringResponse + "~" + sqlReader[ColumnArray[y]];
                            }
                            else
                            {
                                StringResponse = sqlReader[ColumnArray[y]].ToString();
                            }
                        }

                        arrayResponse[x] = StringResponse;

                        x = x + 1;

                    }

                }
                catch (MySqlException myerror)
                {
                    Logger.Error(myerror.Message);
                    MessageBox.Show("Error Connecting to Database: " + myerror.Message);
                }
                finally
                {
                    sqlCommand.Dispose();
                    sqlConnect.Close();

                }

            }


            return arrayResponse;

        }


        public static void DbExecute(string SQLQuery)
        {
            if (MainWindow.DatabaseType == 0)
            {
                //These are standard SQLite databases.
                //Open the connection.
                SQLiteConnection sqlConnect = new SQLiteConnection();
                SQLiteCommand sqlCommand = null;
                sqlConnect.ConnectionString = "Data Source=" + MainWindow.VideoDatabaseLocation;

                try
                {
                    sqlConnect.Open();

                    //Set the command.
                    sqlCommand = sqlConnect.CreateCommand();

                    //Execute the command.
                    sqlCommand.CommandText = SQLQuery;
                    sqlCommand.ExecuteNonQuery();

                }
                catch (MySqlException myerror)
                {
                    Logger.Error(myerror.Message);
                    MessageBox.Show("Error Connecting to Database: " + myerror.Message);
                }
                finally
                {
                    //Dispose of and close the connection.
                    sqlCommand.Dispose();
                    sqlConnect.Close();
                }

            }
            else
            {
                //These are for MYSQL connections
                //Just anything that says SQLite is changed to MySQL .. no biggy.

                //Open the connection.
                MySqlConnection sqlConnect = new MySqlConnection();
                MySqlCommand sqlCommand = null;
                sqlConnect.ConnectionString = MainWindow.MySQLConnectionString;
                try
                {
                    sqlConnect.Open();

                    //Set the command.
                    sqlCommand = sqlConnect.CreateCommand();

                    //Execute the command.
                    sqlCommand.CommandText = SQLQuery;
                    sqlCommand.ExecuteNonQuery();
                }
                catch (MySqlException myerror)
                {
                    Logger.Error(myerror.Message);
                    MessageBox.Show("Error Connecting to Database: " + myerror.Message);
                    //Finally
                    //Dispose of and close the connection.
                }
                finally
                {
                    sqlCommand.Dispose();
                    sqlConnect.Close();

                }

            }
        }

        public static object TestMYSQL(string connectionstring)
        {

            bool ConnectSuccessful = false;

            dynamic conn = new MySqlConnection();
            //Dim sqlCommand As MySqlCommand
            conn.ConnectionString = connectionstring;
            try
            {
                conn.Open();
                ConnectSuccessful = true;
                MessageBox.Show("Successfully connected to database.");
                conn.Close();
            }
            catch (MySqlException myerror)
            {
                Logger.Error(myerror.Message);
                MessageBox.Show("Error Connecting to Database: " + myerror.Message);
            }
            finally
            {
                conn.Dispose();
            }

            return ConnectSuccessful;
        }

        public static object TestMYSQLite(string connectionstring)
        {
            var ConnectSuccessful = false;

            var conn = new SQLiteConnection();
            //Dim sqlCommand As MySqlCommand
            conn.ConnectionString = "Data Source=" + connectionstring;
            try
            {
                conn.Open();
                ConnectSuccessful = true;
                MessageBox.Show("Successfully connected to database.");
                conn.Close();
            }
            catch (MySqlException myerror)
            {
                Logger.Error(myerror.Message);
                MessageBox.Show("Error Connecting to Database: " + myerror.Message);
            }
            finally
            {
                conn.Dispose();
            }

            return ConnectSuccessful;
        }

        public static object testMysql2(string DBLocation, string sqlStatement, string[] ColumnArray)
        {
            //Connect to the data-base
            MySqlConnection sqlConnect = new MySqlConnection();
            MySqlCommand sqlCommand = default(MySqlCommand);
            sqlConnect.ConnectionString = "server=" + "127.0.0.1" + ";" + "user id=" + "xbmc" + ";" + "password=" +
                                          "xbmc" + ";" + "database=xbmcvideo60";
            sqlConnect.Open();
            sqlCommand = sqlConnect.CreateCommand();
            sqlCommand.CommandText = sqlStatement;
            MySqlDataReader sqlReader = sqlCommand.ExecuteReader();

            int x = 0;

            string[] ArrayResponse = null;
            while (sqlReader.Read())
            {
                Array.Resize(ref ArrayResponse, x + 1);
                string StringResponse = "";


                for (var y = 0; y <= ColumnArray.Length; y++)
                {
                    if (y > 0)
                    {
                        StringResponse = StringResponse + "~" + sqlReader[ColumnArray[y]];
                    }
                    else
                    {
                        StringResponse = sqlReader[ColumnArray[y]].ToString();
                    }
                }

                ArrayResponse[x] = StringResponse;

                x = x + 1;

            }
            sqlCommand.Dispose();
            sqlConnect.Close();


            return ArrayResponse;
        }

        public static bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }
    }


}