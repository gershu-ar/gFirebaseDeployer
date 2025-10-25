using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using gFirebaseDeployer.Logic;
using gFirebaseDeployer.Models;
using System.Text.RegularExpressions;


namespace gFirebaseDeployer.Forms
{
    public partial class MainForm : Form
    {
        private ToolTip tips = null!;
        private NotifyIcon trayIcon = null!;
        private bool isDeploying = false;
        private bool isInitializing = true;

        private System.Windows.Forms.Timer deployAnimTimer = new System.Windows.Forms.Timer();
        private int deployAnimIndex = 0;
        private readonly string[] deployFrames =
        {
            "DEPLOYING ⬆️....",
            "DEPLOYING .x...",
            "DEPLOYING ..⬆..",
            "DEPLOYING .️...x"
        };

        public MainForm()
        {
			//COMIENZO CONSTRUCTOR
			
			
            InitializeComponent();

            ConfigManager.Load();
			this.onTopCheckBox.Checked = ConfigManager.Config.AlwaysOnTop;
			this.TopMost = ConfigManager.Config.AlwaysOnTop;
            LanguageManager.Load(ConfigManager.Config.Language);
            ApplyLanguage();
            this.languageDropdown.SelectedItem = ConfigManager.Config.Language == "es" ? "Español" : "English";
            ApplyTooltips();
            SetupTrayIcon();
            ValidateFirebase();
			string version = FirebaseValidator.GetFirebaseVersion();
			this.firebaseStatusLabel.Text = $"{LanguageManager.Get("RunningFirebase")}: {version}";
            PopulateProfileSelector();

            this.startupCheckBox.Checked = ConfigManager.Config.StartWithWindows;

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.FormClosing += MainForm_FormClosing;
            this.Load += MainForm_Load;

            this.deployAnimTimer.Interval = 150;
            this.deployAnimTimer.Tick += (s, e) =>
            {
                deployAnimIndex = (deployAnimIndex + 1) % deployFrames.Length;
                this.deployButton.Text = deployFrames[deployAnimIndex];
            };
        
		//FIN CONSTRUCTOR
		isInitializing = false;

		
		}

		private void MainForm_Load(object? sender, EventArgs e)
		{
			string[] args = Environment.GetCommandLineArgs();
			bool launchedByStartup = args.Contains("--startup");

			if (launchedByStartup && ConfigManager.Config.StartWithWindows)
			{
				this.WindowState = FormWindowState.Minimized;
				this.Hide();
			}
			else
			{
				this.WindowState = FormWindowState.Normal;
				this.Show();
			}

			// Inicializar consola
			this.consoleBox.BackColor = Color.LightGray;
			this.consoleBox.ForeColor = Color.Black;
			this.consoleBox.Clear();
			this.consoleBox.AppendText("System ready" + Environment.NewLine);

			// Auto-fit language dropdown width to text
			int maxWidth = 0;
			using (Graphics g = this.CreateGraphics())
			{
				foreach (var item in this.languageDropdown.Items)
				{
					int w = (int)g.MeasureString(item?.ToString() ?? "", this.languageDropdown.Font).Width;
					if (w > maxWidth) maxWidth = w;
				}
			}
			this.languageDropdown.Width = Math.Min(maxWidth + 24, this.optionsGroup.Width - 20);
		}


        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                AttemptExit();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (isDeploying)
            {
                e.Cancel = true;
                AttemptExit();
            }
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void AttemptExit()
        {
            if (isDeploying)
            {
                var confirm = MessageBox.Show("Exit gFirebaseDeployer?\nAny running deploy will be interrupted.",
                    "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    trayIcon?.Dispose();
                    isDeploying = false;
                    Application.Exit();
                }
            }
            else
            {
                trayIcon?.Dispose();
                Application.Exit();
            }
        }
		
