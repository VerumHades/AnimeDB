using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.Database
{
    /// <summary>
    /// Attribute for handling table template classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnNameAttribute : Attribute
    {
        public string Name { get; }
        public int MaxLength { get; }

        public string? LinkTableName { get; }
        public string? LinkColumnName {  get; }

        public ColumnNameAttribute(string name, int max_length = 100, string? link_table_name = null, string? link_column_name = null)
        {
            Name = name;
            MaxLength = max_length;
            LinkTableName = link_table_name;
            LinkColumnName = link_column_name;
        }
    }
}
