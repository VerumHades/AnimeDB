using AnimeDB.Database.Tables;
using AnimeDB.UserInterface;
using AnimeDB.UserInterface.prompts;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnimeDB.Database
{
    public class Exports
    {
        private class ExportStructure
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public List<string> AnimeNames { get; set; }

        }

        /// <summary>
        /// Returns a user by name
        /// </summary>
        /// <param name="name">The user name</param>
        /// <returns>The user if found otherwise null</returns>
        public static User? GetByName(string name)
        {
            User? osoba = null;
            SqlConnection connection = DatabaseSingleton.GetInstance();
            // 1. declare command object with parameter
            using (SqlCommand command = new SqlCommand("SELECT * FROM app_user WHERE username = @name", connection))
            {
                // 2. define parameters used in command 
                SqlParameter param = new SqlParameter();
                param.ParameterName = "@name";
                param.Value = name;

                // 3. add new parameter to command object
                command.Parameters.Add(param);
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        osoba = new User(
                            Convert.ToInt32(reader[0].ToString()),
                            reader[1].ToString(),
                            reader[2].ToString()
                            );
                    }
                }
            }
            return osoba;

        }
        /// <summary>
        /// Imports a user and his watchlist from an export file, creates the user if he doesnt exist
        /// </summary>
        /// <param name="filepath">Path to export file</param>
        /// <param name="root">Optional display for error prompts</param>
        /// <returns>Wheter the operation was successful</returns>
        public static bool Import(string filepath, Root? root = null) //  El Chato
        {
            string json;
            try
            {
                json = File.ReadAllText(filepath);
            }
            catch (IOException ex)
            {
                root?.OpenPrompt(new ErrorPrompt("An error occurred while reading the file: " + ex.Message));
                return false;
            }
            catch (Exception ex)
            {
                root?.OpenPrompt(new ErrorPrompt("An unexpected error occurred: " + ex.Message));
                return false;
            }

            ExportStructure importStructure;
            try
            {
                importStructure = JsonSerializer.Deserialize<ExportStructure>(json);
                if (importStructure == null)
                {
                    root?.OpenPrompt(new ErrorPrompt("Failed to deserialize JSON data."));
                    return false;
                }
            }
            catch (JsonException ex)
            {
                root?.OpenPrompt(new ErrorPrompt("Failed to parse JSON: " + ex.Message));
                return false;
            }

            User? user = GetByName(importStructure.Username);
            if (user == null || user.Password != importStructure.Password)
            {
                if(!new TableBuilder<User>("app_user").Save(new User(0, importStructure.Username, importStructure.Password)))
                {
                    root?.OpenPrompt(new ErrorPrompt("Failed to create user."));
                    return false;
                }
                user = GetByName(importStructure.Username);
            }

            SqlConnection connection = DatabaseSingleton.GetInstance();
            foreach (var animeTitle in importStructure.AnimeNames)
            {
                string? animeId = TableBuilder<Anime>.GetID("anime", "title", animeTitle, root);
                if (animeId != null)
                {
                    using (SqlCommand command = new SqlCommand("INSERT INTO watchlist (user_id, anime_id) VALUES (@userId, @animeId)", connection))
                    {
                        command.Parameters.AddWithValue("@userId", user.Id);
                        command.Parameters.AddWithValue("@animeId", animeId);

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            root?.OpenPrompt(new ErrorPrompt("An error occurred while adding anime to watchlist: " + ex.Message));
                            return false;
                        }
                    }
                }
            }

            root?.OpenPrompt(new InformationPrompt("Watchlist imported successfully!"));
            return true;
        }

        /// <summary>
        /// Exports a user and his watchlist
        /// </summary>
        /// <param name="username">Name of the  user</param>
        /// <param name="filepath">Path to export</param>
        /// <param name="root">Display for error prompts</param>
        /// <returns>Whether the export was successful</returns>
        public static bool ExportUserData(string username, string filepath, Root? root = null)
        {
            User? user = GetByName(username);
            if (user == null) return false;

            List<string> watchlist = new();
            SqlConnection connection = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT anime.title FROM watchlist inner join anime on watchlist.anime_id = anime.id WHERE user_id = @id", connection))
            {
                SqlParameter param = new SqlParameter();
                param.ParameterName = "@id";
                param.Value = user.Id;

                command.Parameters.Add(param);
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        watchlist.Add(reader[0].ToString());
                    }
                }
            }

            ExportStructure exportStructure = new ExportStructure();
            exportStructure.AnimeNames = watchlist;
            exportStructure.Username = username;
            exportStructure.Password = user.Password;

            string json = JsonSerializer.Serialize(exportStructure, new JsonSerializerOptions { WriteIndented = true });
            try
            {
                File.WriteAllText(filepath, json);
                root?.OpenPrompt(new InformationPrompt("Exported successfully: " + filepath));
            }
            catch (IOException ex)
            {
                root?.OpenPrompt(new ErrorPrompt("An error occurred while writing the file: " + ex.Message));
            }
            catch (Exception ex)
            {
                root?.OpenPrompt(new ErrorPrompt("An unexpected error occurred: " + ex.Message));
            }

            return true;
        }
    }
}
