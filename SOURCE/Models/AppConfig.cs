using System.Collections.Generic;

namespace gFirebaseDeployer.Models
{
    public class AppConfig
    {
        public string Language { get; set; } = "en";
        public bool StartWithWindows { get; set; } = false;
        public string LastUsedProfile { get; set; } = "";
        public List<DeployProfile> Profiles { get; set; } = new();
		public bool AlwaysOnTop { get; set; } = false;

    }
}
