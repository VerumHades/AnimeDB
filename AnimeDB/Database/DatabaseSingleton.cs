
using AnimeDB.Database;
using AnimeDB.UserInterface;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace AnimeDB.Database
{
    /// <summary>
    /// A class from the original DAO, just modified to return null on failed connection
    /// </summary>
    public class DatabaseSingleton
    {
        private static SqlConnection? conn = null;
        private static string last_failiure_reason = "";
        public static string LastConnectionFailiureReason { get { return last_failiure_reason; } }
        private DatabaseSingleton()
        {

        }

        /// <summary>
        /// Creates a database connection  with the configuration information if possible
        /// </summary>
        /// <returns>Connection or null is the connection failed to establish</returns>
        public static SqlConnection? GetInstance()
        {
            if (conn == null)
            {
                Config conf = Config.Get();

                SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
                consStringBuilder.UserID = conf.Name;
                consStringBuilder.Password = conf.Password;
                consStringBuilder.InitialCatalog = conf.Database;
                consStringBuilder.DataSource = conf.DataSource;
                consStringBuilder.ConnectTimeout = 5;
                consStringBuilder.TrustServerCertificate = conf.TrustCertificate;


                try
                {
                    conn = new SqlConnection(consStringBuilder.ConnectionString);
                    conn.Open();
                }
                catch(SqlException exception)
                {
                    last_failiure_reason = exception.Message;
                    conn = null;
                }

            }
            return conn;
        }

        public static void CloseConnection()
        {
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }
    }
}
