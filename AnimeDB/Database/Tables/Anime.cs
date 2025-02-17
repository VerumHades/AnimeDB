using AnimeDB.UserInterface;
using AnimeDB.UserInterface.prompts;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.Database.Tables
{
    public class Anime: Table
    {
        [ColumnName("genre_id", 100, "genre", "name")]
        public string GenreName { get; set; }
        [ColumnName("title")]
        public string Title { get; set; }
        [ColumnName("description",256)]
        public string Description { get; set; }
        [ColumnName("date_aired")]
        public DateTime DateAired {  get; set; }
        [ColumnName("date_ended")]
        public DateTime DateEnded {  get; set; }
        public Anime() { }

        public Anime(string genreName, string title, string description, DateTime dateAired, DateTime dateEnded)
        {
            GenreName = genreName;
            Title = title;
            Description = description;
            DateAired = dateAired;
            DateEnded = dateEnded;
        }

        public override string GetName()
        {
            return "anime";
        }

        /// <summary>
        /// Deletes an anime all of its episodes and all watchlist entries
        /// </summary>
        /// <param name="name">Name of the anime to delete</param>
        /// <param name="root">The UI root, can function without it</param>
        /// <returns></returns>
        public static bool Delete(string name, Root? root = null)
        {
            string? id = TableBuilder<Anime>.GetID("anime","title",name,root);
            if (id == null)
            {
                root?.OpenPrompt(new ErrorPrompt($"Failed to delete because anime under name '{name}' doesnt exist."));
                return false;
            }
            SqlConnection connection = DatabaseSingleton.GetInstance();

            try
            {
                using (SqlCommand command = new SqlCommand("delete from watchlist WHERE anime_id = @id", connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", id));
                    command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand("delete from movie_episode WHERE anime_id = @id", connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", id));
                    command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand("delete from anime WHERE id = @id", connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", id));
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                root?.OpenPrompt(new ErrorPrompt($"Failed to delete anime, Reason:" + ex.Message));
                return false;
            }

            return true;
        }
    }
}
