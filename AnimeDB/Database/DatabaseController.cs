using AnimeDB.Database.Tables;
using AnimeDB.UserInterface;
using AnimeDB.UserInterface.prompts;
using Azure;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.Database
{
    public class DatabaseController
    {
        private DatabaseController() { }

        public enum TableStatus
        {
            NOT_EXISTS,
            MISSING_COLUMNS,
            VALID,
            NO_CONNECTION
        }

        static private List<(string, List<string>, List<string>)> table_templates =
            new() {
                ("genre", new () {"name"}, new() {"name varchar(100) not null unique"}),
                ("app_user", new () {"username", "userpassword"}, new() {
                    "username varchar(100) not null unique",
                    "userpassword varchar(100) not null"
                }),
                ("anime", new () {"genre_id", "title", "description", "date_aired", "date_ended"}, new() {
                    "genre_id int foreign key references genre(id) not null",
                    "title varchar(100) not null unique",
                    "description varchar(256) not null",
                    "date_aired datetime not null",
                    "date_ended datetime not null",
                    "check(date_aired < date_ended)"
                }),
                ("watchlist", new () {"anime_id", "user_id"}, new() {
                    "anime_id int foreign key references anime(id) not null",
                    "user_id int foreign key references app_user(id) not null",
                    "CONSTRAINT unique_watch UNIQUE (anime_id, user_id)"
                }),
                ("movie_episode", new () {"anime_id", "name", "duration_min", "is_movie", "episode_number"}, new() {
                    "anime_id int foreign key references anime(id) not null",
                    "name varchar(100) not null",
                    "duration_min float not null",
                    "is_movie char(1) not null",
                    "episode_number int not null",
                    "CONSTRAINT unique_episode UNIQUE (anime_id, episode_number)" // El Chato
                }),
            };

        /// <summary>
        /// Checks if the database is correctly setup and tries to fix it if its is not
        /// </summary>
        /// <param name="root">Where to display error prompts, will probably not give as many fixing options without it</param>
        static public void CheckDatabaseIntegrity(Root? root = null)
        {
            bool tables_created = false;
            List<string> missing_tables = new List<string>(){ };
            foreach(var c in table_templates)
            {
                TableStatus status = CheckTableIntegrity(c.Item1, c.Item2);

                if (status == TableStatus.VALID) continue;
                if (status == TableStatus.NO_CONNECTION) continue;

                if (status == TableStatus.NOT_EXISTS)
                {
                    if(!CreateTable(c.Item1, c.Item3))
                    {
                        root.OpenPrompt(new ErrorPrompt("Failed to create table: " + c.Item1 + " Reason: " + create_last_problem));
                    }
                    tables_created = true;
                    missing_tables.Add(c.Item1);
                }
                else if(status == TableStatus.MISSING_COLUMNS)
                {
                    root?.OpenPrompt(new ConfirmationPrompt(
                    "The table '" + c.Item1 + "' exists but has invalid format, do you wish to delete it and create a valid one? (Not doing so will impare the proper function of this application)",
                        () => {
                            if (!DropTable(c.Item1))
                            {
                                root.OpenPrompt(new ConfirmationPrompt($"Dropping single table {c.Item1} failed. Drop and reset entire database?", () =>
                                {
                                    if (!DropDatabase()) root.OpenPrompt(new ErrorPrompt("Failed to drop database, You are on your own to fix this mess."));
                                    else CheckDatabaseIntegrity(root);
                                }));
                            }
                            else CreateTable(c.Item1, c.Item3);
                        }
                    ));
                }
            }

            if(tables_created)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Created missing tables: ");
                foreach (var c in missing_tables)
                {
                    sb.AppendLine(c + " ");
                }
                root?.OpenPrompt(new InformationPrompt(sb.ToString()));
            }
        }
        static private string create_last_problem = "";

        /// <summary>
        /// Creates a table with of name with column  attributes
        /// </summary>
        /// <param name="name">The name of the table</param>
        /// <param name="columns">A list of SQL valid string column definitions</param>
        /// <returns>Whether the operation was successful</returns>
        static public bool CreateTable(string name, List<string> columns)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"create table {name}(");
            sb.AppendLine($"id int primary key identity(1,1),");
            foreach (var c in columns)
            {
                sb.AppendLine(c + ",");
            }
            sb.AppendLine(");");
            
            var connection = DatabaseSingleton.GetInstance();
            if (connection == null)
            {
                create_last_problem = "No database connected.";
                return false;
            }

            try
            {
                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    int result = command.ExecuteNonQuery();

                    if (result == -1) return true;
                    else
                    {
                        create_last_problem = "Command execution failed.";
                        return false;
                    }
                }
            } catch (SqlException ex) {
                create_last_problem = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Drops a table
        /// </summary>
        /// <param name="name">Name of the table</param>
        /// <returns>Whether the operation was successful</returns>
        static public bool DropTable(string name)
        {
            var connection = DatabaseSingleton.GetInstance();
            if (connection == null) return false;

            try
            {
                using (SqlCommand command = new SqlCommand($"drop table {name}", connection))
                {
                    int result = command.ExecuteNonQuery();

                    if (result == -1) return true;
                    else return false;
                }
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Drops all tables in the database and creates a new one according to the setup format
        /// </summary>
        /// <param name="root">Where to display error prompts</param>
        /// <returns>Whether the operation was successful</returns>
        static public bool ResetDatabase(Root? root = null)
        {
            if (!DropDatabase())
            {
                root?.OpenPrompt(new ErrorPrompt("Failed to drop database, You are on your own to fix this mess."));
                return false;
            }
            else
            {
                CheckDatabaseIntegrity(root);
                return true;
            }
        }

        /// <summary>
        /// Drops all tables in a database
        /// </summary>
        /// <returns>Whether the operation was successful</returns>
        static public bool DropDatabase()
        {
            foreach (var template in table_templates.AsEnumerable().Reverse())
            {
                var integrity = CheckTableIntegrity(template.Item1, template.Item2);
                if (!DropTable(template.Item1) && integrity == TableStatus.MISSING_COLUMNS) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks whether a table exists and has all the columns under set names, doesnt check column types
        /// </summary>
        /// <param name="name">Name of the table</param>
        /// <param name="columns">Names of columns</param>
        /// <returns></returns>
        static public TableStatus CheckTableIntegrity(string name, List<string> columns)
        {
            var connection = DatabaseSingleton.GetInstance();
            if (connection == null) return TableStatus.NO_CONNECTION;

            string tableQuery = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = @tableName"; // El chato
            using (SqlCommand command = new SqlCommand(tableQuery, connection))
            {
                command.Parameters.AddWithValue("@tableName", name);
                int tableExists = (int)command.ExecuteScalar();

                if (tableExists == 0)
                {
                    return TableStatus.NOT_EXISTS;
                }
            }

            foreach (var column in columns)
            {
                string columnQuery = "SELECT COUNT(*) FROM information_schema.columns WHERE table_name = @tableName AND column_name = @columnName"; // El chato
                using (SqlCommand command = new SqlCommand(columnQuery, connection))
                {
                    command.Parameters.AddWithValue("@tableName", name);
                    command.Parameters.AddWithValue("@columnName", column);
                    int columnExists = (int)command.ExecuteScalar();

                    if (columnExists == 0)
                    {
                        return TableStatus.MISSING_COLUMNS;
                    }
                }
            }

            return TableStatus.VALID;
        }
    }
}
