using AnimeDB.UserInterface.MenuOptions;
using AnimeDB.UserInterface.prompts;
using AnimeDB.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using AnimeDB.Database.Tables;

namespace AnimeDB.Database
{   
    /// <summary>
    /// Derived from code of the MenuBuild this class operates on table templates to save them without the need to create save and load operation for each table class individialy, it works mainly around attributes
    /// </summary>
    /// <typeparam name="T">The table template to use from Tables</typeparam>
    public class TableBuilder<T> where T : class, new()
    {
        private readonly Dictionary<string, object> _modifiedValues = new();
        private Root? root;
        private string table_name = "";

        public TableBuilder(string table_name, Root? root = null)
        {
            this.root = root;
            this.table_name = table_name;
        }

        public static string? GetID(string table_name, string columm_name, string value,  Root? root)
        {
            SqlConnection? connection = DatabaseSingleton.GetInstance();
            if (connection == null) return null;
            
            try
            {
                using (SqlCommand command = new SqlCommand($"SELECT id FROM {table_name} WHERE {columm_name} = @value", connection))
                {
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@value";
                    param.Value = value;

                    command.Parameters.Add(param);
                    return command.ExecuteScalar()?.ToString();
                }
            }
            catch(Exception ex)
            {
                root?.OpenPrompt(new ErrorPrompt($"Failed to fetch '{columm_name}' from ${table_name}: " + ex.Message));
                return null;
            }
        }


        /// <summary>
        /// Saves a table value into a table based on the predefined attributes of said template
        /// </summary>
        /// <param name="table">The table value to save</param>
        /// <returns></returns>
        public bool Save(T table)
        {
            SqlConnection? connection = DatabaseSingleton.GetInstance();
            if (connection == null) return false;

            try
            {
                List<string> property_names = new List<string>();
                List<string> property_aliases = new List<string>();

                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance); // El chato helpo
                var options = new List<MenuOption>();

                for(int i = 0;i < properties.Length; i++)
                {
                    var property = properties[i];
                    if (property.Name.ToLower() == "id") continue;

                    var attribute = property.GetCustomAttribute<ColumnNameAttribute>(); // El chato helpo

                    string columnName = attribute?.Name ?? property.Name;
                    int max_length = attribute?.MaxLength ?? 100;
                    if (columnName.Length > max_length) columnName = columnName.Substring(0,max_length);

                    var table_name = attribute?.LinkTableName;
                    var column_name = attribute?.LinkColumnName;
                    if (table_name != null && column_name != null)
                    {
                        object value = property.GetValue(table) ?? "";
                        string valueString = value.ToString();

                        string id_value = GetID(table_name, column_name, valueString, root);

                        if (property != null && property.CanWrite)
                            property.SetValue(table, id_value);
                    }

                    property_names.Add(columnName);
                    property_aliases.Add($"@{columnName}");
                }

                using (var command = new SqlCommand($"INSERT INTO {table_name} ({string.Join(",",property_names)}) VALUES ({string.Join(",", property_aliases)})", connection))
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name.ToLower() == "id") continue;

                        string columnName = prop.GetCustomAttribute<ColumnNameAttribute>()?.Name ?? prop.Name; // El chato helpo
                        object value = prop.GetValue(table);

                        // 🔥 Ensure correct type conversion
                        if (value is string strValue)
                        {
                            if (prop.PropertyType == typeof(int)) value = int.TryParse(strValue, out var i) ? i : DBNull.Value;
                            else if (prop.PropertyType == typeof(float)) value = float.TryParse(strValue, out var f) ? f : DBNull.Value;
                            else if (prop.PropertyType == typeof(double)) value = double.TryParse(strValue, out var d) ? d : DBNull.Value;
                            else if (prop.PropertyType == typeof(DateTime)) value = DateTime.TryParse(strValue, out var dt) ? dt : DBNull.Value;
                        }

                        // If value is null, use DBNull.Value
                        value ??= DBNull.Value;

                        command.Parameters.Add(new SqlParameter($"@{columnName}", value));
                    }

                    command.ExecuteNonQuery();
                    root?.OpenPrompt(new InformationPrompt("Created successfully."));
                }
            }
            catch (SqlException ex)
            {
                if (root == null) return false;
                if (ex.Number == 2627)  // Unique constraint violation
                {
                    root.OpenPrompt(new ErrorPrompt($"Failed. Already exists."));
                }
                else if (ex.Number == 547)  // Foreign key constraint violation
                {
                    root.OpenPrompt(new ErrorPrompt("This record cannot be added because it violates a foreign key constraint."));
                }
                else if (ex.Number == 515)  // Non-nullable constraint violation
                {
                    root.OpenPrompt(new ErrorPrompt("This record cannot be added because a non-nullable field is missing a value. Or some field depending on another is invalid (Anime or Genre name might be invalid.)"));
                }
                else
                    root.OpenPrompt(new ErrorPrompt("Failed to save, Reason: " + ex.Message));
                return false;
            }
            catch (Exception ex)
            {
                root?.OpenPrompt(new ErrorPrompt("Failed to save, Reason: " + ex.Message));
                return false;
            }
            return true;
        }
    }
}