		private void CopyConsoleButton_Click(object? sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.consoleBox.Text))
				Clipboard.SetText(this.consoleBox.Text);
		}

		private void CleanConsoleButton_Click(object? sender, EventArgs e)
		{
			this.consoleBox.Clear();
		}

		
		private void DeleteProfile(string? name = null)
		{
			name ??= this.profileSelector.Text.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				MessageBox.Show("No profile selected to delete.");
				return;
			}

			var profile = ConfigManager.Config.Profiles.FirstOrDefault(p => p.Name == name);
			if (profile == null)
			{
				MessageBox.Show($"Profile '{name}' not found.");
				return;
			}

			var confirm = MessageBox.Show(
				$"Delete profile '{name}'?",
				"Confirm Delete",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);

			if (confirm != DialogResult.Yes)
				return;

			ConfigManager.Config.Profiles.Remove(profile);

			// If the deleted profile was the last used, clear it
			if (ConfigManager.Config.LastUsedProfile == name)
				ConfigManager.Config.LastUsedProfile = "";

			ConfigManager.Save();

			// Refresh selector without restoring last used
			PopulateProfileSelector(null, restoreLastUsed: false);
			this.profileSelector.Text = ""; // clear dropdown
			this.folderPathBox.Text = "";
			
			this.extraFlagsBox.Text = "";
			for (int i = 0; i < this.targetsList.Items.Count; i++)
				this.targetsList.SetItemChecked(i, false);
		}


		private void NewProfile()
		{
			this.profileSelector.Text = "";
			this.folderPathBox.Text = "";
			
			this.extraFlagsBox.Text = "";

			for (int i = 0; i < this.targetsList.Items.Count; i++)
				this.targetsList.SetItemChecked(i, false);

			// No tocar Config todavía, es solo limpiar la UI
		}


        private void ValidateFirebase()
        {
            if (!FirebaseValidator.IsFirebaseInstalled())
            {
                MessageBox.Show("Firebase CLI not found. Please install it and ensure it's in your PATH.");
            }
        }

        private void PopulateProfileSelector(string? selectProfile = null, bool restoreLastUsed = true)
		{
			this.profileSelector.Items.Clear();
			foreach (var p in ConfigManager.Config.Profiles)
				this.profileSelector.Items.Add(p.Name);

			// Explicit selection (e.g. after save)
			if (!string.IsNullOrWhiteSpace(selectProfile) &&
				this.profileSelector.Items.Contains(selectProfile))
			{
				this.profileSelector.SelectedItem = selectProfile;
				var prof = ConfigManager.Config.Profiles
					.FirstOrDefault(x => x.Name.Trim() == selectProfile.Trim());
				if (prof != null) LoadProfile(prof);
			}
			// Restore last used profile (startup case)
			else if (restoreLastUsed)
			{
				var last = ConfigManager.Config.LastUsedProfile;
				if (!string.IsNullOrWhiteSpace(last))
				{
					var match = this.profileSelector.Items.Cast<string?>()
						.FirstOrDefault(i => i?.Trim() == last.Trim());
					if (match != null)
					{
						this.profileSelector.SelectedItem = match;
						var prof = ConfigManager.Config.Profiles
							.FirstOrDefault(x => x.Name.Trim() == match.Trim());
						if (prof != null) LoadProfile(prof);
					}
				}
			}
		}


        private void LoadProfile(DeployProfile profile)
        {
            this.folderPathBox.Text = profile.FolderPath;

            this.extraFlagsBox.Text = profile.ExtraFlags;
            this.profileSelector.Text = profile.Name;

            for (int i = 0; i < this.targetsList.Items.Count; i++)
            {
                var item = this.targetsList.Items[i]?.ToString();
                this.targetsList.SetItemChecked(i, item != null && profile.Targets.Contains(item));
            }
        }

        private void SaveProfile(string? name = null)
		{
			name ??= this.profileSelector.Text.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				MessageBox.Show("Profile name is required.");
				return;
			}

			var existing = ConfigManager.Config.Profiles.FirstOrDefault(p => p.Name == name);
			if (existing != null)
			{
				var confirm = MessageBox.Show(
					$"Profile '{name}' already exists. Overwrite?",
					"Confirm Save",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question);

				if (confirm != DialogResult.Yes)
					return;
			}
			else
			{
				existing = new DeployProfile { Name = name };
				ConfigManager.Config.Profiles.Add(existing);
			}

			// Update profile fields
			existing.FolderPath = this.folderPathBox.Text.Trim();
			
			existing.ExtraFlags = this.extraFlagsBox.Text.Trim();
			existing.Targets = this.targetsList.CheckedItems.Cast<string>().ToList();

			ConfigManager.Config.LastUsedProfile = existing.Name;
			ConfigManager.Save();

			// Refresh selector and keep this profile selected
			PopulateProfileSelector(existing.Name, restoreLastUsed: false);
		}

		private void LoadProfileByName(string? name = null)
		{
			name ??= this.profileSelector.Text.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				MessageBox.Show("No profile selected to load.");
				return;
			}

			var profile = ConfigManager.Config.Profiles.FirstOrDefault(p => p.Name == name);
			if (profile == null)
			{
				MessageBox.Show($"Profile '{name}' not found.");
				return;
			}

			LoadProfile(profile);
			ConfigManager.Config.LastUsedProfile = profile.Name;
			ConfigManager.Save();

			// Mantener el selector en el perfil cargado
			PopulateProfileSelector(profile.Name, restoreLastUsed: false);
		}


       private void ApplyLanguage()
		{
			this.Text = "gFirebaseDeployer";
			this.profileGroup.Text = LanguageManager.Get("ProfileGroup");
			this.profileLabel.Text = LanguageManager.Get("SelectProfile");
			this.newButton.Text = LanguageManager.Get("NewProfile");
			this.loadButton.Text = LanguageManager.Get("LoadProfile");
			this.saveButton.Text = LanguageManager.Get("SaveProfile");
			this.deleteButton.Text = LanguageManager.Get("DeleteProfile");

			this.folderGroup.Text = LanguageManager.Get("FirebaseFolder");
			this.folderLabel.Text = LanguageManager.Get("FolderPath");
			this.browseButton.Text = LanguageManager.Get("Browse");

			this.targetsGroup.Text = LanguageManager.Get("DeployTargets");
			this.optionsGroup.Text = LanguageManager.Get("DeployOptions");
			this.flagsLabel.Text = LanguageManager.Get("ExtraFlags");

			this.startupCheckBox.Text = LanguageManager.Get("StartWithWindows");
			this.deployButton.Text = LanguageManager.Get("Deploy");
			this.statusLabel.Text = LanguageManager.Get("ConsoleOutput");

			// 👇 NUEVO: botones de consola
			this.copyConsoleButton.Text = LanguageManager.Get("CopyConsole");
			this.cleanConsoleButton.Text = LanguageManager.Get("CleanConsole");
		}


        private void ApplyTooltips()
		{
			this.tips = new ToolTip
			{
				InitialDelay = 200,
				ReshowDelay = 100,
				AutoPopDelay = 5000,
				ShowAlways = true
			};

			this.tips.Active = true; // ensure enabled

			this.tips.SetToolTip(this.targetsList, "Select the Firebase services to deploy.");
			this.tips.SetToolTip(this.extraFlagsBox, "Any extra CLI flags to pass.");
		}


        private void SetupTrayIcon()
        {
            this.trayIcon = new NotifyIcon
            {
                Text = "gFirebaseDeployer",
                Visible = true
            };

            try
			{
				using var stream = Assembly.GetExecutingAssembly()
					.GetManifestResourceStream("gFirebaseDeployer.Resources.AppIcon.ico");

				if (stream != null)
					this.trayIcon.Icon = new Icon(stream);
				else
					this.trayIcon.Icon = SystemIcons.Application;
			}
			catch
			{
				this.trayIcon.Icon = SystemIcons.Application;
			}


            this.trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (this.Visible && this.WindowState != FormWindowState.Minimized)
                        this.Hide();
                    else
                    {
                        this.Show();
                        this.WindowState = FormWindowState.Normal;
                    }
                }
            };

            this.trayIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add("Show", null, (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            });
            menu.Items.Add("Exit", null, (s, e) => AttemptExit());
            this.trayIcon.ContextMenuStrip = menu;
        }


        private void NewButton_Click(object? sender, EventArgs e)
		{
			NewProfile();
		}


        private void LoadButton_Click(object? sender, EventArgs e)
		{
			LoadProfileByName();
		}


       private void SaveButton_Click(object? sender, EventArgs e)
		{
			SaveProfile();
		}



        private void DeleteButton_Click(object? sender, EventArgs e)
		{
			DeleteProfile();
		}



        private void BrowseButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                this.folderPathBox.Text = dialog.SelectedPath;
        }


		private void TargetsList_ItemCheck(object? sender, ItemCheckEventArgs e)
		{
			string? item = this.targetsList.Items[e.Index]?.ToString();
			if (string.IsNullOrWhiteSpace(item))
				return;

			// Bloquear el separador
			if (item == "────────────")
			{
				e.NewValue = e.CurrentValue; // no permitir cambio
				return;
			}

			if (item.Equals("all", StringComparison.OrdinalIgnoreCase))
			{
				if (e.NewValue == CheckState.Checked)
				{
					for (int i = 0; i < this.targetsList.Items.Count; i++)
					{
						if (i != e.Index)
							this.targetsList.SetItemChecked(i, false);
					}
				}
			}
			else
			{
				if (e.NewValue == CheckState.Checked)
				{
					int allIndex = this.targetsList.Items.IndexOf("all");
					if (allIndex >= 0)
						this.targetsList.SetItemChecked(allIndex, false);
				}
			}
		}



		
		
        private void DeployButton_Click(object? sender, EventArgs e)
		{
			var name = this.profileSelector.Text.Trim();
			var profile = ConfigManager.Config.Profiles.FirstOrDefault(p => p.Name == name);
			if (profile == null)
			{
				MessageBox.Show("No profile selected.");
				return;
			}

			try
			{
				// ✅ Actualizar campos en memoria sin pedir confirmación
				profile.FolderPath = this.folderPathBox.Text.Trim();
				
				profile.ExtraFlags = this.extraFlagsBox.Text.Trim();
				profile.Targets = this.targetsList.CheckedItems.Cast<string>().ToList();

				// 🚨 Validación: al menos un target
				if (profile.Targets.Count == 0)
				{
					MessageBox.Show("Please select at least one deploy target or 'all'.",
									"No Targets Selected",
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning);
					return;
				}

				ConfigManager.Config.LastUsedProfile = profile.Name;
				ConfigManager.Save();

				// 🚀 Lanzar deploy
				var command = DeployManager.BuildCommand(profile);
				this.consoleBox.BackColor = Color.LightGray;
				this.consoleBox.ForeColor = Color.Black;
				this.consoleBox.Clear();
				StartSpinner();
				Application.DoEvents();
				isDeploying = true;
				DeployManager.ExecuteCommand(command, profile.FolderPath, OnDeployComplete, this);
			}
			catch (Exception ex)
			{
				isDeploying = false;
				StopSpinner();
				MessageBox.Show("Error: " + ex.Message);
			}
		}


				
        private void StartSpinner()
		{
			// Bloquear clicks sin deshabilitar el botón
			this.deployButton.Click -= DeployButton_Click;

			this.deployButton.BackColor = Color.Purple;
			this.deployButton.ForeColor = Color.White;

			deployAnimIndex = 0;
			this.deployButton.Text = deployFrames[deployAnimIndex];
			this.deployAnimTimer.Start();
		}



		private void StopSpinner()
		{
			this.deployAnimTimer.Stop();
			isDeploying = false;

			// Restaurar evento click
			this.deployButton.Click += DeployButton_Click;

			this.deployButton.Text = LanguageManager.Get("Deploy");
			this.deployButton.BackColor = Color.Green;
			this.deployButton.ForeColor = Color.White;
		}



        private void OnDeployComplete(bool success, string output)
		{
			if (InvokeRequired)
			{
				Invoke(() => OnDeployComplete(success, output));
				return;
			}

			isDeploying = false;
			StopSpinner();
			this.consoleBox.BackColor = success ? Color.Green : Color.Red;
			this.consoleBox.ForeColor = Color.White;
		}



		 private static readonly Regex AnsiRegex = new Regex(@"\x1B\[[0-9;]*[A-Za-z]", RegexOptions.Compiled);

        public void AppendConsoleLine(string line)
		{
			string clean = AnsiRegex.Replace(line, "");

			this.consoleBox.AppendText(clean + Environment.NewLine);
			this.consoleBox.SelectionStart = this.consoleBox.TextLength;
			this.consoleBox.ScrollToCaret();
		}



		private void OnTopCheckBox_CheckedChanged(object? sender, EventArgs e)
		{
			bool enable = this.onTopCheckBox.Checked;
			this.TopMost = enable;
			ConfigManager.Config.AlwaysOnTop = enable;
			ConfigManager.Save();
		}



        private void LanguageDropdown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isInitializing) return;

            string selected = this.languageDropdown.SelectedItem?.ToString() ?? "";
            string langCode = selected == "Español" ? "es" : "en";

            ConfigManager.Config.Language = langCode;
            ConfigManager.Save();
            LanguageManager.Load(langCode);
            ApplyLanguage();
        }

        private void StartupCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            var enable = this.startupCheckBox.Checked;
            StartupManager.SetStartup(enable);
            ConfigManager.Config.StartWithWindows = enable;
            ConfigManager.Save();
        }

        private void StatusBar_LinkClicked(object? s, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "https://github.com/gershu-ar/",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
		
		}
		
    }
}
