using System.Windows.Forms;

namespace gFirebaseDeployer.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;


		// Footer
		private Panel statusBarPanel;
		private Label firebaseStatusLabel;

		// Profile controls
		private GroupBox profileGroup;
		private Label profileLabel;
		private ComboBox profileSelector;
		private Button newButton;
		private Button loadButton;
		private Button saveButton;
		private Button deleteButton;

		// Console buttons
		private Button copyConsoleButton;
		private Button cleanConsoleButton;

		// Folder controls
		private GroupBox folderGroup;
		private Label folderLabel;
		private TextBox folderPathBox;
		private Button browseButton;

		// Targets
		private GroupBox targetsGroup;
		private CheckedListBox targetsList;

		// Deploy options
		private GroupBox optionsGroup;
		private Label flagsLabel;
		private TextBox extraFlagsBox;
		private ComboBox languageDropdown; // ÚNICO dropdown, dentro de optionsGroup

		// Misc controls
		private CheckBox startupCheckBox;
		private CheckBox onTopCheckBox;
		private Button deployButton;
		private Label statusLabel;

		// Console output
		private Panel consolePanel;
		private RichTextBox consoleBox = null!;


		// Footer
		private LinkLabel statusBar;


        private void InitializeComponent()
{
    this.components = new System.ComponentModel.Container();

    // Profile Group
    this.profileGroup = new GroupBox();
    this.profileLabel = new Label();
    this.profileSelector = new ComboBox();
    this.newButton = new Button();
    this.loadButton = new Button();
    this.saveButton = new Button();
    this.deleteButton = new Button();

    this.profileGroup.Text = "Profile";
    this.profileGroup.Location = new System.Drawing.Point(12, 12);
    this.profileGroup.Size = new System.Drawing.Size(460, 60);

    this.profileLabel.Text = "Select:";
    this.profileLabel.Location = new System.Drawing.Point(10, 25);
    this.profileLabel.Size = new System.Drawing.Size(45, 15);

    this.profileSelector.Location = new System.Drawing.Point(60, 22);
    this.profileSelector.Size = new System.Drawing.Size(140, 23);

    this.newButton.Text = "New";
    this.newButton.Location = new System.Drawing.Point(210, 21);
    this.newButton.Size = new System.Drawing.Size(50, 25);

    this.loadButton.Text = "Load";
    this.loadButton.Location = new System.Drawing.Point(265, 21);
    this.loadButton.Size = new System.Drawing.Size(50, 25);

    this.saveButton.Text = "Save";
    this.saveButton.Location = new System.Drawing.Point(320, 21);
    this.saveButton.Size = new System.Drawing.Size(57, 25);

    this.deleteButton.Text = "Delete";
    this.deleteButton.Location = new System.Drawing.Point(385, 21);
    this.deleteButton.Size = new System.Drawing.Size(55, 25);

    this.profileGroup.Controls.Add(this.profileLabel);
    this.profileGroup.Controls.Add(this.profileSelector);
    this.profileGroup.Controls.Add(this.newButton);
    this.profileGroup.Controls.Add(this.loadButton);
    this.profileGroup.Controls.Add(this.saveButton);
    this.profileGroup.Controls.Add(this.deleteButton);

    // Folder Group
    this.folderGroup = new GroupBox();
    this.folderLabel = new Label();
    this.folderPathBox = new TextBox();
    this.browseButton = new Button();

    this.folderGroup.Text = "Firebase Folder";
    this.folderGroup.Location = new System.Drawing.Point(12, 80);
    this.folderGroup.Size = new System.Drawing.Size(460, 60);

    this.folderLabel.Text = "Path:";
    this.folderLabel.Location = new System.Drawing.Point(10, 25);
    this.folderLabel.Size = new System.Drawing.Size(40, 15);

    this.folderPathBox.Location = new System.Drawing.Point(60, 22);
    this.folderPathBox.Size = new System.Drawing.Size(320, 23);

    this.browseButton.Text = "Browse";
    this.browseButton.Location = new System.Drawing.Point(390, 21);
    this.browseButton.Size = new System.Drawing.Size(60, 25);

    this.folderGroup.Controls.Add(this.folderLabel);
    this.folderGroup.Controls.Add(this.folderPathBox);
    this.folderGroup.Controls.Add(this.browseButton);

    // Targets Group
    this.targetsGroup = new GroupBox();
    this.targetsList = new CheckedListBox();

    this.targetsGroup.Text = "Deploy Targets";
    this.targetsGroup.Location = new System.Drawing.Point(12, 150);
    this.targetsGroup.Size = new System.Drawing.Size(220, 140);

    this.targetsList.Location = new System.Drawing.Point(10, 20);
    this.targetsList.Size = new System.Drawing.Size(200, 110);
    this.targetsList.ItemHeight = 18;

    this.targetsList.Items.AddRange(new object[] {
        "all",
        "────────────",
        "hosting", "functions", "firestore", "database", "storage",
        "extensions", "remoteconfig", "messaging", "pubsub", "emulators"
    });

    this.targetsList.ItemCheck += new ItemCheckEventHandler(this.TargetsList_ItemCheck);

    this.targetsGroup.Controls.Add(this.targetsList);

    // Options Group
    this.optionsGroup = new GroupBox();
    this.flagsLabel = new Label();
    this.extraFlagsBox = new TextBox();
    this.languageDropdown = new ComboBox();

    this.optionsGroup.Text = "Deploy Options";
    this.optionsGroup.Location = new System.Drawing.Point(252, 150);
    this.optionsGroup.Size = new System.Drawing.Size(220, 140);

	// Language Label
	Label languageLabel = new Label();
	languageLabel.Text = "Language — Idioma";
	languageLabel.Location = new System.Drawing.Point(10, 80);
	languageLabel.Size = new System.Drawing.Size(200, 15);

	this.optionsGroup.Controls.Add(languageLabel);


    // Extra Flags
    this.flagsLabel.Text = "Extra Flags:";
    this.flagsLabel.Location = new System.Drawing.Point(10, 25);
    this.flagsLabel.Size = new System.Drawing.Size(100, 15);

    this.extraFlagsBox.Location = new System.Drawing.Point(10, 45);
    this.extraFlagsBox.Size = new System.Drawing.Size(200, 23);

    // Language Dropdown (antiguo)
    //this.languageDropdown.Location = new System.Drawing.Point(10, 80);
    //this.languageDropdown.Size = new System.Drawing.Size(200, 23);
    //this.languageDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
    //this.languageDropdown.Items.Add("English");
    //this.languageDropdown.Items.Add("Español");

    this.optionsGroup.Controls.Add(this.flagsLabel);
    this.optionsGroup.Controls.Add(this.extraFlagsBox);
    this.optionsGroup.Controls.Add(this.languageDropdown);

    // Startup Checkbox
    this.startupCheckBox = new CheckBox();
    this.startupCheckBox.Location = new System.Drawing.Point(12, 300);
    this.startupCheckBox.Size = new System.Drawing.Size(150, 20);
	
	// On Top Checkbox
	this.onTopCheckBox = new CheckBox();
	this.onTopCheckBox.Text = "On top";
	this.onTopCheckBox.Location = new System.Drawing.Point(12, 325); // 👈 debajo del startup
	this.onTopCheckBox.Size = new System.Drawing.Size(150, 20);
	this.onTopCheckBox.CheckedChanged += new System.EventHandler(this.OnTopCheckBox_CheckedChanged);

    // Copy Console Button
    this.copyConsoleButton = new Button();
    this.copyConsoleButton.Text = "Copy Console";
    this.copyConsoleButton.Location = new System.Drawing.Point(260, 298);
    this.copyConsoleButton.Size = new System.Drawing.Size(100, 23);
    this.copyConsoleButton.Click += new System.EventHandler(this.CopyConsoleButton_Click);

    // Clean Console Button
    this.cleanConsoleButton = new Button();
    this.cleanConsoleButton.Text = "Clean Console";
    this.cleanConsoleButton.Location = new System.Drawing.Point(370, 298);
    this.cleanConsoleButton.Size = new System.Drawing.Size(100, 23);
    this.cleanConsoleButton.Click += new System.EventHandler(this.CleanConsoleButton_Click);

    // Deploy Button
    this.deployButton = new Button();
    this.deployButton.Location = new System.Drawing.Point(180, 330);
    this.deployButton.Size = new System.Drawing.Size(120, 30);
    this.deployButton.BackColor = System.Drawing.Color.Green;
    this.deployButton.ForeColor = System.Drawing.Color.White;
    this.deployButton.FlatStyle = FlatStyle.Flat;
    this.deployButton.UseVisualStyleBackColor = false;

    // Status Label
    this.statusLabel = new Label();
    this.statusLabel.Text = "Console Output";
    this.statusLabel.Location = new System.Drawing.Point(12, 370);
    this.statusLabel.Size = new System.Drawing.Size(100, 15);

    // Console Panel
    this.consolePanel = new Panel();
    this.consolePanel.Location = new System.Drawing.Point(12, 390);
    this.consolePanel.Size = new System.Drawing.Size(460, 120);
    this.consolePanel.BackColor = Color.Black;
    this.consolePanel.BorderStyle = BorderStyle.FixedSingle;

    // Console Box
    this.consoleBox = new RichTextBox();
    this.consoleBox.Location = new System.Drawing.Point(0, 0);
    this.consoleBox.Size = new System.Drawing.Size(440, 100);
    this.consoleBox.ReadOnly = true;
    this.consoleBox.BackColor = Color.Black;
    this.consoleBox.ForeColor = Color.Lime;
    this.consoleBox.BorderStyle = BorderStyle.None;
    this.consoleBox.Dock = DockStyle.Fill;
    this.consoleBox.Margin = Padding.Empty;

    this.consolePanel.Controls.Add(this.consoleBox);

    // Status Bar Panel
    this.statusBarPanel = new Panel();
    this.statusBarPanel.Location = new System.Drawing.Point(12, 515);
    this.statusBarPanel.Size = new System.Drawing.Size(460, 15);
    this.statusBarPanel.BackColor = System.Drawing.Color.Transparent;

    // Left label (Firebase version)
    this.firebaseStatusLabel = new Label();
    this.firebaseStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
    this.firebaseStatusLabel.AutoSize = false;
    this.firebaseStatusLabel.Dock = DockStyle.Left;
    this.firebaseStatusLabel.Width = 250;

    // Right link label (credits)
    this.statusBar = new LinkLabel();
    this.statusBar.Text = "gFirebaseDeployer 1.0 - by gershu";
    this.statusBar.LinkArea = new LinkArea(27, 6);
    this.statusBar.TextAlign = ContentAlignment.MiddleRight;
    this.statusBar.AutoSize = false;
    this.statusBar.Dock = DockStyle.Fill;
    this.statusBar.LinkClicked += new LinkLabelLinkClickedEventHandler(this.StatusBar_LinkClicked);
	
	// Language Dropdown
	this.languageDropdown.Location = new System.Drawing.Point(10, 100);
	this.languageDropdown.Size = new System.Drawing.Size(120, 23);
	this.languageDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
	this.languageDropdown.Items.Add("English");
	this.languageDropdown.Items.Add("Español");


    // Add to status panel
    this.statusBarPanel.Controls.Add(this.statusBar);
    this.statusBarPanel.Controls.Add(this.firebaseStatusLabel);

    // Add controls to form
    this.Controls.Add(this.profileGroup);
    this.Controls.Add(this.folderGroup);
    this.Controls.Add(this.targetsGroup);
    this.Controls.Add(this.optionsGroup);
    this.Controls.Add(this.startupCheckBox);
    this.Controls.Add(this.copyConsoleButton);
    this.Controls.Add(this.cleanConsoleButton);
    this.Controls.Add(this.deployButton);
    this.Controls.Add(this.statusLabel);
    this.Controls.Add(this.consolePanel);
    this.Controls.Add(this.statusBarPanel);
	this.Controls.Add(this.onTopCheckBox);


    // Event hookups
    this.newButton.Click += new System.EventHandler(this.NewButton_Click);
    this.loadButton.Click += new System.EventHandler(this.LoadButton_Click);
    this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.deleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
    this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
    this.deployButton.Click += new System.EventHandler(this.DeployButton_Click);
    this.languageDropdown.SelectedIndexChanged += new System.EventHandler(this.LanguageDropdown_SelectedIndexChanged);
    this.startupCheckBox.CheckedChanged += new System.EventHandler(this.StartupCheckBox_CheckedChanged);

    // Form settings
    this.ClientSize = new System.Drawing.Size(484, 540);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "gFirebaseDeployer";
    this.Resize += new System.EventHandler(this.MainForm_Resize);

    this.ResumeLayout(false);
    this.PerformLayout();
}

    }
}

