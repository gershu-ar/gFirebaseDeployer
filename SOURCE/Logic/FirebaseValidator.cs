using System.Diagnostics;

namespace gFirebaseDeployer.Logic
{
    public static class FirebaseValidator
    {
        
		public static string GetFirebaseVersion()
		{
			try
			{
				var process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = "cmd.exe",
						Arguments = "/C firebase --version",
						RedirectStandardOutput = true,
						UseShellExecute = false,
						CreateNoWindow = true
					}
				};
				process.Start();
				string output = process.StandardOutput.ReadToEnd().Trim();
				process.WaitForExit();

				return process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output)
					? output
					: "Unknown";
			}
			catch
			{
				return "Not Installed";
			}
		}

		
		public static bool IsFirebaseInstalled()
		{
			try
			{
				var process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = "cmd.exe",
						Arguments = "/C firebase --version",
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						UseShellExecute = false,
						CreateNoWindow = true
					}
				};
				process.Start();
				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				return process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output);
			}
			catch
			{
				return false;
			}
		}

    }
}
