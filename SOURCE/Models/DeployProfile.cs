namespace gFirebaseDeployer.Models
{
    public class DeployProfile
    {
        public string Name { get; set; } = "";
        public string FolderPath { get; set; } = "";
        public string ExtraFlags { get; set; } = "";
        public List<string> Targets { get; set; } = new();
    }
}
