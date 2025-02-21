﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using IO = System.IO;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Data.Sqlite;
using Snowflake.Filesystem;

namespace Snowflake.Persistence
{
    public class SqliteDatabase : ISqlDatabase
    {
        /// <inheritdoc/>
        public string DatabaseName { get; }

        private readonly string dbConnectionString;

        public SqliteDatabase(string fileName)
        {
            this.DatabaseName = fileName;
            if (!IO.File.Exists(this.DatabaseName))
            {
                IO.File.Create(this.DatabaseName).Dispose();
            }

            this.dbConnectionString = $"Data Source={this.DatabaseName};";
        }

        /// <inheritdoc/>
        public IDbConnection GetConnection()
        {
            var connection = new SqliteConnection(this.dbConnectionString);
            connection.Open();
            using (var walCommand = connection.CreateCommand())
            {
                walCommand.CommandText = "PRAGMA journal_mode = WAL;" +
                                         "PRAGMA foreign_keys = ON;" +
                                         "PRAGMA recursive_triggers = ON;" +
                                         "PRAGMA synchronous = NORMAL;";
                walCommand.ExecuteNonQuery();
            }

            return connection;
        }

        /// <summary>
        /// Queries on a new connection to the database.
        /// The connection will be safely closed after the operation.
        /// </summary>
        /// <typeparam name="T">The type the query will return</typeparam>
        /// <param name="queryFunction">A function to query the database using the opened connection</param>
        /// <returns>The requested data.</returns>
        public T Query<T>(Func<IDbConnection, T> queryFunction)
        {
            T record;
            using (var dbTransaction = this.GetConnection().BeginTransaction())
            using (var dbConnection = dbTransaction.Connection)
            {
                dbConnection.Open();
                record = queryFunction(dbConnection);
                dbTransaction.Commit();
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }

            return record;
        }

        /// <summary>
        /// Executes on a new connection to the database.
        /// The connection will be safely closed after the operation.
        /// </summary>
        /// <param name="queryFunction">A function to query the database using the opened connection</param>
        public void Execute(Action<IDbConnection> queryFunction)
        {
            using (var dbTransaction = this.GetConnection().BeginTransaction())
            using (var dbConnection = dbTransaction.Connection)
            {
                dbConnection.Open();
                queryFunction(dbConnection);
                dbTransaction.Commit();
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }
        }

        /// <summary>
        /// Queries on a new connection to the database.
        /// The connection will be safely closed after the operation.
        /// </summary>
        /// <typeparam name="T">The type the query will return</typeparam>
        /// <param name="query">The SQL query to execute</param>
        /// <param name="param">The query parameters</param>
        /// <returns>The requested data.</returns>
        public IEnumerable<T> Query<T>(string query, object? param = null)
        {
            return this.Query(dbConnection => dbConnection.Query<T>(query, param));
        }

        /// <summary>
        /// Executes on a new connection to the database.
        /// The connection will be safely closed after the operation.
        /// </summary>
        /// <param name="query">The SQL query to execute</param>
        /// <param name="param">The query parameters</param>
        public void Execute(string query, object? param = null)
        {
            this.Execute(dbConnection => dbConnection.Execute(query, param));
        }

        /// <summary>
        /// Executes a single row query
        /// The connection will be safely closed after the operation.
        /// </summary>
        /// <typeparam name="T">The type the query will return</typeparam>
        /// <param name="query">The SQL query to execute</param>
        /// <param name="param">The query parameters</param>
        /// <returns>The requested data, or null if not present.</returns>
        public T QueryFirstOrDefault<T>(string query, object? param = null)
        {
            return this.Query(dbConnection => dbConnection.QueryFirstOrDefault<T>(query, param));
        }

        /// <summary>
        /// Creates a table in the database
        /// </summary>
        /// <param name="tableName">The name of the table to create</param>
        /// <param name="columns">The names of the columns to create</param>
        public void CreateTable(string tableName, params string[] columns)
        {
            this.Execute($@"CREATE TABLE IF NOT EXISTS {tableName}({string.Join(",", columns)})");
        }
    }
}
