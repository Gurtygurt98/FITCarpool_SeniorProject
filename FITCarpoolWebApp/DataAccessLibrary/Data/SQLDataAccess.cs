﻿/*
    Project Developed by : Austin Phillips
    Page was Implemented using Dapper ORM 
    Page Description: This page provides a set of functions to perform database operations, an interface is used for this in classes that specify 
    database operations in order to secure the connection string 

*/
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DataAccessLibrary.Datas
{
    public class SQLDataAccess
    {
        private readonly IConfiguration _config;

        public SQLDataAccess(IConfiguration config)
        {
            _config = config;
        }
        // Gets the connection string 
        public string ConnectionString => _config.GetConnectionString("Default");
        // Sends data to the database - add operations 
        public async Task<List<T>> LoadData<T>(string sql, object queryParameters = null)
        {
            using (IDbConnection connection = new SqliteConnection(ConnectionString))
            {
                var data = await connection.QueryAsync<T>(sql, queryParameters);
                return data.ToList();
            }
        } 
        // Updates data in the database Remove , Update , Delete operations 
        public async Task SaveData<T>(string sql, T parameters)
        {
            using (IDbConnection connection = new SqliteConnection(ConnectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}