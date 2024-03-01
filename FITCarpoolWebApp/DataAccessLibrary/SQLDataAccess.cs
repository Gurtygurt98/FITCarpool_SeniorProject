using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public class SQLDataAccess : ISQLDataAccess
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SQLDataAccess> _logger;

        public string ConnectionStringName { get; set; } = "Default";

        public SQLDataAccess(IConfiguration config, ILogger<SQLDataAccess> logger)
        {
            _config = config;
            _logger = logger;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString(ConnectionStringName);
        }

        private string GetDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog; // This gets the database name from the connection string
        }

        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            string connectionString = GetConnectionString();
            string databaseName = GetDatabaseName(connectionString);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                _logger.LogTrace("Loading data using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                try
                {
                    var data = await connection.QueryAsync<T>(sql, parameters);
                    return data.ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading data using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                    throw;
                }
            }
        }
        public async Task SaveData<T>(string sql, T parameters)
        {
            string connectionString = GetConnectionString();
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                _logger.LogTrace("Loading data using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                try
                {
                    await connection.ExecuteAsync(sql, parameters);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving data using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                    throw;
                }
            }
        }
    }
}
