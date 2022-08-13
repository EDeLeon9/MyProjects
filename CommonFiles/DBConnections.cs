using System;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;

namespace DBConnections
{
    public class DBConnection
    {
        public const string DB_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public static List<DBConnection> GlobalConnections { get; set; } = new List<DBConnection>();  //Listado auxiliar para almacenar conexiones y poder usarlas en cualquier parte del programa

        private string connString = "";
        private MySqlConnection conn;
        private DataTable data = new DataTable();
        private bool fillData = false;

        public bool ManualConnection { get; set; } = false;
        public bool UseOfCALL { get; set; } = true;


        public DBConnection()
        {
        }


        public DBConnection(string ip, string dataBase, string user, string password, int port = 3306)
        {
            ConnectionParams(ip, dataBase, user, password, port);
        }


        ~DBConnection()
        {
            Disconnect();
        }


        public void ConnectionParams(string ip, string dataBase, string user, string password, int port = 3306)
        {
            connString = $"Server={ip};Port={port};Database={dataBase};Uid={user};password={password}";
        }


        public bool Connect()
        {
            if (connString != "")
            {
                try
                {
                    Disconnect();
                    conn = new MySqlConnection(connString);
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    Disconnect();
                    MessageBox.Show(ex.Message, "Database Connection", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("Please provide the connection parameters before attempting to connect.", "Database Connection", MessageBoxButton.OK, MessageBoxImage.Error);

            return false;
        }


        public void Disconnect()
        {
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }


        public bool Execute(string command, params object[] parameters)
        {
            data = new DataTable();

            bool execute = false;
            if (!ManualConnection)
                execute = Connect();
            else if (conn == null)
                MessageBox.Show("Please establish a successful connection to a database before attempting to execute commands.", "Database Connection", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                execute = true;

            if (execute)
            {
                try
                {
                    MySqlCommand cmd;
                    if (UseOfCALL)
                    {
                        cmd = new MySqlCommand("CALL " + command + "({0})", conn);
                        if ((parameters?.Length ?? 0) > 0)  //Si parameters es null no se consultará Length y devolverá null, luego con ?? devolverá 0
                        {
                            for (int i = 0; i < parameters.Length; i++)
                                cmd.CommandText = string.Format(cmd.CommandText, $"{(parameters[i] == null ? "NULL" : parameters[i].GetType() == typeof(string) ? $"'{parameters[i]}'" : parameters[i].GetType() == typeof(DateTime) ? $"'{((DateTime)parameters[i]).ToString(DBConnection.DB_DATETIME_FORMAT, CultureInfo.InvariantCulture)}'" : parameters[i])}{(i + 1 < parameters.Length ? ", {0}" : "")}");
                        }
                        else cmd.CommandText = string.Format(cmd.CommandText, "");
                    }
                    else cmd = new MySqlCommand(command, conn);

                    if ((cmd.CommandText.ToUpper().Contains("DELETE ") || cmd.CommandText.ToUpper().Contains("UPDATE ")) && !cmd.CommandText.ToUpper().Contains("WHERE"))
                    {
                        MessageBox.Show($"Cannot execute {(cmd.CommandText.ToUpper().Contains("UPDATE") ? "UPDATE" : "DELETE")} without WHERE clausule", "Database Connection", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    if (fillData)
                        new MySqlDataAdapter(cmd).Fill(data);
                    else
                        cmd.ExecuteNonQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Connection", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (!ManualConnection)
                        Disconnect();
                }
            }
            return false;
        }


        public bool Execute<T>(string command, ref List<T> storageList, params object[] parameters)
        {
            fillData = true;
            bool executed = Execute(command, parameters);
            fillData = false;

            if (executed)
            {
                try
                {
                    List<string> columnNames = data.Columns.Cast<DataColumn>().Select(dataColumn => dataColumn.ColumnName).ToList();
                    //Los Value Types son los tipos básicos, excepto string. Los Value Types son diferentes a los Reference Types que son por ejemplo las clases.
                    if (typeof(T).IsValueType || typeof(T) == typeof(string))
                    {
                        storageList = data.AsEnumerable().Select(dataRow => dataRow[0] != DBNull.Value ? (T)(dataRow[0].GetType().Name == "Int64" ? Convert.ToInt32(dataRow[0]) : dataRow[0]) : default(T)).ToList();
                    }
                    else
                    {
                        PropertyInfo[] properties = typeof(T).GetProperties();  //Recordar: si es string sí va a traer muchas propiedades, por eso se separa con el if de arriba.
                        storageList = data.AsEnumerable().Select(dataRow =>
                        {
                            T storageObject = Activator.CreateInstance<T>();  //No deja hacer new T() (a menos que se coloque where T : new()). Pero en su lugar se usa Activator.CreateInstance<T>()
                            foreach (PropertyInfo prop in properties)
                            {
                                if (columnNames.Contains(prop.Name))
                                {
                                    //No se usa Convert.ChangeType() como en lo comentado ya que se debe estar establecido correctamente el tipo que retorna la BD para que no de error.
                                    //prop.SetValue(storageObject, dataRow[prop.Name] != DBNull.Value? Convert.ChangeType(dataRow[prop.Name], prop.PropertyType) : null);
                                    prop.SetValue(storageObject, dataRow[prop.Name] != DBNull.Value ? (dataRow[prop.Name].GetType().Name == "Int64" ? Convert.ToInt32(dataRow[prop.Name]) : dataRow[prop.Name]) : null);
                                }
                            }
                            return storageObject;
                        }).ToList();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Connection", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return false;
        }


        public bool Execute<T>(string command, ref T storage, params object[] parameters)
        {
            List<T> list = new List<T>();
            if (Execute<T>(command, ref list, parameters))
            {
                if (list.Count >= 1)
                    storage = list[0];
                return true;
            }
            return false;
        }
    }
}
