using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PseudoTV_Manager.Enum;
using PseudoTV_Manager.Properties;

namespace PseudoTV_Manager.Forms
{

    public partial class SettingsWindow : Form
    {
        public SettingsWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _addonDbFileDialog = new OpenFileDialog();
            _videoDbFileDialog = new OpenFileDialog();
            _settingsFileDialog = new OpenFileDialog();
            InitializeComponent();
        }

        private readonly MainWindow _mainWindow;

        public string User = Environment.UserName;
        private string _videoDbFile;
        private string _addonDbFile;
        private string _kodiVersionString;

        public KodiVersion KodiVersion = KodiVersion.Helix;

        private readonly OpenFileDialog _videoDbFileDialog;
        private readonly OpenFileDialog _settingsFileDialog;
        private readonly OpenFileDialog _addonDbFileDialog
            ;
        private void Button1_Click(System.Object sender, System.EventArgs e)
        {
            _kodiVersionString = XbmcVersion.SelectedIndex == 0 ? "XBMC" : "Kodi";

            _videoDbFileDialog.InitialDirectory = "C:\\Users\\" + User + "\\AppData\\Roaming\\" + _kodiVersionString + "\\userdata\\Database";

            _videoDbFileDialog.DefaultExt = "";
            _videoDbFileDialog.Filter = "SqliteDB files (*.db)|*MyVideos*.db";

            if ((_videoDbFileDialog.ShowDialog() == DialogResult.OK))
            {
                TxtVideoDbLocation.Text = _videoDbFileDialog.FileName;
            }
        }

        private void Button2_Click(System.Object sender, System.EventArgs e)
        {
            _settingsFileDialog.InitialDirectory = "C:\\Users\\" + User + "\\AppData\\Roaming\\" + _kodiVersionString + "\\userdata\\addon_data\\script.pseudotv.live";

            _settingsFileDialog.DefaultExt = "";
            _settingsFileDialog.Filter = "Settings2 files (*.xml)|**.xml";

            if ((_settingsFileDialog.ShowDialog() == DialogResult.OK))
            {
                TxtPseudoTvSettingsLocation.Text = _settingsFileDialog.FileName;
            }

        }

        private void Button4_Click(System.Object sender, System.EventArgs e)
        {
            _addonDbFileDialog.InitialDirectory = "C:\\Users\\" + User + "\\AppData\\Roaming\\" + _kodiVersionString + "\\userdata\\Database";

            _addonDbFileDialog.DefaultExt = "";
            _addonDbFileDialog.Filter = "SqliteDB files (*.db)|*Addons*.db";

            if ((_addonDbFileDialog.ShowDialog() == DialogResult.OK))
            {
                TxtAddonDatabaseLocation.Text = _addonDbFileDialog.FileName;
            }

        }


