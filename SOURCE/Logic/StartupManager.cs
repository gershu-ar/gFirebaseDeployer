using Microsoft.Win32;
using System.Windows.Forms;

namespace gFirebaseDeployer.Logic
{
    public static class StartupManager
    {
        private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "gFirebaseDeployer";

        public static void SetStartup(bool enable)
{
    using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
    if (enable)
    {
        key?.SetValue("gFirebaseDeployer", Application.ExecutablePath);
    }
    else
    {
        key?.SetValue("gFirebaseDeployer", "", RegistryValueKind.String); // disables without removing
    }
}


        public static bool IsStartupEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, false);
            if (key == null) return false;

            var value = key.GetValue(AppName);
            return value != null;
        }
    }
}
