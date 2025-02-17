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
    internal class MovieEpisode: Table
    {
        [ColumnName("anime_id", 100, "anime", "title")]
        public string AnimeName  { get; set; }
        [ColumnName("name")]
        public string Name { get; set; }
        [ColumnName("duration_min")]
        public float DurationMin { get; set; }
        [ColumnName("is_movie")]
        public bool IsMovie {  get; set; }
        [ColumnName("episode_number")]
        public int EpisodeNumber { get; set; }

        public MovieEpisode() { }
        public MovieEpisode(int id, string name, float durationMin, bool isMovie, int episodeNumber)
        {
            Id = id;
            Name = name;
            DurationMin = durationMin;
            IsMovie = isMovie;
            EpisodeNumber = episodeNumber;
        }

        public override string GetName()
        {
            return "movie_episode";
        }
    }
}