        private void Button3_Click(System.Object sender, System.EventArgs e)
        {
            //Dim SettingsFile As String = Application.StartupPath() & "\" & "Settings.txt"

            //See if there's already a text file in place, if not then create one.

            //If System.IO.File.Exists(SettingsFile) = False Then
            //System.IO.File.Create(SettingsFile)
            //End If

            //Verify that all 3 files indeed exist at least

            if (System.IO.File.Exists(TxtVideoDbLocation.Text) == true & System.IO.File.Exists(TxtPseudoTvSettingsLocation.Text) == true & System.IO.File.Exists(TxtAddonDatabaseLocation.Text) == true)
            {
                KodiVersion = PseudoTvUtils.GetKodiVersion(TxtVideoDbLocation.Text);


                if (PseudoTvUtils.TestMYSQLite(TxtVideoDbLocation.Text) == true)
                {
                    //Save them to the settings file
                    //Dim FilePaths As String = "0" & " | " & TxtVideoDbLocation.Text & " | " & TxtPseudoTvSettingsLocation.Text & " | " & TxtAddonDatabaseLocation.Text
                    //SaveFile(SettingsFile, FilePaths)

                    //Now, update the variables in the Main form with the proper paths
                    MainWindow.DatabaseType = 0;
                    Settings.Default.DatabaseType = 0;
                    MainWindow.VideoDatabaseLocation = TxtVideoDbLocation.Text;
                    Settings.Default.VideoDatabaseLocation = TxtVideoDbLocation.Text;
                    MainWindow.PseudoTvSettingsLocation = TxtPseudoTvSettingsLocation.Text;
                    Settings.Default.PseudoTvSettingsLocation = TxtPseudoTvSettingsLocation.Text;
                    MainWindow.AddonDatabaseLocation = TxtAddonDatabaseLocation.Text;
                    Settings.Default.AddonDatabaseLocation = TxtAddonDatabaseLocation.Text;
                    MainWindow.KodiVersion = KodiVersion;
                    Settings.Default.KodiVersion = (int)KodiVersion;
                    Settings.Default.Save();
                    //Refresh everything
                    MainWindow.RefreshAll();
                    MainWindow.RefreshTvGuide();

                    Visible = false;
                    //TODO: Pass instance of MainWindow to this class
                    _mainWindow.Focus();
                }

            }
            else if (!string.IsNullOrEmpty(TextBox3.Text) & !string.IsNullOrEmpty(TextBox4.Text) & !string.IsNullOrEmpty(TextBox6.Text) & System.IO.File.Exists(TxtPseudoTvSettingsLocation.Text) == true & System.IO.File.Exists(TxtAddonDatabaseLocation.Text) == true)
            {
                //server=localhost; user id=mike; password=12345; database=in_out

                dynamic connectionString = "server=" + TextBox3.Text + "; user id=" + TextBox4.Text + "; password=" + TextBox5.Text + "; database=" + TextBox6.Text + "; port=" + TextBox7.Text;


                if (PseudoTvUtils.TestMYSQL(connectionString) == true)
                {
                    //Dim FilePaths As String = "1" & " | " & ConnectionString & " | " & TxtPseudoTvSettingsLocation.Text & " | " & TxtAddonDatabaseLocation.Text
                    //SaveFile(SettingsFile, FilePaths)

                    //Now, update the variables in the Main form with the proper paths
                    MainWindow.DatabaseType = 1;
                    Settings.Default.DatabaseType = 1;
                    MainWindow.MySqlConnectionString = connectionString;
                    Settings.Default.MySQLConnectionString = connectionString;
                    MainWindow.PseudoTvSettingsLocation = TxtPseudoTvSettingsLocation.Text;
                    Settings.Default.PseudoTvSettingsLocation = TxtPseudoTvSettingsLocation.Text;
                    MainWindow.AddonDatabaseLocation = TxtAddonDatabaseLocation.Text;
                    Settings.Default.AddonDatabaseLocation = TxtAddonDatabaseLocation.Text;
                    MainWindow.KodiVersion = this.KodiVersion;
                    Settings.Default.KodiVersion = (int)KodiVersion;
                    Settings.Default.Save();
                    //Refresh everything
                    MainWindow.RefreshAll();
                    MainWindow.RefreshTvGuide();

                    this.Visible = false;
                    _mainWindow.Focus();
                }
            }
        }


