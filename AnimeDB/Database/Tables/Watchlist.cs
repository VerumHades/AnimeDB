using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.Database.Tables
{
    public class Watchlist : Table
    {
        [ColumnName("anime_id", 100, "anime", "title")]
        public string AnimeName { get; set; }
        [ColumnName("user_id", 100, "app_user", "username")]
        public string UserName {  get; set; }
        public override string GetName()
        {
            return "watchlist";
        }

        public Watchlist()
        {
        }

        public Watchlist(int id, string animeName, string userName)
        {
            Id = id;
            AnimeName = animeName;
            UserName = userName;
        }
    }
}
