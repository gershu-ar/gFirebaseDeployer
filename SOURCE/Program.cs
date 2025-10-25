using System;
using System.IO;
using System.Windows.Forms;

namespace gFirebaseDeployer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
			{
				LogException(args.ExceptionObject as Exception);
			};

			Application.ThreadException += (sender, args) =>
			{
				LogException(args.Exception);
			};

			bool createdNew;
			using (Mutex mutex = new Mutex(true, "gFirebaseDeployer_SingleInstance", out createdNew))
			{
				if (!createdNew)
				{
					MessageBox.Show("Application is already running.", "Info",
						MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				Application.SetHighDpiMode(HighDpiMode.SystemAware);
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Forms.MainForm());
			}
		}

        private static void LogException(Exception? ex)
        {
            if (ex == null) return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            string filename = $"error_{timestamp}.log";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            string content = $"[{DateTime.Now}]\n{ex.GetType()}\n{ex.Message}\n{ex.StackTrace}";

            try
            {
                File.WriteAllText(path, content);
                MessageBox.Show("An unexpected error occurred. See log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                // If logging fails, silently ignore
            }
        }
    }
}
