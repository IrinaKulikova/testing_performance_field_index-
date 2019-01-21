using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace indexes
{
    /// <summary>
    /// class for testing indexes varchar feild in ms sql databases
    /// </summary>
    public class TesterDB
    {
        public bool HasDataBase { get; private set; }
        public bool HasTable { get; private set; }
        public string ConnectionString { get; private set; }
        public string SchemaName { get; private set; }
        public string SQLFilePath { get; private set; }

        public TesterDB(string schema, string table, string sql)
        {
            HasTable = false;
            HasDataBase = false;
            SchemaName = schema;
            SQLFilePath = sql;
            ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;";
            CreateDB();
            InitializationDB();
        }

        /// <summary>
        /// create data base if not exists
        /// </summary>
        private void CreateDB()
        {
            if (!HasDataBase)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string createDB = "IF NOT EXISTS ( SELECT * FROM master.dbo.sysdatabases WHERE name = '" + SchemaName + "') BEGIN CREATE DATABASE " + SchemaName + " END";
                    using (var createDBCommand = new SqlCommand(createDB, connection))
                    {
                        var exists = createDBCommand.ExecuteNonQuery();
                        HasDataBase = true;
                        ConnectionString += "Initial Catalog = " + SchemaName + ";";
                    }
                }
            }
        }

        /// <summary>
        /// method form inizatilation table Users by data from sql file
        /// </summary>
        private void InitializationDB()
        {
            CheckTableExists();
            if (!HasTable)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    if (!HasTable)
                    {
                        var script = File.ReadAllText(SQLFilePath);
                        var commandStrings = Regex.Split(script, @";", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                        connection.Open();

                        foreach (var commandString in commandStrings)
                        {
                            if (commandString.Trim() != "")
                            {
                                using (var command = new SqlCommand(commandString, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// check exists database
        /// </summary>
        private void CheckTableExists()
        {
            if (!HasTable)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string DBandTableExists = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '" + SchemaName + "'";
                    using (var existsCommand = new SqlCommand(DBandTableExists, connection))
                    {
                        var exists = existsCommand.ExecuteReader();
                        HasTable = exists.HasRows;
                    }
                }
            }
        }


        /// <summary>
        /// test performance, method selects from Users by name
        /// </summary>
        /// <param name="name"></param>
        public void TestMethod(string name)
        {
            Stopwatch watcher = new Stopwatch();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                var query = "SELECT * FROM Users WHERE first_name = '" + name + "'";
                connection.Open();
                watcher.Start();

                using (var existsCommand = new SqlCommand(query, connection))
                {
                    var exists = existsCommand.ExecuteReader();
                }
                watcher.Stop();
            }

            Console.WriteLine("Time elapsed: {0}", watcher.Elapsed);
            Console.WriteLine(new string('-', 50));

            Drop();
        }

        /// <summary>
        /// droping database after testing
        /// </summary>
        private void Drop()
        {
            if (HasDataBase)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string dropDB = "USE master IF EXISTS ( SELECT * FROM master.dbo.sysdatabases WHERE name = '" + SchemaName + "') BEGIN DROP DATABASE " + SchemaName + " END";
                    using (var dropDBCommand = new SqlCommand(dropDB, connection))
                    {
                        var exists = dropDBCommand.ExecuteNonQuery();
                        HasDataBase = false;
                    }
                }
            }
        }
    }
}
