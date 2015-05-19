// Written by: Roman D. Clarkson
// http://www.romanclarkson.com  mailto:inspirit@romanclarkson.com

using System;
using System.Web.Configuration;

namespace BlogEngine.Core.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class InstallUtil
    {
        private const string beversion = "BEVersion";
        private const string installdate = "InstallDate";
        private const string lastupdated = "LastUpdated";

        /// <summary>
        /// 
        /// </summary>
        public static void CheckInstallation()
        {
            CheckIfInstalling();
            CheckIfUpgrading();
        }

        private static void UpdateTheLastUpdatedAppKey()
        {
            string currentDateTime = DateTime.Now.ToString();

            if (WebConfigurationManager.AppSettings[lastupdated] == null)
            {
                AppConfig.Instance().SetValue(lastupdated, currentDateTime);
            }
            else
            {
                AppConfig.Instance().SetValue(lastupdated, currentDateTime);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// Todo:  Need to expand this to check the web.config assembly version and compare it with the current assembly in the bin.
        private static void CheckIfUpgrading()
        {
            string currentAssemblyVersion = WebConfigurationManager.AppSettings[beversion];
            string assemblyVersion = BlogSettings.Instance.Version();

            if (WebConfigurationManager.AppSettings[beversion] == null)
            {
                AppConfig.Instance().SetValue(beversion, BlogSettings.Instance.Version());
                UpdateTheLastUpdatedAppKey();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// Todo:  This can be expanded using install scripts.
        private static void CheckIfInstalling()
        {
            string currentDateTime = DateTime.Now.ToString();

            if (WebConfigurationManager.AppSettings[installdate] == string.Empty ||
                WebConfigurationManager.AppSettings[installdate] == null)
            {
                AppConfig.Instance().SetValue(installdate, currentDateTime);
                UpdateTheLastUpdatedAppKey();
            }
        }
    }
}