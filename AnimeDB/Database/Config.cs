using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnimeDB.Database
{
    using System.Configuration;

    /// <summary>
    /// A class that holds all relevant configuration, can be change on runtime and will be saved for the next run
    /// </summary>
    public class Config
    {
        private static Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public string DataSource
        {
            get => GetValue("DataSource");
            set => SetValue("DataSource", value);
        }

        public string Database
        {
            get => GetValue("Database");
            set => SetValue("Database", value);
        }

        public string Name
        {
            get => GetValue("Name");
            set => SetValue("Name", value);
        }

        public string Password
        {
            get => GetValue("Password");
            set => SetValue("Password", value);
        }

        public bool TrustCertificate
        {
            get => bool.TryParse(GetValue("TrustCertificate"), out bool result) && result;
            set => SetValue("TrustCertificate", value.ToString());
        }

        private static Config? instance = null;

        public static Config Get()
        {
            if(instance == null) instance = new Config();
            return instance;
        }

        private Config() { }

        private string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? "";
        }

        private void SetValue(string key, string value)
        {
            configFile.AppSettings.Settings[key].Value = value;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }

}
