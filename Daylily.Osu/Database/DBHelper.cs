using System;
using System.Collections.Generic;
using System.Data;
using Daylily.Common.Utils.LoggerUtils;
using MySql.Data.MySqlClient;

namespace Daylily.Osu.Database
{
    public class DbHelper
    {
        public static Dictionary<string, string> ConnectionString { get; set; } = new Dictionary<string, string>();

        private readonly string _currentString;

        public DbHelper(string connectionName)
        {
            _currentString = ConnectionString[connectionName];
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_currentString);
        }

        public DataTable FillTable(string queryString, params MySqlParameter[] param)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable;
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
                    Logger.Exception(ex);
                    return -1;
                }
            }
        }

        private static MySqlCommand GetParamCommand(string queryString, MySqlConnection connection,
            params MySqlParameter[] param)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = queryString;
            if (param != null && param.Length > 0)
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
