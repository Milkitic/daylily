using DaylilyWeb.Assist;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Database
{
    public class DbHelper
    {
        public static Dictionary<string, string> ConnectionString { get; set; } = new Dictionary<string, string>();

        string currentString;
        public DbHelper(string connectionName)
        {
            currentString = ConnectionString[connectionName];
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(currentString);
        }

        public DataTable FillTable(string queryString, params MySqlParameter[] param)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command = GetParamCommand(queryString, connection, param);

                MySqlDataAdapter mysqlDataAdapter = new MySqlDataAdapter(command);
                mysqlDataAdapter.Fill(dataSet);
                dataTable = dataSet.Tables[0];
            }
            return dataTable;
        }

        //public List<T> FillList<T>(string queryString, params MySqlParameter[] param)
        //{
        //    var list_t = new List<T>();

        //    DataSet dataSet = new DataSet();
        //    DataTable dataTable = new DataTable();
        //    using (MySqlConnection connection = GetConnection())
        //    {
        //        connection.Open();

        //        MySqlCommand command = GetParamCommand(queryString, connection, param);

        //        MySqlDataAdapter mysqlDataAdapter = new MySqlDataAdapter(command);
        //        mysqlDataAdapter.Fill(dataSet);
        //        dataTable = dataSet.Tables[0];
        //        MySqlDataReader objSqlReader = command.ExecuteReader();
        //        foreach (var dataRow in dataTable.Rows)
        //        {
        //            list_t.Add((T)new { dataRow });
        //        }
        //        while (objSqlReader.Read())
        //        {
        //            list_t.Add(new { 234, 234 });
        //        }
        //    }
        //    return list_t;
        //}

        public int ExecuteNonQuery(string queryString, params MySqlParameter[] param)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand command = GetParamCommand(queryString, connection, param);

                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.DangerLine("Failed: " + ex.Message);
                    return -1;
                }
            }
        }

        private static MySqlCommand GetParamCommand(string queryString, MySqlConnection connection, params MySqlParameter[] param)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = queryString;
            if (param != null || param.Length > 0)
            {
                foreach (var item in param)
                {
                    command.Parameters.Add(item);
                }
            }

            return command;
        }
    }
}
