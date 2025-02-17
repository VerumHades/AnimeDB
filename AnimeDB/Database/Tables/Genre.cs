using AnimeDB.UserInterface;
using AnimeDB.UserInterface.prompts;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.Database.Tables
{
    public class Genre: Table
    {
        [ColumnName("name")]
        public string Name { get; set; }

        public Genre() { }
        public Genre(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string GetName()
        {
            return "genre";
        }
    }
}
