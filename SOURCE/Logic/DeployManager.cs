using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using gFirebaseDeployer.Models;

namespace gFirebaseDeployer.Logic
{
    public static class DeployManager
    {
        public static string BuildCommand(DeployProfile profile)
		{
			if (profile.Targets == null || profile.Targets.Count == 0)
				throw new InvalidOperationException("No deploy targets selected.");

			var sb = new StringBuilder("firebase deploy");

			// Add --only targets
			var onlyClause = string.Join(",", profile.Targets);
			sb.Append($" --only {onlyClause}");

			// Add extra flags if present
			if (!string.IsNullOrWhiteSpace(profile.ExtraFlags))
				sb.Append($" {profile.ExtraFlags.Trim()}");

			return sb.ToString();
		}



        public static void ExecuteCommand(string command, string workingDirectory, Action<bool, string> onComplete, Form owner)
		{
			Task.Run(() =>
			{
				var psi = new ProcessStartInfo
				{
					FileName = "cmd.exe",
					Arguments = "/C " + command,
					WorkingDirectory = workingDirectory,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				var process = new Process { StartInfo = psi };
				var outputBuilder = new StringBuilder();
				var errorBuilder = new StringBuilder();

				process.OutputDataReceived += (s, e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						outputBuilder.AppendLine(e.Data);
						owner?.Invoke(() =>
						{
							var method = owner.GetType().GetMethod("AppendConsoleLine");
							method?.Invoke(owner, new object[] { e.Data });
						});
					}
				};

				process.ErrorDataReceived += (s, e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						errorBuilder.AppendLine(e.Data);
						owner?.Invoke(() =>
						{
							var method = owner.GetType().GetMethod("AppendConsoleLine");
							method?.Invoke(owner, new object[] { e.Data });
						});
					}
				};

				try
				{
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.WaitForExit();

					var output = outputBuilder.ToString() + errorBuilder.ToString();
					bool success = process.ExitCode == 0 && !output.ToLower().Contains("error");

					owner?.Invoke(() => onComplete?.Invoke(success, output));
				}
				catch (Exception ex)
				{
					owner?.Invoke(() => onComplete?.Invoke(false, "Exception: " + ex.Message));
				}
			});
		}

    }
}
