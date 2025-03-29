﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;

namespace Hospital.DatabaseServices
{
    public class LoggerDatabaseService
    {
        private readonly Config _config;

        public LoggerDatabaseService()
        {
            _config = Config.GetInstance();
        }

        public async Task<List<LogEntryModel>> GetLogsFromDB()
        {
            const string queryGetLogs = "SELECT * FROM Logs ORDER BY Timestamp DESC;";
            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);
                await connection.OpenAsync().ConfigureAwait(false);

                SqlCommand selectCommand = new SqlCommand(queryGetLogs, connection);

                SqlDataReader reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);

                List<LogEntryModel> logs = new List<LogEntryModel>();

                while(await reader.ReadAsync().ConfigureAwait(false))
                {
                    int logId = reader.GetInt32(0);
                    int userId = reader.GetInt32(1);
                    ActionType action = (ActionType)Enum.Parse(typeof(ActionType), reader.GetString(2));
                    DateTime timestamp = reader.GetDateTime(3);
                    logs.Add(new LogEntryModel(logId, userId, action, timestamp));
                }
                return logs;

            }
            catch (SqlException e) 
            {
                Console.WriteLine($"SQL Exception: {e.Message}");
                return new List<LogEntryModel>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"General Exception: {e.Message}");
                return new List<LogEntryModel>();
            }
        }
    }
}
  