        private void Form6_Load(object sender, System.EventArgs e)
        {
            switch (KodiVersion)
            {
                case KodiVersion.Gotham:
                    XbmcVersion.SelectedIndex = 0;
                    break;
                case KodiVersion.Helix:
                    XbmcVersion.SelectedIndex = 1;
                    break;
                case KodiVersion.Isengard:
                    XbmcVersion.SelectedIndex = 2;
                    break;
                case KodiVersion.Jarvis:
                    XbmcVersion.SelectedIndex = 3;
                    break;
            }

            if (!string.IsNullOrEmpty(MainWindow.VideoDatabaseLocation))
            {
                TxtVideoDbLocation.Text = MainWindow.VideoDatabaseLocation;
                TxtPseudoTvSettingsLocation.Text = MainWindow.PseudoTvSettingsLocation;
                TxtAddonDatabaseLocation.Text = MainWindow.AddonDatabaseLocation;
            }
            else
            {
                FindKodiSettings();
            }

            if (!string.IsNullOrEmpty(MainWindow.MySqlConnectionString))
            {
                var splitString = MainWindow.MySqlConnectionString.Split(';');

                TxtPseudoTvSettingsLocation.Text = MainWindow.PseudoTvSettingsLocation;
                TextBox3.Text = splitString[0].Split(Convert.ToChar("server="))[1];
                TextBox4.Text = splitString[1].Split(Convert.ToChar("user id="))[1];
                TextBox5.Text = splitString[2].Split(Convert.ToChar("password="))[1];
                TextBox6.Text = splitString[3].Split(Convert.ToChar("database="))[1];
                TextBox7.Text = splitString[4].Split(Convert.ToChar("port="))[1];
            }

        }

        private void FindKodiSettings()
        {
            var folderKodi = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\kodi\\userdata";
            var folderXbmc = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.xbmc\\userdata";
            string databaseFolder = null;
            string addonDataFolder = null;

            if ((Directory.Exists(folderKodi)))
            {
                databaseFolder = folderKodi + "\\Database";
                addonDataFolder = folderKodi + "\\addon_data";
            }
            else if ((Directory.Exists(folderXbmc)))
            {
                databaseFolder = folderXbmc + "\\Database";
                addonDataFolder = folderXbmc + "\\addon_data";
            }
            else
            {
                return;
            }

            var regex = new Regex("(Addons|MyVideos)(\\d+).db");
            var databaseDir = new DirectoryInfo(databaseFolder);
            var filelist = databaseDir.GetFiles();

            foreach (var file in filelist)
            {
                var match = regex.Match(file.Name);
                if (match.Success)
                {
                    if ((match.Groups[1].Value == "MyVideos"))
                    {
                        KodiVersion = PseudoTvUtils.GetKodiVersion(file.Name);
                        TxtVideoDbLocation.Text = file.FullName;
                    }
                    else if ((match.Groups[1].Value == "Addons"))
                    {
                        TxtAddonDatabaseLocation.Text = file.FullName;
                    }
                }
            }

            //C:\Users\Scott\AppData\Roaming\Kodi\userdata\addon_data\script.pseudotv.live\settings2.xml
            if ((System.IO.File.Exists(addonDataFolder + "\\script.pseudotv.live\\settings2.xml")))
            {
                TxtPseudoTvSettingsLocation.Text = addonDataFolder + "\\script.pseudotv.live\\settings2.xml";
            }

            switch (KodiVersion)
            {
                case KodiVersion.Gotham:
                    XbmcVersion.SelectedIndex = 0;
                    break;
                case KodiVersion.Helix:
                    XbmcVersion.SelectedIndex = 1;
                    break;
                case KodiVersion.Isengard:
                    XbmcVersion.SelectedIndex = 2;
                    break;
                case KodiVersion.Jarvis:
                    XbmcVersion.SelectedIndex = 3;
                    break;
            }
        }

        public object ReadVersion(string genreName)
        {
            //This looks up the Genre based on the name and returns the proper Genre ID

            int genreId = 0;

            var selectArray = new[] { 0 };

            //Shoot it over to the ReadRecord sub
            var returnArray = PseudoTvUtils.DbReadRecord(TxtVideoDbLocation.Text, "SELECT idVersion FROM KodiVersion", selectArray);

            //The ID # is all we need.
            //Just make sure it's not a null reference.
            if (returnArray == null)
            {
                PseudoTvUtils.ShowInputDialog("Could not find version of Kodi installed! KodiVersion table didn't return an idVersion");
            }
            else
            {
                genreId = Convert.ToInt32(returnArray[0]);
            }

            return genreId;
        }
    }
}
