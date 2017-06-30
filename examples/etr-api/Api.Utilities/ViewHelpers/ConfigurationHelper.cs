using System.Configuration;

namespace Api.Utilities.ViewHelpers
{
    public class ConfigurationHelper
    {

        public static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static bool SettingExists(string key)
        {
            return ConfigurationManager.AppSettings[key] != null;
        }

        public static bool IsSettingTrue(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            return setting == "true" || setting == "1" || setting == "yes";
        }

        public static string Environment => ConfigurationManager.AppSettings["EnvironmentName"];

        public static string AppName => typeof(ConfigurationHelper).Assembly.FullName;

        public static string ServerMode => ConfigurationManager.AppSettings["ServerMode"];

        public static string SiteName => ConfigurationManager.AppSettings["SiteName"];

        public static string SiteUrl => ConfigurationManager.AppSettings["SiteUrl"];

        public static string AssetUrl => ConfigurationManager.AppSettings["AssetUrl"] ?? "~";

        public static string LiveSiteUrl => "https://" + SiteUrl;

        public static string OperationsUserEmail => ConfigurationManager.AppSettings["OperationsUserEmail"];

    }
}
