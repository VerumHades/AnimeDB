using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.Database.Tables
{
    public abstract class Table
    {
        [ColumnName("id")]
        public int Id { get; set; }

        abstract public string GetName();
    }
}
