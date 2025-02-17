using AnimeDB.UserInterface;
using AnimeDB.UserInterface.prompts;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AnimeDB.Database.Tables
{
    public class User: Table
    {
        [ColumnName("username")]
        public string Name { get; set; }
        [ColumnName("userpassword")]
        public string Password { get; set; }

        public User() { }
        public User(int id, string name, string password)
        {
            Id = id;
            Name = name;
            Password = password;
        }

        public override string GetName()
        {
            return "app_user";
        }

        /// <summary>
        /// Find a user by name and renames them to a new name
        /// </summary>
        /// <param name="old_name">Old name</param>
        /// <param name="new_name">New name</param>
        /// <param name="root">Optional name where prompts will be displayed</param>
        /// <returns></returns>
        public static bool Rename(string old_name, string new_name, Root? root = null)
        {
            string? id = TableBuilder<User>.GetID("app_user", "username", old_name, root);
            if (id == null)
            {
                root?.OpenPrompt(new ErrorPrompt($"Failed to rename because user under name '{old_name}' doesnt exist."));
                return false;
            }

            SqlConnection connection = DatabaseSingleton.GetInstance();

            try
            {
                using (SqlCommand command = new SqlCommand("UPDATE app_user SET username = @newUserName WHERE id = @id", connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", id));
                    command.Parameters.Add(new SqlParameter("@newUserName", new_name));
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                root?.OpenPrompt(new ErrorPrompt($"Failed to rename user, Reason:" + ex.Message));
                return false;
            }

            return true;
        }
    }
}
