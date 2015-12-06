using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
//Import the SQLite DLL file.
//C:\SQLite\THERE!

using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using NLog;

namespace PseudoTV_Manager
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //For sorting columns in listviews
        private ColumnHeader m_SortingColumn;

        private ColumnHeader m_SortingColumn2;
        public static int DatabaseType;
        public static string MySQLConnectionString;
        public static string VideoDatabaseLocation;
        public string PseudoTvSettingsLocation;
        public static string AddonDatabaseLocation;
        public int XbmcVersion;
        public string UserDataFolder;
        public string PluginNotInclude;
        public string YouTubeMulti;
        private int _resetHours;

        public KodiVersion KodiVersion = KodiVersion.Helix;

        protected Logger Logger = LogManager.GetCurrentClassLogger();

        public object LookUpGenre(string GenreName)
        {
            //This looks up the Genre based on the name and returns the proper Genre ID

            string GenreID = null;

            var SelectArray = new[] { 0 };

            string genrePar = "name";
            if ((KodiVersion < KodiVersion.Isengard))
            {
                genrePar = "strGenre";
            }

            //Shoot it over to the ReadRecord sub
            string[] ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                "SELECT * FROM genre where " + genrePar + "='" + GenreName + "'", SelectArray);

            //The ID # is all we need.
            //Just make sure it's not a null reference.
            if (ReturnArray == null)
            {
                MessageBox.Show("nothing!");
            }
            else
            {
                GenreID = ReturnArray[0];
            }

            return GenreID;
        }

        public object LookUpNetwork(string Network)
        {
            //This looks up the Network name and returns the proper Network ID

            string NetworkID = null;

            var SelectArray = new[] { 0 };

            string studioPar = "name";
            if ((KodiVersion < KodiVersion.Isengard))
            {
                studioPar = "strStudio";
            }

            //Shoot it over to the ReadRecord sub
            var ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                "SELECT * FROM studio where " + studioPar + "='" + Network + "'", SelectArray);

            //The ID # is all we need.
            //Just make sure it's not a null reference.
            if (ReturnArray == null)
            {
            }
            else
            {
                NetworkID = ReturnArray[0];
            }
            return NetworkID;

        }

        public void RefreshTVGuide()
        {
            //Clear the TV name and the List items
            TVGuideShowName.Text = "";
            TVGuideList.Items.Clear();

            int TotalChannels = 0;

            //This will hold an array of our channel #s
            string[] ChannelArray = null;
            int ChannelNum = 0;

            string FILE_LOCATION = PseudoTvSettingsLocation;

            if (System.IO.File.Exists(PseudoTvSettingsLocation) == true)
            {
                //Load everything into the FullFile string
                var FullFile = PseudoTvUtils.ReadFile(PseudoTvSettingsLocation);

                System.IO.StreamReader objReader = new System.IO.StreamReader(FILE_LOCATION);

                //Loop through each line individually, then add the channel # to an array

                while (objReader.Peek() != -1)
                {
                    var SingleLine = objReader.ReadLine();

                    if (SingleLine.Contains("_type" + (char)34 + " value="))
                    {
                        var Part1 = SingleLine.Split(Convert.ToChar("_type"))[0];
                        var Part2 = Part1.Split('_')[1];

                        Array.Resize(ref ChannelArray, ChannelNum + 1);
                        ChannelNum = ChannelNum + 1;
                        ChannelArray[ChannelArray.Length] = Part2;

                    }

                }

                objReader.Close();


                for (var x = 0; x <= ChannelArray.Length; x++)
                {
                    string[] ChannelInfo = null;

                    string ChannelRules = "";
                    string ChannelRulesAdvanced = "";
                    string ChannelRulesCount = "";
                    string ChannelType = "";
                    string ChannelTypeDetail = "";
                    string ChannelTime = "";
                    string ChannelTypeDetail2 = "";
                    string ChannelTypeDetail3 = "";
                    string ChannelTypeDetail4 = "";

                    //Grab everything that says setting id = Channel #
                    ChannelInfo = FullFile.Split(
                        new[] { "<setting id=" + (char)34 + "Channel_" + ChannelArray[x] + "_" },
                        StringSplitOptions.RemoveEmptyEntries);

                    //Now loop through everything it returned.
                    for (var y = 1; y <= ChannelInfo.Length - 1; y++)
                    {
                        string RuleType = null;
                        string RuleValue = null;

                        RuleType = ChannelInfo[y].Split((char)34)[0];

                        RuleValue = ChannelInfo[y].Split(Convert.ToChar("value=" + (char)34))[1];
                        RuleValue = RuleValue.Split((char)34)[0];


                        if (RuleType == "changed")
                        {
                        }
                        else if (RuleType == "rulecount")
                        {
                            ChannelRulesCount = RuleValue;
                        }
                        else if (RuleType == "time")
                        {
                            ChannelTime = RuleValue;
                        }
                        else if (RuleType == "type")
                        {
                            //Update the Channel type to the value of that.
                            ChannelType = RuleValue;
                        }
                        else if (RuleType == "1")
                        {
                            //Gets more information on what type the channel is, playlist location/genre/zap2it/etc.
                            ChannelTypeDetail = RuleValue;
                        }
                        else if (RuleType == "2")
                        {
                            //Gets (LiveTV-8)stream source/(IPTV-9)iptv source/(Youtube-10)youtube channel type/
                            //(Rss-11)reserved/(LastFM-13)LastFM User/(BTP/Cinema Experience14)filter types or smart playlist/
                            //(Direct/SF-15, Direct Playon-16)exclude list/
                            ChannelTypeDetail2 = RuleValue;
                        }
                        else if (RuleType == "3")
                        {
                            //Gets (LiveTV-8)xmltv filename/(IPTV-9)show titles/(Youtube-10, Rss-11, LastFM-13)media limits/
                            //(BTP/Cinema Experience-14)parsing resolution/(Direct/SF-15, Direct Playon-16)file limit
                            ChannelTypeDetail3 = RuleValue;
                        }
                        else if (RuleType == "4")
                        {
                            //Gets (IPTV-9)show description/(Youtube-10, Rss-11, LastFM-13)sort ordering/
                            //(BTP/Cinema Experience-14)years to parse by or unused/(Direct/SF-15, Direct Playon-16)sort ordering
                            ChannelTypeDetail4 = RuleValue;
                        }
                        else if (RuleType.Contains("rule"))
                        {
                            //Okay, It's rule information.

                            //Get the rule number.
                            string RuleNumber = null;
                            RuleNumber = RuleType.Split(Convert.ToChar("rule_"))[1];
                            RuleNumber = RuleNumber.Split('_')[0];

                            if (RuleType.Contains("opt"))
                            {
                                //Okay, it's an actual option tied to another rule.

                                var OptNumber = RuleType.Split(Convert.ToChar("_opt_"))[1];
                                RuleNumber = RuleType.Split(Convert.ToChar("_opt_"))[0];
                                RuleNumber = RuleNumber.Split(Convert.ToChar("rule_"))[1];

                                //MsgBox("Opt : " & RuleNumber & " | " & OptNumber & " | " & RuleValue)
                                //ChannelRulesAdvanced = ChannelRulesAdvanced & "~" & RuleNumber & "|" & OptNumber & "|" & RuleValue
                                //MsgBox(RuleNumber & " | " & OptNumber & " | " & RuleValue)

                                //Add this to the previous rule, remove the ending 
                                //Then add this rule as Rule#:RuleValue
                                ChannelRules = ChannelRules + "|" + OptNumber + "^" + RuleValue;
                            }
                            else
                            {
                                ChannelRules = ChannelRules + "~" + RuleNumber + "|" + RuleValue;
                            }

                        }
                        else
                        {
                        }

                        //End result for a basic option:  ~RuleNumber|RuleValue 
                        //End result for an advanced option:  ~RuleNumber|RuleValue|Rule1^Rule1Value|Rule2^Rule2Value

                    }

                    string[] str = new string[11];

                    str[0] = ChannelArray[x];
                    //Channel #.  
                    str[1] = ChannelType;
                    str[2] = ChannelTypeDetail;
                    str[3] = ChannelTime;
                    str[4] = ChannelRules;
                    str[5] = ChannelRulesCount;
                    str[6] = ChannelTypeDetail2;
                    str[7] = ChannelTypeDetail3;
                    str[8] = ChannelTypeDetail4;

                    ListViewItem itm = default(ListViewItem);
                    itm = new ListViewItem(str);
                    //Add to list
                    TVGuideList.Items.Add(itm);

                }

            }

            //Sort List
            TVGuideList.ListViewItemSorter = new ClsListviewSorter(0, SortOrder.Ascending);
            // Sort. 
            TVGuideList.Sort();
        }

        public void RefreshGenres()
        {
            GenresList.Items.Clear();
            var SelectArrayMain = new[] { 0, 1 };


            //Shoot it over to the ReadRecord sub
            string[] ReturnArrayMain = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation, "SELECT * FROM genre",
                SelectArrayMain);

            //Loop through and read the name


            for (var x = 0; x <= ReturnArrayMain.Length; x++)
            {
                //Sort them into an array
                var splitItem = ReturnArrayMain[x].Split('~');
                //Position 0 = genre ID
                //Position 1 = genre name

                //Push array into ListViewItem

                var selectArray = new[] { 1 };

                //Now, grab a list of all the shows that match the GenreID
                string[] returnArray = null;
                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genre_link WHERE genre_id='" + splitItem[0] + "' AND media_type = 'tvshow'",
                        selectArray);
                }
                else
                {
                    returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genrelinktvshow WHERE idGenre='" + splitItem[0] + "'", selectArray);
                }

                //This will grab the number of movies.
                string[] ReturnArray2 = null;
                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    ReturnArray2 = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genre_link WHERE genre_id='" + splitItem[0] + "' AND media_type = 'movie'",
                        selectArray);
                }
                else
                {
                    ReturnArray2 = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genrelinkmovie WHERE idGenre='" + splitItem[0] + "'", selectArray);
                }

                int ShowNum;
                int MovieNum;

                //This is the total number of shows that match this genre.
                //Also, verify the returning array is something, not null before proceeding.
                if (returnArray == null)
                {
                    ShowNum = 0;
                }
                else
                {
                    ShowNum = returnArray.Length;
                }

                if (ReturnArray2 == null)
                {
                    MovieNum = 0;
                }
                else
                {
                    MovieNum = ReturnArray2.Length;
                }

                string[] str = new string[5];
                //Genre Name
                //# of shows in genre
                //# of movies in genre
                //Total of both /\
                //Genre ID

                str[0] = splitItem[1];
                str[1] = ShowNum.ToString();
                str[2] = MovieNum.ToString();
                str[3] = (ShowNum + MovieNum).ToString();
                str[4] = splitItem[0];


                ListViewItem itm = default(ListViewItem);
                itm = new ListViewItem(str);
                //Add to list
                GenresList.Items.Add(itm);

            }

            GenresList.Sort();
        }


        public void RefreshTVShows()
        {
            TVShowList.Items.Clear();

            //Set an array with the columns you want returned
            var SelectArray = new[] { 1, 15, 0 };

            //Shoot it over to the ReadRecord sub, 
            string[] ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation, "SELECT * FROM tvshow ORDER BY c00",
                SelectArray);

            //Now, read the output of the array.
            //Loop through each of the Array items.

            for (var x = 0; x <= ReturnArray.Length - 1; x++)
            {
                //Split them by ~'s.  This is how we seperate the rows in the single-element.
                string[] str = ReturnArray[x].Split('~');

                //Now take that split string and make it an item.
                ListViewItem itm = default(ListViewItem);
                itm = new ListViewItem(str);

                //Add the item to the TV show list.
                TVShowList.Items.Add(itm);
            }
        }

        private void RefreshPlugins()
        {
            PluginType.Items.Clear();

            string addonLike = "plugin.video";
            var SelectArray = new[] { 1 };

            //Grab the Plugin List
            var ReturnArray = PseudoTvUtils.ReadPluginRecord(AddonDatabaseLocation,
                "SELECT DISTINCT addon.addonID, addon.name FROM addon, package WHERE addon.addonID = package.addonID and addon.addonID LIKE '" +
                addonLike + "%'", SelectArray);

            for (var x = 0; x <= ReturnArray.Length - 1; x++)
            {
                //Split them by ~'s.  This is how we seperate the rows in the single-element.
                var str = ReturnArray[x].Split('~');

                //Add the item to the Plugins List
                PluginType.Items.Add(str[0]);
            }

        }

        public void RefreshMovieList()
        {
            MovieList.Items.Clear();

            //Set an array with the columns you want returned
            var SelectArray = new[] { 2, 0 };

            //Shoot it over to the ReadRecord sub, 
            string[] ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation, "SELECT * FROM movie ORDER BY c00",
                SelectArray);

            //Now, read the output of the array.
            //Loop through each of the Array items.

            for (var x = 0; x <= ReturnArray.Length - 1; x++)
            {
                //Split them by ~'s.  This is how we seperate the rows in the single-element.
                string[] str = ReturnArray[x].Split('~');

                //Now take that split string and make it an item.
                ListViewItem itm = default(ListViewItem);
                itm = new ListViewItem(str);

                //Add the item to the TV show list.
                MovieList.Items.Add(itm);
            }
        }


        private void MainWindow_Load(object sender, System.EventArgs e)
        {
            HelpList.SelectedIndex = 0;

            RefreshSettings();

            TVShowList.Columns.Add("Show", 100, HorizontalAlignment.Left);
            TVShowList.Columns.Add("Network", 100, HorizontalAlignment.Left);
            TVShowList.Columns.Add("ID", 0, HorizontalAlignment.Left);

            MovieList.Columns.Add("Movie", 300, HorizontalAlignment.Left);
            MovieList.Columns.Add("ID", 0, HorizontalAlignment.Left);

            NetworkList.Columns.Add("Network", 140, HorizontalAlignment.Left);
            NetworkList.Columns.Add("# Shows", 60, HorizontalAlignment.Left);

            MovieNetworkList.Columns.Add("Studio", 170, HorizontalAlignment.Left);
            MovieNetworkList.Columns.Add("# Movies", 60, HorizontalAlignment.Left);

            GenresList.Columns.Add("Genre", 100, HorizontalAlignment.Left);
            GenresList.Columns.Add("# Shows", 60, HorizontalAlignment.Center);
            GenresList.Columns.Add("# Movies", 60, HorizontalAlignment.Center);
            GenresList.Columns.Add("# Total", 80, HorizontalAlignment.Center);
            GenresList.Columns.Add("Genre ID", 0, HorizontalAlignment.Left);

            TVGuideList.Columns.Add("Channel", 200, HorizontalAlignment.Left);
            TVGuideList.Columns.Add("Type", 0, HorizontalAlignment.Left);
            TVGuideList.Columns.Add("TypeDetail", 0, HorizontalAlignment.Left);
            TVGuideList.Columns.Add("Time", 0, HorizontalAlignment.Left);
            TVGuideList.Columns.Add("Rules", 0, HorizontalAlignment.Left);
            TVGuideList.Columns.Add("RuleCount", 0, HorizontalAlignment.Left);

            InterleavedList.Columns.Add("Chan", 50, HorizontalAlignment.Left);
            InterleavedList.Columns.Add("Min", 45, HorizontalAlignment.Left);
            InterleavedList.Columns.Add("Max", 45, HorizontalAlignment.Left);
            InterleavedList.Columns.Add("Epi", 45, HorizontalAlignment.Left);
            InterleavedList.Columns.Add("Epis", 45, HorizontalAlignment.Left);

            SchedulingList.Columns.Add("Chan", 53, HorizontalAlignment.Left);
            SchedulingList.Columns.Add("Days", 45, HorizontalAlignment.Left);
            SchedulingList.Columns.Add("Time", 45, HorizontalAlignment.Left);
            SchedulingList.Columns.Add("Epi", 45, HorizontalAlignment.Left);

            TVGuideSubMenu.Columns.Add("Shows / Movies", 300, HorizontalAlignment.Left);

        }


        public void RefreshSettings()
        {
            //TODO:
            //My.Settings.Reload();
            //Settings.txt location
            //Dim SettingsFile As String = Application.StartupPath() & "\" & "Settings.txt"

            //See if there's already a text file in place, if so load it.
            //    If System.IO.File.Exists(SettingsFile) = True Then
            //        Dim FileLocations = ReadFile(SettingsFile)

            //        'Make sure there's the | symbol there so we can split it
            //        If InStr(FileLocations, " | ") Then
            //            FileLocations = Split(FileLocations, " | ")

            //            'Now count the split and make sure it has the proper amount.
            //            If UBound(FileLocations) = 3 Then

            //                If FileLocations[0] = "0" Then
            //                    'This is for a standard SQLite Entry.
            //                    DatabaseType = 0
            //                    VideoDatabaseLocation = FileLocations[1]
            //                    PseudoTvSettingsLocation = FileLocations[2]
            //		AddonDatabaseLocation = FileLocations[3]
            //		'get Kodi KodiVersion based on video db KodiVersion
            //		Version = GetKodiVersion(VideoDatabaseLocation)
            //                Else
            //                    DatabaseType = 1
            //                    MySQLConnectionString = FileLocations[1]
            //                    PseudoTvSettingsLocation = FileLocations[2]
            //                    AddonDatabaseLocation = FileLocations[3]
            //	End If

            //End If

            //            RefreshALL()
            //            RefreshTVGuide()

            //        End If
            //if (!(My.Settings.VideoDatabaseLocation == "False") &
            //    !string.IsNullOrEmpty(My.Settings.VideoDatabaseLocation) &
            //    !string.IsNullOrEmpty(My.Settings.PseudoTvSettingsLocation) &
            //    !string.IsNullOrEmpty(My.Settings.AddonDatabaseLocation))
            //{
            //    DatabaseType = 0;
            //    VideoDatabaseLocation = My.Settings.VideoDatabaseLocation;
            //    _logger.Debug(VideoDatabaseLocation);
            //    PseudoTvSettingsLocation = My.Settings.PseudoTvSettingsLocation;
            //    _logger.Debug(PseudoTvSettingsLocation);
            //    AddonDatabaseLocation = My.Settings.AddonDatabaseLocation;
            //    _logger.Debug(AddonDatabaseLocation);
            //    this.Version = My.Settings.Version;
            //    _logger.Debug(this.Version);
            //    RefreshALL();
            //    RefreshTVGuide();
            //}
            //else if (!(My.Settings.VideoDatabaseLocation == "False") &
            //         !string.IsNullOrEmpty(My.Settings.MySQLConnectionString) &
            //         !string.IsNullOrEmpty(My.Settings.PseudoTvSettingsLocation) &
            //         !string.IsNullOrEmpty(My.Settings.AddonDatabaseLocation))
            //{
            //    DatabaseType = 1;
            //    MySQLConnectionString = My.Settings.MySQLConnectionString;
            //    _logger.Debug(MySQLConnectionString);
            //    PseudoTvSettingsLocation = My.Settings.PseudoTvSettingsLocation;
            //    _logger.Debug(PseudoTvSettingsLocation);
            //    AddonDatabaseLocation = My.Settings.AddonDatabaseLocation;
            //    _logger.Debug(AddonDatabaseLocation);
            //    this.Version = My.Settings.Version;
            //    _logger.Debug(this.Version);
            //    RefreshALL();
            //    RefreshTVGuide();
            //}
            //else
            //{
            //    //System.IO.File.Create(SettingsFile)
            //    MessageBox.Show(
            //        "Unable to locate the location of XBMC video library and PseudoTV's setting location.  Please enter them and save the changes.");
            //    Form6 mySettings = new Form6();
            //    //mySettings.Version = Me.Version
            //    mySettings.Show();
            //}

            //Form2.Version = this.Version;
            //Form3.Version = this.Version;
            //Form7.Version = this.Version;
            //Form8.Version = this.Version;
        }

        private void ListTVBanners_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            int x = ListTVBanners.SelectedIndex;
            if (x < 0)
            {
                return;
            }
            if (ListTVBanners.Items.Count <= 0)
            {
                TVBannerPictureBox.ImageLocation = Application.StartupPath + "\\Images\\banner.png";
            }
            else
            {
                TVBannerPictureBox.ImageLocation = ListTVBanners.Items[x].ToString();
                TVBannerPictureBox.Refresh();
            }
        }

        private void TVBannerSelect_Click(System.Object sender, System.EventArgs e)
        {
            int x = ListTVBanners.SelectedIndex;
            string Type = "tvshow";
            string MediaType = "banner";

            if (txtShowLocation.TextLength >= 6)
            {
                if (txtShowLocation.Text.Substring(0, 6) == "smb://")
                {
                    txtShowLocation.Text = txtShowLocation.Text.Replace("/", "\\");
                    txtShowLocation.Text = "\\\\" + txtShowLocation.Text.Substring(6);
                }
            }

            // Displays a SaveFileDialog so the user can save the Image
            // assigned to TVBannerSelect.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = txtShowLocation.Text;
            saveFileDialog1.Filter = "JPeg Image|*.jpg";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.FileName = "banner.jpg";
            saveFileDialog1.ShowDialog();

            string FileToSaveAs = System.IO.Path.Combine(txtShowLocation.Text, saveFileDialog1.FileName);
            TVBannerPictureBox.Image.Save(FileToSaveAs, System.Drawing.Imaging.ImageFormat.Jpeg);

            PseudoTvUtils.DbExecute("UPDATE art SET url = '" + ListTVBanners.Items[x].ToString() +
                                    "' WHERE media_id = '" +
                                    TVShowLabel.Text + "' and type = '" + Type + "' and type = '" + MediaType + "'");
            //TODO: VisualStyleElement.Status.Text = "Updated " + TxtShowName.Text + " Successfully with " +
            //                                 ListTVBanners.Items[x].ToString() + "";
        }

        private void ListTVPosters_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            int x = ListTVPosters.SelectedIndex;
            if (x < 0)
            {
                return;
            }
            if (ListTVPosters.Items.Count == 0)
            {
                TVPosterPictureBox.ImageLocation = Application.StartupPath + "\\Images\\poster.png";
            }
            else
            {
                TVPosterPictureBox.ImageLocation = ListTVPosters.Items[x].ToString();
                TVPosterPictureBox.Refresh();
            }

        }

        private void TVPosterSelect_Click(System.Object sender, System.EventArgs e)
        {
            int x = ListTVPosters.SelectedIndex;
            string MediaType = "poster";


            if (txtShowLocation.TextLength >= 6)
            {
                if (txtShowLocation.Text.Substring(0, 6) == "smb://")
                {
                    txtShowLocation.Text = txtShowLocation.Text.Replace("/", "\\");
                    txtShowLocation.Text = "\\\\" + txtShowLocation.Text.Substring(6);
                }
            }

            // Displays a SaveFileDialog so the user can save the Image
            // assigned to TVPosterSelect.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = txtShowLocation.Text;
            saveFileDialog1.Filter = "JPeg Image|*.jpg";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.FileName = "poster.jpg";
            saveFileDialog1.ShowDialog();

            var fileToSaveAs = System.IO.Path.Combine(Path.GetTempPath(), saveFileDialog1.FileName);
            TVPosterPictureBox.Image.Save(fileToSaveAs, System.Drawing.Imaging.ImageFormat.Jpeg);

            PseudoTvUtils.DbExecute("UPDATE art SET url = '" + ListTVPosters.Items[x].ToString() +
                                    "' WHERE media_id = '" +
                                    TVShowLabel.Text + "' and type = '" + MediaType + "'");
            //VisualStyleElement.Status.Text = "Updated " + TxtShowName.Text + " Successfully with " +
            //                                 ListTVPosters.Items[x].ToString() + "";

        }

        private void TVShowList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {

            if (TVShowList.SelectedItems.Count > 0)
            {
                ListViewItem ListItem = default(ListViewItem);
                ListItem = TVShowList.SelectedItems[0];

                string TVShowName = null;
                string TVShowID = null;

                TVShowID = ListItem.SubItems[2].Text;
                TVShowName = ListItem.SubItems[0].Text;

                TVShowLabel.Text = TVShowID;

                var tvShowArray = new int[5];
                tvShowArray[0] = 1;
                tvShowArray[1] = 9;
                tvShowArray[2] = 15;
                tvShowArray[3] = 17;
                tvShowArray[4] = 7;

                //Shoot it over to the ReadRecord sub, 
                var tvShowReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                    "SELECT * FROM tvshow WHERE idShow='" + TVShowID + "'", tvShowArray);

                string[] tvShowReturnArraySplit = null;

                //We only have 1 response, since it searches by ID. So, just break it into parts. 
                tvShowReturnArraySplit = tvShowReturnArray[0].Split('~');

                var TVPathArray = new[] { 0, 1 };


                //Shoot it over to the ReadRecord sub, 
                string[] TVidPathArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                    "SELECT * FROM tvshowlinkpath WHERE idShow='" + TVShowID + "'", TVPathArray);

                string[] TVidPathArraySplit = null;

                TVidPathArraySplit = TVidPathArray[0].Split('~');

                var TVShowLocationArray = new[] { 0, 1 };

                string[] TVShowLocationArrayReturn = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                    "SELECT * FROM path WHERE idPath='" + TVidPathArraySplit[1] + "'", TVShowLocationArray);

                string[] TVShowLocationSplit = null;

                TVShowLocationSplit = TVShowLocationArrayReturn[0].Split('~');

                TxtShowName.Text = tvShowReturnArraySplit[0];

                string TVGenres = tvShowReturnArraySplit[1];
                if (string.IsNullOrEmpty(tvShowReturnArraySplit[2]))
                {
                    txtShowNetwork.SelectedIndex = -1;
                }
                else
                {
                    txtShowNetwork.Text = tvShowReturnArraySplit[2];
                }

                txtShowLocation.Text = TVShowLocationSplit[1];

                string TVPoster = tvShowReturnArraySplit[4];
                ListTVPosters.Items.Clear();

                string TVBanner = tvShowReturnArraySplit[4];
                ListTVBanners.Items.Clear();

                if (TVPoster.Contains("<thumb aspect=\"poster\">"))
                {
                    string[] TVPosterSplit = TVPoster.Split(Convert.ToChar("<thumb aspect=\"poster\">"));

                    for (var x = 1; x <= TVPosterSplit.Length; x++)
                    {
                        int i = TVPosterSplit[x].IndexOf("<thumb aspect=\"poster\">");
                        TVPosterSplit[x] = TVPosterSplit[x].Substring(i + 1, TVPosterSplit[x].IndexOf("</thumb>"));
                        ListTVPosters.Items.Add(TVPosterSplit[x]);
                    }
                }
                else
                {
                    ListTVPosters.Items.Add("Nothing Found");
                }

                if (TVBanner.Contains("<thumb aspect=\"banner\">"))
                {
                    string[] TVBannerSplit = TVBanner.Split(Convert.ToChar("<thumb aspect=\"banner\">"));

                    for (var x = 1; x <= TVBannerSplit.Length; x++)
                    {
                        int i = TVBannerSplit[x].IndexOf("<thumb aspect=\"banner\">");
                        TVBannerSplit[x] = TVBannerSplit[x].Substring(i + 1, TVBannerSplit[x].IndexOf("</thumb>"));
                        ListTVBanners.Items.Add(TVBannerSplit[x]);
                    }
                }
                else
                {
                    ListTVBanners.Items.Add("Nothing Found");
                }

                //Loop through each TV Genre, if there more than one.
                ListTVGenres.Items.Clear();
                if (TVGenres.Contains(" / "))
                {
                    string[] TVGenresSplit = TVGenres.Split(Convert.ToChar(" / "));

                    for (var x = 0; x <= TVGenresSplit.Length; x++)
                    {
                        ListTVGenres.Items.Add(TVGenresSplit[x]);
                    }
                }
                else if (!string.IsNullOrEmpty(TVGenres))
                {
                    ListTVGenres.Items.Add(TVGenres);
                }

                if (txtShowLocation.TextLength >= 6)
                {
                    if (txtShowLocation.Text.Substring(0, 6) == "smb://")
                    {
                        txtShowLocation.Text = txtShowLocation.Text.Replace("/", "\\");
                        txtShowLocation.Text = "\\\\" + txtShowLocation.Text.Substring(6);
                    }
                }

                if (System.IO.File.Exists(txtShowLocation.Text + "poster.jpg"))
                {
                    TVPosterPictureBox.ImageLocation = txtShowLocation.Text + "poster.jpg";
                }
                else
                {
                    TVPosterPictureBox.ImageLocation =
                        "https://github.com/Lunatixz/script.pseudotv.live/raw/development/resources/images/poster.png";
                }

                if (System.IO.File.Exists(txtShowLocation.Text + "banner.jpg"))
                {
                    TVBannerPictureBox.ImageLocation = txtShowLocation.Text + "banner.jpg";
                }
                else
                {
                    TVBannerPictureBox.ImageLocation =
                        "https://github.com/Lunatixz/script.pseudotv.live/raw/development/resources/images/banner.png";
                }
            }

        }

        private void TVShowList_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Get the new sorting column. 
            ColumnHeader new_sorting_column = TVShowList.Columns[e.Column];
            // Figure out the new sorting order. 
            System.Windows.Forms.SortOrder sort_order = default(System.Windows.Forms.SortOrder);
            if (m_SortingColumn == null)
            {
                // New column. Sort ascending. 
                sort_order = SortOrder.Ascending;
                // See if this is the same column. 
            }
            else
            {
                if (new_sorting_column.Equals(m_SortingColumn))
                {
                    // Same column. Switch the sort order. 
                    if (m_SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending. 
                    sort_order = SortOrder.Ascending;
                }
                // Remove the old sort indicator. 
                m_SortingColumn.Text = m_SortingColumn.Text.Substring(2);
            }
            // Display the new sort order. 
            m_SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                m_SortingColumn.Text = "> " + m_SortingColumn.Text;
            }
            else
            {
                m_SortingColumn.Text = "< " + m_SortingColumn.Text;
            }
            // Create a comparer. 
            TVShowList.ListViewItemSorter = new ClsListviewSorter(e.Column, sort_order);
            // Sort. 
            TVShowList.Sort();
        }

        public object ConvertGenres(ListBox Genrelist)
        {
            //Converts the existing ListTVGenre's contents to the proper format.

            string TVGenresString = "";
            for (var x = 0; x <= Genrelist.Items.Count - 1; x++)
            {
                if (x == 0)
                {
                    TVGenresString = Genrelist.Items[x].ToString();
                }
                else
                {
                    TVGenresString = TVGenresString + " / " + Genrelist.Items[x].ToString();
                }
            }

            return TVGenresString;
        }

        public void RefreshAllGenres()
        {
            var SavedText = Option2.Text;
            Option2.Items.Clear();
            //Set an array with the columns you want returned
            var selectArray = new[] { 1 };

            //Shoot it over to the ReadRecord sub, 
            string[] returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation, "SELECT * FROM genre", selectArray);

            //Now, read the output of the array.

            //Loop through each of the Array items.
            for (var x = 0; x <= returnArray.Length - 1; x++)
            {
                Option2.Items.Add(returnArray[x]);
            }
            Option2.Sorted = true;
            Option2.Text = SavedText;
        }

        public void RefreshAllStudios()
        {
            var SavedText = Option2.Text;

            //Clear all
            Option2.Items.Clear();
            //Form3.ListBox1.Items.Clear()
            txtShowNetwork.Items.Clear();
            txtMovieNetwork.Items.Clear();
            //TODO: Form8.ListBox1.Items.Clear();

            //Set an array with the columns you want returned
            var selectArray = new[] { 1 };

            //Shoot it over to the ReadRecord sub, 
            string[] returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation, "SELECT * FROM studio", selectArray);

            //Now, read the output of the array.

            //Loop through each of the Array items.
            for (var x = 0; x <= returnArray.Length - 1; x++)
            {
                Option2.Items.Add(returnArray[x]);
                //Form3.ListBox1.Items.Add(ReturnArray[x])
                txtShowNetwork.Items.Add(returnArray[x]);
                txtMovieNetwork.Items.Add(returnArray[x]);
                //TODO: Form8.ListBox1.Items.Add(returnArray[x]);
            }

            //Sort them all.
            Option2.Sorted = true;
            //Form3.ListBox1.Sorted = True
            //TODO: Form8.ListBox1.Sorted = true;
            txtShowNetwork.Sorted = true;
            txtMovieNetwork.Sorted = true;
            Option2.Text = SavedText;

        }

        private void NetworkList_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Get the new sorting column. 
            ColumnHeader new_sorting_column = NetworkList.Columns[e.Column];
            // Figure out the new sorting order. 
            System.Windows.Forms.SortOrder sort_order = default(System.Windows.Forms.SortOrder);
            if (m_SortingColumn == null)
            {
                // New column. Sort ascending. 
                sort_order = SortOrder.Ascending;
                // See if this is the same column. 
            }
            else
            {
                if (new_sorting_column.Equals(m_SortingColumn))
                {
                    // Same column. Switch the sort order. 
                    if (m_SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending. 
                    sort_order = SortOrder.Ascending;
                }
                // Remove the old sort indicator. 
                m_SortingColumn.Text = m_SortingColumn.Text.Substring(2);
            }
            // Display the new sort order. 
            m_SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                m_SortingColumn.Text = "> " + m_SortingColumn.Text;
            }
            else
            {
                m_SortingColumn.Text = "< " + m_SortingColumn.Text;
            }
            // Create a comparer. 
            NetworkList.ListViewItemSorter = new ClsListviewSorter(e.Column, sort_order);
            // Sort. 
            NetworkList.Sort();
        }

        private void NetworkList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            NetworkListSubList.Items.Clear();


            if (NetworkList.SelectedIndices.Count > 0)
            {
                var selectArray = new[] { 1 };

                var returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                    "SELECT * FROM tvshow WHERE c14='" +
                    NetworkList.Items[NetworkList.SelectedIndices[0]].SubItems[0].Text + "'", selectArray);

                for (var x = 0; x <= returnArray.Length - 1; x++)
                {
                    NetworkListSubList.Items.Add(returnArray[x]);
                }

            }
        }

        private void Button3_Click(System.Object sender, System.EventArgs e)
        {
            if (TVShowList.SelectedIndices.Count > 0)
            {
                //TODO:
                //Form2.Visible = true;
                //Form2.Focus();
            }
        }

        private void Button4_Click(System.Object sender, System.EventArgs e)
        {

            if (ListTVGenres.SelectedIndex >= 0)
            {
                //Grab the 3rd column from the TVShowList, which is the TVShowID
                var GenreID = LookUpGenre(ListTVGenres.Items[ListTVGenres.SelectedIndex].ToString());

                //Now, remove the link in the database.
                //PseudoTvUtils.DbExecute("DELETE FROM genrelinktvshow WHERE idGenre = '" & GenreID & "' AND idShow ='" & TVShowList.Items(TVShowList.SelectedIndices[0]).SubItems[2].Text & "'")


                ListTVGenres.Items.RemoveAt(ListTVGenres.SelectedIndex);
                // SaveTVShow_Click(Nothing, Nothing)
                RefreshGenres();
            }
        }

        private void Button1_Click(System.Object sender, System.EventArgs e)
        {
            RefreshALL();
        }

        public void RefreshALL()
        {
            if (!string.IsNullOrEmpty(VideoDatabaseLocation) |
                !string.IsNullOrEmpty(MySQLConnectionString) & !string.IsNullOrEmpty(PseudoTvSettingsLocation))
            {
                RefreshMovieList();
                RefreshTVShows();
                RefreshPlugins();
                RefreshAllStudios();
                RefreshNetworkList();
                RefreshNetworkListMovies();
                RefreshGenres();
                TxtShowName.Text = "";
                txtShowLocation.Text = "";
                TVPosterPictureBox.ImageLocation = "";
                MovieLocation.Text = "";
                MoviePicture.ImageLocation = "";
            }
        }

        private void GenresList_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Get the new sorting column. 
            ColumnHeader new_sorting_column = GenresList.Columns[e.Column];
            // Figure out the new sorting order. 
            System.Windows.Forms.SortOrder sort_order = default(System.Windows.Forms.SortOrder);
            if (m_SortingColumn2 == null)
            {
                // New column. Sort ascending. 
                sort_order = SortOrder.Ascending;
                // See if this is the same column. 
            }
            else
            {
                if (new_sorting_column.Equals(m_SortingColumn2))
                {
                    // Same column. Switch the sort order. 
                    if (m_SortingColumn2.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending. 
                    sort_order = SortOrder.Ascending;
                }
                // Remove the old sort indicator. 
                m_SortingColumn2.Text = m_SortingColumn2.Text.Substring(2);
            }
            // Display the new sort order. 
            m_SortingColumn2 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                m_SortingColumn2.Text = "> " + m_SortingColumn2.Text;
            }
            else
            {
                m_SortingColumn2.Text = "< " + m_SortingColumn2.Text;
            }
            // Create a comparer. 
            GenresList.ListViewItemSorter = new ClsListviewSorter(e.Column, sort_order);
            // Sort. 
            GenresList.Sort();
        }

        private void GenresList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            GenresListSubList.Items.Clear();
            GenresListSubListMovies.Items.Clear();

            if (GenresList.SelectedIndices.Count > 0)
            {
                var SelectArray = new[] { 1 };

                //Now, gather a list of all the show IDs that match the genreID
                string[] ReturnArray = null;
                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genre_link WHERE genre_id='" +
                        GenresList.Items[GenresList.SelectedIndices[0]].SubItems[4].Text + "' AND media_type = 'tvshow'",
                        SelectArray);
                }
                else
                {
                    ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genrelinktvshow WHERE idGenre='" +
                        GenresList.Items[GenresList.SelectedIndices[0]].SubItems[4].Text + "'", SelectArray);
                }

                //Now loop through each one individually.

                if (ReturnArray == null)
                {
                }
                else
                {
                    for (var x = 0; x <= ReturnArray.Length - 1; x++)
                    {
                        string[] ShowNameArray = new string[1];
                        SelectArray[0] = 1;

                        string[] ReturnArray2 = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM tvshow WHERE idShow='" + ReturnArray[x] + "'", SelectArray);

                        //Now add that name to the list.
                        GenresListSubList.Items.Add(ReturnArray2[0]);
                    }
                }

                //MOVIES REPEAT THIS PROCESS.
                string[] ReturnArrayMovies = null;
                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    ReturnArrayMovies = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genre_link WHERE genre_id='" +
                        GenresList.Items[GenresList.SelectedIndices[0]].SubItems[4].Text + "' AND media_type = 'movie'",
                        SelectArray);
                }
                else
                {
                    ReturnArrayMovies = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genrelinkmovie WHERE idGenre='" +
                        GenresList.Items[GenresList.SelectedIndices[0]].SubItems[4].Text + "'", SelectArray);
                }

                //Now loop through each one individually 
                if (ReturnArrayMovies == null)
                {
                }
                else
                {
                    for (var x = 0; x <= ReturnArrayMovies.Length - 1; x++)
                    {
                        string[] ShowNameArray = new string[1];
                        SelectArray[0] = 1;

                        string[] ReturnArray2 = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM movie WHERE idMovie='" + ReturnArrayMovies[x] + "'", SelectArray);

                        //Now add that name to the list.
                        GenresListSubListMovies.Items.Add(ReturnArray2[0]);
                    }
                }
            }

        }


        private void SaveTVShow_Click(System.Object sender, System.EventArgs e)
        {

            if (TVShowList.SelectedItems.Count > 0)
            {
                // Fix any issues with shows and 's.
                string TvShowName = TxtShowName.Text;
                //Convert show genres into the format ex:  genre1 / genre2 / etc.
                var ShowGenres = ConvertGenres(ListTVGenres);
                TvShowName = TvShowName.Replace("'", "''");
                //Grab the Network ID based on the name
                var NetworkID = LookUpNetwork(txtShowNetwork.Text);

                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    PseudoTvUtils.DbExecute("DELETE FROM studio_link WHERE media_id = '" + TVShowLabel.Text + "'");
                    PseudoTvUtils.DbExecute("INSERT INTO studio_link(studio_id, media_id, media_type) VALUES ('" +
                                            NetworkID + "', '" +
                                            TVShowLabel.Text + "', 'tvshow')");
                }
                else
                {
                    PseudoTvUtils.DbExecute("DELETE FROM studiolinktvshow WHERE idShow = '" + TVShowLabel.Text + "'");
                    PseudoTvUtils.DbExecute("INSERT INTO studiolinktvshow (idStudio, idShow) VALUES ('" + NetworkID +
                                            "', '" +
                                            TVShowLabel.Text + "')");
                }

                PseudoTvUtils.DbExecute("UPDATE tvshow SET c00 = '" + TvShowName + "', c08 = '" + ShowGenres +
                                        "', c14 ='" +
                                        txtShowNetwork.Text + "' WHERE idShow = '" + TVShowLabel.Text + "'");
                //TODO: VisualStyleElement.Status.Text = "Updated " + TxtShowName.Text + " Successfully";

                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    //Remove all genres from tv show
                    PseudoTvUtils.DbExecute("DELETE FROM genre_link WHERE media_id = '" + TVShowLabel.Text + "'");
                    //add each one.  one by one.
                    for (var x = 0; x <= ListTVGenres.Items.Count - 1; x++)
                    {
                        var GenreID = LookUpGenre(ListTVGenres.Items[x].ToString());
                        PseudoTvUtils.DbExecute("INSERT INTO genre_link (genre_id, media_id, media_type) VALUES ('" +
                                                GenreID + "', '" +
                                                TVShowLabel.Text + "', 'tvshow')");
                    }
                }
                else
                {
                    //Remove all genres from tv show
                    PseudoTvUtils.DbExecute("DELETE FROM genrelinktvshow  WHERE idShow = '" + TVShowLabel.Text + "'");

                    //add each one.  one by one.
                    for (var x = 0; x <= ListTVGenres.Items.Count - 1; x++)
                    {
                        var GenreID = LookUpGenre(ListTVGenres.Items[x].ToString());
                        PseudoTvUtils.DbExecute("INSERT INTO genrelinktvshow (idGenre, idShow) VALUES ('" + GenreID +
                                                "', '" +
                                                TVShowLabel.Text + "')");
                    }
                }

                //Now update the tv show table

                var SavedName = txtShowNetwork.Text;

                //Refresh Things
                RefreshNetworkList();
                RefreshGenres();

                //Reset the text
                //txtShowNetwork.Text = SavedName

                var returnindex = TVShowList.SelectedIndices[0];
                RefreshALL();
                TVShowList.Items[returnindex].Selected = true;
            }
        }

        public void RefreshTVGuideSublist(int ListType, string ListValue)
        {
            TVGuideSubMenu.Items.Clear();

            var TVChannelTypeValue = ListValue;

            if (ListType == (int)TVGuideListEnum.Playlist)
            {
                //Playlist

                //Add Info for PlayList editing/loading.

            }
            else if (ListType == (int)TVGuideListEnum.TvNetwork)
            {
                //This is a TV Network.

                //Make sure there's a value in this box.
                if (!string.IsNullOrEmpty(TVChannelTypeValue))
                {
                    var channelPreview = new[] { 1 };

                    string[] returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM tvshow WHERE c14='" + TVChannelTypeValue + "'", channelPreview);

                    //Make sure the Array is not null.
                    if (returnArray == null)
                    {
                    }
                    else
                    {
                        for (var x = 0; x <= returnArray.Length - 1; x++)
                        {
                            //Add each item it returns to the list.
                            TVGuideSubMenu.Items.Add(returnArray[x]);
                        }
                    }
                }

            }
            else if (ListType == (int)TVGuideListEnum.MovieStudio)
            {
                //Movie Studio

                //Make sure there's a value in this box.
                if (!string.IsNullOrEmpty(TVChannelTypeValue))
                {
                    var SelectArray = new[] { 2 };


                    //Now, gather a list of all the show IDs that match the genreID
                    string[] returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM movie WHERE c18='" + TVChannelTypeValue + "'", SelectArray);

                    //Now loop through each one individually.
                    if (returnArray == null)
                    {
                    }
                    else
                    {
                        for (var x = 0; x <= returnArray.Length - 1; x++)
                        {
                            //Now add that name to the list.
                            TVGuideSubMenu.Items.Add(returnArray[x]);
                        }
                    }
                }

            }
            else if (ListType == (int)TVGuideListEnum.TvGenre)
            {
                //TV Genre

                //Make sure there's a value in this box.
                if (!string.IsNullOrEmpty(TVChannelTypeValue))
                {
                    var SelectArray = new[] { 1 };


                    //Now, gather a list of all the show IDs that match the genreID
                    string[] returnArray = null;
                    if ((KodiVersion >= KodiVersion.Isengard))
                    {
                        returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM genre_link WHERE genre_id='" + LookUpGenre(TVChannelTypeValue) +
                            "' AND media_type = 'tvshow'", SelectArray);
                    }
                    else
                    {
                        returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM genrelinktvshow WHERE idGenre='" + LookUpGenre(TVChannelTypeValue) + "'",
                            SelectArray);
                    }


                    //Now loop through each one individually.
                    for (var x = 0; x <= returnArray.Length - 1; x++)
                    {
                        var ShowNameArray = new[] { 1 };

                        string[] ReturnArray2 = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM tvshow WHERE idShow='" + returnArray[x] + "'", ShowNameArray);

                        //Now add that name to the list.
                        TVGuideSubMenu.Items.Add(ReturnArray2[0]);
                    }
                }
            }
            else if (ListType == (int)TVGuideListEnum.MovieGenre)
            {
                //Movie Genre

                var selectArrayMovies = new[] { 1 };

                string[] ReturnArrayMovies = null;
                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    ReturnArrayMovies = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genre_link WHERE genre_id='" + LookUpGenre(TVChannelTypeValue) +
                        "' AND media_type = 'movie'", selectArrayMovies);
                }
                else
                {
                    ReturnArrayMovies = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                        "SELECT * FROM genrelinkmovie WHERE idGenre='" + LookUpGenre(TVChannelTypeValue) + "'",
                        selectArrayMovies);
                }

                //Now loop through each one individually.
                if (ReturnArrayMovies == null)
                {
                }
                else
                {

                    for (var x = 0; x <= ReturnArrayMovies.Length; x++)
                    {
                        var ShowArray = new[] { 2 };

                        string[] ReturnArray2 = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM movie WHERE idMovie='" + ReturnArrayMovies[x] + "'", ShowArray);

                        //Now add that name to the list.
                        TVGuideSubMenu.Items.Add(ReturnArray2[0]);
                    }
                }
            }
            else if (ListType == (int)TVGuideListEnum.MixedGenre)
            {
                //Mixed Genre

                //Make sure there's a value in this box.
                if (!string.IsNullOrEmpty(TVChannelTypeValue))
                {
                    var SelectArray = new[] { 1 };

                    //Now, gather a list of all the show IDs that match the genreID
                    string[] ReturnArray = null;
                    if ((KodiVersion >= KodiVersion.Isengard))
                    {
                        ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM genre_link WHERE genre_id='" + LookUpGenre(TVChannelTypeValue) +
                            "' AND media_type = 'tvshow'", SelectArray);
                    }
                    else
                    {
                        ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM genrelinktvshow WHERE idGenre='" + LookUpGenre(TVChannelTypeValue) + "'",
                            SelectArray);
                    }

                    //Now loop through each one individually.
                    if (ReturnArray == null)
                    {
                    }
                    else
                    {
                        for (var x = 0; x <= ReturnArray.Length; x++)
                        {
                            var showArray = new[] { 1 };

                            var returnArray2 = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                                "SELECT * FROM tvshow WHERE idShow='" + ReturnArray[x] + "'", showArray);

                            //Now add that name to the list.
                            TVGuideSubMenu.Items.Add(returnArray2[0]);
                        }
                    }
                    //------------------------------------
                    //Repeat this step for the Movies now.

                    var SelectArrayMovies = new[] { 1 };

                    string[] ReturnArrayMovies = null;
                    if ((KodiVersion >= KodiVersion.Isengard))
                    {
                        ReturnArrayMovies = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM genre_link WHERE genre_id='" + LookUpGenre(TVChannelTypeValue) +
                            "' AND media_type = 'movie'", SelectArrayMovies);
                    }
                    else
                    {
                        ReturnArrayMovies = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                            "SELECT * FROM genrelinkmovie WHERE idGenre='" + LookUpGenre(TVChannelTypeValue) + "'",
                            SelectArrayMovies);
                    }


                    //Now loop through each one individually.
                    //Verify it's not NULL.
                    if (ReturnArrayMovies == null)
                    {
                    }
                    else
                    {
                        for (var x = 0; x <= ReturnArrayMovies.Length; x++)
                        {
                            var ShowArray = new[] { 2 };

                            string[] ReturnArray2 = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                                "SELECT * FROM movie WHERE idMovie='" + ReturnArrayMovies[x] + "'", ShowArray);

                            //Now add that name to the list.
                            TVGuideSubMenu.Items.Add(ReturnArray2[0]);
                        }
                    }


                }

            }
            else if (ListType == (int)TVGuideListEnum.TvShow)
            {
                //TV Show
            }
            else if (ListType == (int)TVGuideListEnum.Directory)
            {
                //Directory

            }

            //Now loop through all the shows listed to NOT show, compare them to the list of shows and make any of them have a red background if they match.
            for (var x = 0; x <= NotShows.Items.Count - 1; x++)
            {
                string NotShow = NotShows.Items[x].ToString();

                for (var y = 0; y <= TVGuideSubMenu.Items.Count - 1; y++)
                {
                    if (!NotShow.Equals(TVGuideSubMenu.Items[y].SubItems[0].Text))
                    {
                        TVGuideSubMenu.Items[y].BackColor = Color.Red;
                    }
                }

            }
            TVGuideSubMenu.Sort();
        }

        private void TVGuideList_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Get the new sorting column. 
            ColumnHeader new_sorting_column = TVGuideList.Columns[e.Column];
            // Figure out the new sorting order. 
            System.Windows.Forms.SortOrder sort_order = default(System.Windows.Forms.SortOrder);
            if (m_SortingColumn == null)
            {
                // New column. Sort ascending. 
                sort_order = SortOrder.Ascending;
                // See if this is the same column. 
            }
            else
            {
                if (new_sorting_column.Equals(m_SortingColumn))
                {
                    // Same column. Switch the sort order. 
                    if (m_SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending. 
                    sort_order = SortOrder.Ascending;
                }
                // Remove the old sort indicator. 
                m_SortingColumn.Text = m_SortingColumn.Text.Substring(2);
            }
            // Display the new sort order. 
            m_SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                m_SortingColumn.Text = "> " + m_SortingColumn.Text;
            }
            else
            {
                m_SortingColumn.Text = "< " + m_SortingColumn.Text;
            }
            // Create a comparer. 
            TVGuideList.ListViewItemSorter = new ClsListviewSorter(e.Column, sort_order);
            // Sort. 
            TVGuideList.Sort();
        }

        private void TVGuideList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {

            if (TVGuideList.SelectedIndices.Count > 0)
            {


                //Reset the checked options.
                ChkLogo.Checked = false;
                chkDontPlayChannel.Checked = false;
                ChkRandom.Checked = false;
                ChkRealTime.Checked = false;
                ChkResume.Checked = false;
                ChkIceLibrary.Checked = false;
                ChkExcludeBCT.Checked = false;
                ChkPopup.Checked = false;
                ChkUnwatched.Checked = false;
                ChkWatched.Checked = false;
                ChkPause.Checked = false;
                ChkPlayInOrder.Checked = false;
                ResetDays.Clear();
                ChannelName.Clear();

                //Clear other form items.
                TVGuideSubMenu.Items.Clear();
                var PlayListNumber = Convert.ToInt32(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[1].Text);
                Option2.Items.Clear();
                InterleavedList.Items.Clear();
                SchedulingList.Items.Clear();
                NotShows.Items.Clear();

                //Display the Channel Number.
                TVGuideShowName.Text = "Channel " + TVGuideList.SelectedItems[0].SubItems[0].Text;


                if (PlayListNumber != 9999)
                {
                    PlayListType.SelectedIndex = PlayListNumber;

                    int Option1 = 0;

                    var TVChannelTypeValue = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;

                    if (PlayListNumber == 0)
                    {
                        //Playlist
                        Option1 = 1;

                        //Add Info for PlayList editing/loading.

                    }
                    else if (PlayListNumber == 1)
                    {
                        //This is a TV Network.

                        for (var x = 0; x <= NetworkList.Items.Count - 1; x++)
                        {
                            Option2.Items.Add(NetworkList.Items[x].SubItems[0].Text);
                        }

                        //Make sure there's a value in this box.
                        if (!string.IsNullOrEmpty(TVChannelTypeValue))
                        {
                            RefreshTVGuideSublist(PlayListNumber, TVChannelTypeValue);
                        }

                    }
                    else if (PlayListNumber == 2)
                    {
                        //Movie Studio
                        RefreshAllStudios();

                    }
                    else if (PlayListNumber == 3)
                    {
                        //TV Genre
                        for (var x = 0; x <= GenresList.Items.Count - 1; x++)
                        {
                            Option2.Items.Add(GenresList.Items[x].SubItems[0].Text);
                        }

                    }
                    else if (PlayListNumber == 4)
                    {
                        //Movie Genre
                        RefreshAllGenres();

                    }
                    else if (PlayListNumber == 5)
                    {
                        //Mixed Genre
                        RefreshAllGenres();

                    }
                    else if (PlayListNumber == 6)
                    {
                        //TV Show
                        for (var x = 0; x <= TVShowList.Items.Count - 1; x++)
                        {
                            Option2.Items.Add(TVShowList.Items[x].SubItems[0].Text);
                        }
                    }
                    else if (PlayListNumber == 7)
                    {
                        //Directory
                        Option1 = 1;
                    }
                    else if (PlayListNumber == 8)
                    {
                        //LiveTV
                        Option1 = 2;
                    }
                    else if (PlayListNumber == 9)
                    {
                        //InternetTV
                        Option1 = 3;
                    }
                    else if (PlayListNumber == 10 | PlayListNumber == 11)
                    {
                        //YoutubeTV or RSS
                        Option1 = 4;
                    }
                    else if (PlayListNumber == 13)
                    {
                        //Music Videos
                        Option1 = 5;
                    }
                    else if (PlayListNumber == 14)
                    {
                        //Extras
                        Option1 = 6;
                    }
                    else if (PlayListNumber == 15)
                    {
                        //Direct Plugin Type
                        Option1 = 7;
                    }

                    //Now, we loop through the advanced rules to populate those properly.

                    //break this array into all the rules for this channel.
                    var AllRules = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[4].Text.Split('~');

                    //Loop through all of them.
                    //But, only the ones it "says" it has.

                    int RuleCount = 0;


                    if (!string.IsNullOrEmpty(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[5].Text))
                    {
                        RuleCount = Convert.ToInt32(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[5].Text);
                    }


                    for (var y = 1; y <= RuleCount; y++)
                    {
                        //For y = 1 To UBound(AllRules)
                        var RuleSettings = AllRules[y].Split('|');

                        //Rule #1, #2, etc.
                        var RuleNum = RuleSettings[0];
                        //Value, most of the time this is the only thing we need.
                        var RuleValue = RuleSettings[1];


                        if (RuleValue == "5")
                        {
                            chkDontPlayChannel.Checked = true;
                        }
                        else if (RuleValue == "10")
                        {
                            ChkRandom.Checked = true;
                        }
                        else if (RuleValue == "7")
                        {
                            ChkRealTime.Checked = true;
                        }
                        else if (RuleValue == "9")
                        {
                            ChkResume.Checked = true;
                        }
                        else if (RuleValue == "11")
                        {
                            ChkUnwatched.Checked = true;
                        }
                        else if (RuleValue == "4")
                        {
                            ChkWatched.Checked = true;
                        }
                        else if (RuleValue == "8")
                        {
                            ChkPause.Checked = true;
                        }
                        else if (RuleValue == "12")
                        {
                            ChkPlayInOrder.Checked = true;

                        }
                        else
                        {
                            //Okay, so it's not something requiring a single option.

                            //Now loop through all the sub-options of each rule.
                            string[] SubOptions = new string[5];

                            for (var z = 2; z <= RuleSettings.Length; z++)
                            {
                                var OptionNum = Convert.ToInt32(RuleSettings[z].Split('^')[0]);
                                var OptionValue = RuleSettings[z].Split('^')[1];


                                if (RuleValue == "13")
                                {
                                    //TODO: Optimize
                                    ResetDays.Text = (Convert.ToInt32(OptionValue) / 60).ToString();
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                                else if (RuleValue == "1")
                                {
                                    ChannelName.Text = OptionValue;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                                else if (RuleValue == "15" & OptionValue == "Yes")
                                {
                                    ChkLogo.Checked = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                                else if (RuleValue == "14" & OptionValue == "Yes")
                                {
                                    ChkIceLibrary.Checked = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                                else if (RuleValue == "17" & OptionValue == "Yes")
                                {
                                    ChkExcludeBCT.Checked = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                                else if (RuleValue == "18" & OptionValue == "Yes")
                                {
                                    ChkPopup.Checked = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                                else if (RuleValue == "2")
                                {
                                    NotShows.Items.Add(OptionValue);
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                                else if (RuleValue == "6")
                                {
                                    //Add this option to a sub-item array to add later to the
                                    //Object at the end
                                    SubOptions[OptionNum - 1] = OptionValue;

                                    if (OptionNum == 5)
                                    {
                                        //last option.
                                        //create + insert object
                                        ListViewItem itm = default(ListViewItem);
                                        itm = new ListViewItem(SubOptions);
                                        //Add to list
                                        InterleavedList.Items.Add(itm);
                                        //Remove it from the loop.  We only need 4 options here.
                                        break; // TODO: might not be correct. Was : Exit For

                                    }
                                    else
                                    {
                                    }
                                }
                                else if (RuleValue == "3")
                                {
                                    //Add this option to a sub-item array to add later to the
                                    //Object at the end
                                    SubOptions[OptionNum - 1] = OptionValue;
                                    if (OptionNum == 4)
                                    {
                                        //last option.
                                        //create + insert object
                                        var itm = new ListViewItem(SubOptions);
                                        //Add to list

                                        SchedulingList.Items.Add(itm);
                                        break; // TODO: might not be correct. Was : Exit For

                                    }
                                }

                            }
                        }
                    }


                    RefreshTVGuideSublist(PlayListNumber, TVChannelTypeValue);

                    if (Option1 == 1)
                    {
                        PlayListLocation.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;
                        //SortTypeBox.SelectedIndex = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[8].Text
                        int index = 0;
                        index =
                            MediaLimitBox.FindString(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[7].Text);
                        MediaLimitBox.SelectedIndex = index;
                    }
                    else if (Option1 == 2)
                    {
                        PlayListLocation.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;
                        StrmUrlBox.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[6].Text;
                        ShowTitleBox.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[7].Text;
                    }
                    else if (Option1 == 3)
                    {
                        PlayListLocation.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;
                        StrmUrlBox.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[6].Text;
                        ShowTitleBox.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[7].Text;
                        ShowDescBox.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[8].Text;
                    }
                    else if (Option1 == 4)
                    {
                        PlayListLocation.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;
                        YouTubeType.SelectedIndex =
                            Convert.ToInt32(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[6].Text) - 1;
                        int index = 0;
                        index =
                            MediaLimitBox.FindString(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[7].Text);
                        MediaLimitBox.SelectedIndex = index;
                        SortTypeBox.SelectedIndex =
                            Convert.ToInt32(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[8].Text);
                        if (YouTubeType.SelectedIndex == 6 | YouTubeType.SelectedIndex == 7)
                        {
                            string ReturnMulti = "";

                            ReturnMulti = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;

                            if (ReturnMulti.Contains("|"))
                            {
                                string[] ReturnMultiSplit = ReturnMulti.Split('|');

                                for (var x = 0; x <= ReturnMultiSplit.Length; x++)
                                {
                                    NotShows.Items.Add(ReturnMultiSplit[x]);
                                }


                            }
                            else
                            {
                                ShowDescBox.Visible = true;
                                ShowDescBox.Text = ReturnMulti;

                            }

                            YouTubeMulti = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;

                        }

                    }
                    else if (Option1 == 5)
                    {
                        PlayListLocation.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[6].Text;
                        SubChannelType.SelectedIndex =
                            Convert.ToInt32(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[6].Text) - 1;
                        int index = 0;
                        index =
                            MediaLimitBox.FindString(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[7].Text);
                        MediaLimitBox.SelectedIndex = index;
                        SortTypeBox.SelectedIndex =
                            Convert.ToInt32(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[8].Text);
                    }
                    else if (Option1 == 7)
                    {
                        PlayListLocation.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;
                        SortTypeBox.SelectedIndex =
                            Convert.ToInt32(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[8].Text);
                        int index = 0;
                        index =
                            MediaLimitBox.FindString(TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[7].Text);
                        MediaLimitBox.SelectedIndex = index;

                        string ReturnPlugin = "";

                        var SelectArray = new[] { 0 };

                        ReturnPlugin = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;

                        string ReturnProperPlugin = ReturnPlugin.Remove(0, 9);

                        string[] ReturnArray = PseudoTvUtils.ReadPluginRecord(AddonDatabaseLocation,
                            "SELECT name FROM addon WHERE addonID = '" + ReturnProperPlugin + "'", SelectArray);

                        int index2 = 0;
                        index2 = PluginType.FindString(ReturnArray[0]);
                        PluginType.SelectedIndex = index2;

                        PluginNotInclude = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[6].Text;

                        if (PluginNotInclude.Contains(","))
                        {
                            string[] PluginNotIncludeSplit = PluginNotInclude.Split(',');

                            for (var x = 0; x <= PluginNotIncludeSplit.Length; x++)
                            {
                                NotShows.Items.Add(PluginNotIncludeSplit[x]);
                            }
                        }
                    }
                    else
                    {
                        Option2.Text = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[2].Text;
                    }
                    TVGuideSubMenu.Sort();
                }
            }
        }

        private void Option2_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (PlayListType.SelectedIndex >= 0 & !string.IsNullOrEmpty(Option2.Text))
            {
                RefreshTVGuideSublist(PlayListType.SelectedIndex, Option2.Text);
            }

        }

        private void PlayListType_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            //Clear the Sub-menu
            TVGuideSubMenu.Items.Clear();
            var _with1 = PlayListLocation;
            _with1.Location = new System.Drawing.Point(267, 120);
            PlayListLocation.Text = "";
            Label6.Visible = false;
            Label6.Text = "";
            Button5.Visible = false;
            PlayListLocation.Visible = false;
            PluginType.Visible = false;
            StrmUrl.Visible = false;
            StrmUrlBox.Visible = false;
            Option2.Visible = false;
            YouTubeType.Visible = false;
            ShowTitle.Visible = false;
            ShowTitleBox.Visible = false;
            ShowDesc.Visible = false;
            ShowDescBox.Visible = false;
            MediaLimit.Visible = false;
            MediaLimitBox.Visible = false;
            SortType.Visible = false;
            SortTypeBox.Visible = false;
            SubChannelType.Visible = false;
            TVGuideSubMenu.Visible = false;
            InterleavedList.Visible = false;
            Label7.Visible = false;
            SchedulingList.Visible = false;
            Label11.Visible = false;
            NotShows.Visible = false;
            Label12.Visible = false;
            AddExcludeBtn.Visible = false;
            DelExcludeBtn.Visible = false;
            PluginNotInclude = "";

            if (PlayListType.SelectedIndex == 0)
            {
                Button5.Visible = true;
                Label6.Text = "Location:";
                Label6.Visible = true;
                PlayListLocation.Visible = true;
            }
            else if (PlayListType.SelectedIndex == 1 | PlayListType.SelectedIndex == 2 | PlayListType.SelectedIndex == 3 |
                     PlayListType.SelectedIndex == 4 | PlayListType.SelectedIndex == 5 | PlayListType.SelectedIndex == 6)
            {
                Option2.Visible = true;
                TVGuideSubMenu.Visible = true;
                InterleavedList.Visible = true;
                Label7.Visible = true;
                SchedulingList.Visible = true;
                Label11.Visible = true;
                NotShows.Visible = true;
                Label12.Visible = true;
                Button8.Visible = true;
                Button9.Visible = true;
                Button10.Visible = true;
                Button11.Visible = true;
                Button12.Visible = true;
            }
            else if (PlayListType.SelectedIndex == 7)
            {
                Label6.Text = "Location:";
                Label6.Visible = true;
                PlayListLocation.Visible = true;
                var _with2 = PlayListLocation;
                _with2.Location = new System.Drawing.Point(270, 120);
                PlayListLocation.Visible = true;
                var _with3 = MediaLimit;
                _with3.Location = new System.Drawing.Point(160, 160);
                MediaLimit.Visible = true;
                var _with4 = MediaLimitBox;
                _with4.Location = new System.Drawing.Point(162, 180);
                MediaLimitBox.SelectedIndex = 0;
                MediaLimitBox.Visible = true;
                var _with5 = SortType;
                _with5.Location = new System.Drawing.Point(225, 160);
                SortType.Visible = true;
                var _with6 = SortTypeBox;
                _with6.Location = new System.Drawing.Point(227, 180);
                SortTypeBox.SelectedIndex = 0;
                SortTypeBox.Visible = true;
                NotShows.Visible = true;
                NotShows.Visible = false;
                AddExcludeBtn.Visible = false;
                DelExcludeBtn.Visible = false;
                Label12.Visible = false;
                Button5.Visible = true;
            }
            else if (PlayListType.SelectedIndex == 8)
            {
                Label6.Text = "Channel id:";
                Label6.Visible = true;
                PlayListLocation.Visible = true;
                StrmUrl.Visible = true;
                StrmUrlBox.Visible = true;
                ShowTitle.Text = "XMLTV Filename:";
                ShowTitle.Visible = true;
                ShowTitleBox.Visible = true;
            }
            else if (PlayListType.SelectedIndex == 9)
            {
                Label6.Text = "Duration:";
                Label6.Visible = true;
                PlayListLocation.Visible = true;
                StrmUrl.Text = "Source path:";
                StrmUrl.Visible = true;
                StrmUrlBox.Visible = true;
            }
            else if (PlayListType.SelectedIndex == 10)
            {
                NotShows.Items.Clear();
                Button5.Visible = false;
                YouTubeType.SelectedIndex = 0;
                YouTubeType.Visible = true;
                Label6.Text = "Channel/User:";
                Label6.Visible = true;
                var _with7 = PlayListLocation;
                _with7.Location = new System.Drawing.Point(270, 120);
                PlayListLocation.Visible = true;
                var _with8 = MediaLimit;
                _with8.Location = new System.Drawing.Point(160, 160);
                MediaLimit.Visible = true;
                var _with9 = MediaLimitBox;
                _with9.Location = new System.Drawing.Point(162, 180);
                MediaLimitBox.SelectedIndex = 0;
                MediaLimitBox.Visible = true;
                var _with10 = SortType;
                _with10.Location = new System.Drawing.Point(225, 160);
                SortType.Visible = true;
                var _with11 = SortTypeBox;
                _with11.Location = new System.Drawing.Point(227, 180);
                SortTypeBox.SelectedIndex = 0;
                SortTypeBox.Visible = true;
                NotShows.Visible = true;
                NotShows.Visible = false;
                AddExcludeBtn.Visible = false;
                DelExcludeBtn.Visible = false;
                Label12.Visible = false;
            }
            else if (PlayListType.SelectedIndex == 11)
            {
                Label6.Text = "Source path:";
                Label6.Visible = true;
                PlayListLocation.Visible = true;
                var _with12 = MediaLimit;
                _with12.Location = new System.Drawing.Point(160, 160);
                MediaLimit.Visible = true;
                var _with13 = MediaLimitBox;
                _with13.Location = new System.Drawing.Point(162, 180);
                MediaLimitBox.SelectedIndex = 0;
                MediaLimitBox.Visible = true;
                var _with14 = SortType;
                _with14.Location = new System.Drawing.Point(225, 160);
                SortType.Visible = true;
                var _with15 = SortTypeBox;
                _with15.Location = new System.Drawing.Point(227, 180);
                SortTypeBox.SelectedIndex = 0;
                SortTypeBox.Visible = true;
                Button8.Visible = false;
                Button9.Visible = false;
                Button10.Visible = false;
                Button11.Visible = false;
                Button12.Visible = false;
            }
            else if (PlayListType.SelectedIndex == 13)
            {
                Label6.Text = "LastFM Username:";
                Label6.Visible = true;
                PlayListLocation.Visible = true;
                var _with16 = MediaLimit;
                _with16.Location = new System.Drawing.Point(160, 160);
                MediaLimit.Visible = true;
                var _with17 = MediaLimitBox;
                _with17.Location = new System.Drawing.Point(162, 180);
                MediaLimitBox.SelectedIndex = 0;
                MediaLimitBox.Visible = true;
                var _with18 = SortType;
                _with18.Location = new System.Drawing.Point(225, 160);
                SortType.Visible = true;
                var _with19 = SortTypeBox;
                _with19.Location = new System.Drawing.Point(227, 180);
                SortTypeBox.SelectedIndex = 0;
                SortTypeBox.Visible = true;
                SubChannelType.Items.Clear();
                var _with20 = SubChannelType.Items;
                _with20.Add("LastFM");
                _with20.Add("MyMusicTV");
                SubChannelType.Visible = true;
                SubChannelType.SelectedIndex = 0;
            }
            else if (PlayListType.SelectedIndex == 14)
            {
                Label6.Visible = false;
                var _with21 = MediaLimit;
                _with21.Location = new System.Drawing.Point(160, 160);
                MediaLimit.Visible = true;
                var _with22 = MediaLimitBox;
                _with22.Location = new System.Drawing.Point(162, 180);
                MediaLimitBox.SelectedIndex = 0;
                MediaLimitBox.Visible = true;
                var _with23 = SortType;
                _with23.Location = new System.Drawing.Point(225, 160);
                SortType.Visible = true;
                var _with24 = SortTypeBox;
                _with24.Location = new System.Drawing.Point(227, 180);
                SortTypeBox.SelectedIndex = 0;
                SortTypeBox.Visible = true;
                SubChannelType.Items.Clear();
                var _with25 = SubChannelType.Items;
                _with25.Add("popcorn");
                _with25.Add("cinema");
                SubChannelType.Visible = true;
                SubChannelType.SelectedIndex = 0;
            }
            else if (PlayListType.SelectedIndex == 15)
            {
                NotShows.Items.Clear();
                Label6.Visible = false;
                var _with26 = PlayListLocation;
                _with26.Location = new System.Drawing.Point(220, 120);
                PlayListLocation.Visible = true;
                var _with27 = PluginType;
                _with27.Location = new System.Drawing.Point(227, 86);
                PluginType.Visible = true;
                var _with28 = MediaLimit;
                _with28.Location = new System.Drawing.Point(160, 160);
                MediaLimit.Visible = true;
                var _with29 = MediaLimitBox;
                _with29.Location = new System.Drawing.Point(162, 180);
                MediaLimitBox.SelectedIndex = 0;
                MediaLimitBox.Visible = true;
                var _with30 = SortType;
                _with30.Location = new System.Drawing.Point(225, 160);
                SortType.Visible = true;
                var _with31 = SortTypeBox;
                _with31.Location = new System.Drawing.Point(227, 180);
                SortTypeBox.SelectedIndex = 0;
                SortTypeBox.Visible = true;
                NotShows.Visible = true;
                Label12.Text = "Do not include these items";
                Label12.Visible = true;
                AddExcludeBtn.Visible = true;
                DelExcludeBtn.Visible = true;
                Button8.Visible = false;
                Button9.Visible = false;
                Button10.Visible = false;
                Button11.Visible = false;
                Button12.Visible = false;

            }
            else
            {
                Option2.Visible = true;
                PlayListLocation.Visible = false;

            }

            Option2.Items.Clear();
            Option2.Text = "";


            if (PlayListType.SelectedIndex == 0)
            {
                for (var x = 0; x <= NetworkList.Items.Count - 1; x++)
                {
                    Option2.Items.Add(NetworkList.Items[x].SubItems[0].Text);
                }
            }
            else if (PlayListType.SelectedIndex == 1)
            {
                for (var x = 0; x <= NetworkList.Items.Count - 1; x++)
                {
                    Option2.Items.Add(NetworkList.Items[x].SubItems[0].Text);
                }
            }
            else if (PlayListType.SelectedIndex == 2)
            {
                RefreshAllStudios();
            }
            else if (PlayListType.SelectedIndex == 3)
            {
                for (var x = 0; x <= GenresList.Items.Count - 1; x++)
                {
                    Option2.Items.Add(GenresList.Items[x].SubItems[0].Text);
                }
            }
            else if (PlayListType.SelectedIndex == 4)
            {
                RefreshAllGenres();
            }
            else if (PlayListType.SelectedIndex == 5)
            {
                RefreshAllGenres();
            }
            else if (PlayListType.SelectedIndex == 6)
            {
                for (var x = 0; x <= TVShowList.Items.Count - 1; x++)
                {
                    Option2.Items.Add(TVShowList.Items[x].SubItems[0].Text);
                }
            }

        }


        private void Button5_Click(System.Object sender, System.EventArgs e)
        {

            if (PlayListType.Text == "Directory")
            {
                FolderBrowserDialog1.ShowDialog();
                PlayListLocation.Text = FolderBrowserDialog1.SelectedPath;
            }
            else if (PlayListType.Text == "Playlist")
            {
                OpenFileDialog1.ShowDialog();

                var Filename = OpenFileDialog1.FileName;
                var FilenameSplit = Filename.Split(Convert.ToChar("\\"));

                PlayListLocation.Text = "special://profile/playlists/video/" +
                                        FilenameSplit[FilenameSplit.Length];
            }
        }

        private void RefreshButton_Click(System.Object sender, System.EventArgs e)
        {
            //Loop through config file.
            //Grab all comments MINUS the ones for selected #
            //Append this & our new content to the file.


            if (TVGuideList.SelectedItems.Count > 0)
            {
                string FILE_NAME = PseudoTvSettingsLocation;
                string TextFile = "";

                var ChannelNum = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[0].Text;


                //Loop through config file.
                //Grab all comments MINUS the ones for selected #

                if (System.IO.File.Exists(FILE_NAME) == true)
                {
                    System.IO.StreamReader objReader = new System.IO.StreamReader(FILE_NAME);


                    while (objReader.Peek() != -1)
                    {
                        var SingleLine = objReader.ReadLine();

                        if (!SingleLine.Contains("<setting id=" + (char)34 + "Channel_" + ChannelNum + "_") &&
                            !SingleLine.Contains("</settings"))
                        {
                            TextFile = TextFile + SingleLine + Environment.NewLine;
                        }

                    }

                    objReader.Close();

                }
                else
                {
                    MessageBox.Show("File Does Not Exist");

                }

                var returnindex = TVGuideList.SelectedIndices[0];
                RefreshTVGuide();
                TVGuideList.Items[returnindex].Selected = true;

            }
        }

        private void Button2_Click(System.Object sender, System.EventArgs e)
        {
            //Loop through config file.
            //Grab all comments MINUS the ones for selected #
            //Append this & our new content to the file.

            SaveExcludeBtn.Visible = false;


            if (TVGuideList.SelectedItems.Count > 0)
            {
                string FILE_NAME = PseudoTvSettingsLocation;
                string TextFile = "";

                var ChannelNum = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[0].Text;


                //Loop through config file.
                //Grab all comments MINUS the ones for selected #

                if (System.IO.File.Exists(FILE_NAME) == true)
                {
                    System.IO.StreamReader objReader = new System.IO.StreamReader(FILE_NAME);


                    while (objReader.Peek() != -1)
                    {
                        var SingleLine = objReader.ReadLine();

                        if (!SingleLine.Contains("<setting id=" + (char)34 + "Channel_" + ChannelNum + "_") &&
                            !SingleLine.Contains("</settings"))
                        {
                            TextFile = TextFile + SingleLine + System.Environment.NewLine;
                        }

                    }

                    objReader.Close();

                }
                else MessageBox.Show("File Does Not Exist");

                //Now, append info for this channel we're editing.

                string AppendInfo = "";
                var rulecount = 0;

                //Show the Logo is checked.
                //<setting id="Channel_1_rule_1_id" value="15" />
                if (ChkLogo.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "15" + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 + " value=" +
                                 (char)34 + "Yes" + (char)34 + " />";
                }

                //Don't show this channel is checked
                //<setting id="Channel_1_rule_1_id" value="5" />
                if (chkDontPlayChannel.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "5" + (char)34 + " />";
                }

                //Play Random Mode
                //<setting id="Channel_1_rule_1_id" value="10" />
                if (ChkRandom.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "10" + (char)34 + " />";
                }

                //Play Real-Time Mode
                //<setting id="Channel_1_rule_1_id" value="7" />
                if (ChkRealTime.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "7" + (char)34 + " />";
                }

                //Play Resume Mode
                //<setting id="Channel_1_rule_1_id" value="9" />
                if (ChkResume.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "9" + (char)34 + " />";
                }

                //Play Only Unwatched Films
                //<setting id="Channel_1_rule_1_id" value="11" />
                if (ChkUnwatched.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "11" + (char)34 + " />";
                }

                //Only play Watched
                //<setting id="Channel_1_rule_1_id" value="4" />
                if (ChkWatched.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "4" + (char)34 + " />";
                }

                //Exclude Strms?
                //<setting id="Channel_1_rule_1_id" value="14" />
                if (ChkIceLibrary.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "14" + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 + " value=" +
                                 (char)34 + "No" + (char)34 + " />";
                }

                //Exclude BCT?
                //<setting id="Channel_1_rule_1_id" value="17" />
                if (ChkExcludeBCT.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "17" + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 + " value=" +
                                 (char)34 + "No" + (char)34 + " />";
                }

                //Disable Popup?
                //<setting id="Channel_1_rule_1_id" value="18" />
                if (ChkPopup.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "18" + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 + " value=" +
                                 (char)34 + "No" + (char)34 + " />";
                }

                //Pause when not watching
                //<setting id="Channel_1_rule_1_id" value="8" />
                if (ChkPause.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "8" + (char)34 + " />";
                }

                //Play shows in order
                //<setting id="Channel_1_rule_1_id" value="12" />
                if (ChkPlayInOrder.Checked == true)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "12" + (char)34 + " />";
                }

                //Theres a # in the reset day amount
                //<setting id="Channel_1_rule_1_id" value="13" />
                //<setting id="Channel_1_rule_1_opt_1" value=ResetDays />

                if (PseudoTvUtils.IsNumeric(ResetDays.Text))
                {
                    _resetHours = (Convert.ToInt32(ResetDays.Text) * 60);
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "13" + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 + " value=" +
                                 (char)34 + _resetHours + (char)34 + " />";
                }

                //Theres a channel name
                //<setting id="Channel_1_rule_1_id" value="1" />
                //<setting id="Channel_1_rule_1_opt_1" value=ChannelName />
                if (!string.IsNullOrEmpty(ChannelName.Text))
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "1" + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 + " value=" +
                                 (char)34 + ChannelName.Text + (char)34 + " />";
                }

                //Loop through shows not to play
                //<setting id="Channel_1_rule_1_id" value="2" />
                //<setting id="Channel_1_rule_1_opt_1" value=ShowName />
                if (PlayListType.SelectedIndex == 15 | PlayListType.SelectedIndex == 10)
                {
                }
                else
                {
                    for (var x = 0; x <= NotShows.Items.Count - 1; x++)
                    {
                        rulecount = rulecount + 1;
                        AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                     "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 +
                                     " value=" + (char)34 + "2" + (char)34 + " />";
                        AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                     "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 +
                                     " value=" + (char)34 + NotShows.Items[x] + (char)34 + " />";
                    }
                }
                //Interleaved loop
                for (var x = 0; x <= InterleavedList.Items.Count - 1; x++)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "6" + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 + " value=" +
                                 (char)34 + InterleavedList.Items[x].SubItems[0].Text + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_2" + (char)34 + " value=" +
                                 (char)34 + InterleavedList.Items[x].SubItems[1].Text + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_3" + (char)34 + " value=" +
                                 (char)34 + InterleavedList.Items[x].SubItems[2].Text + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_4" + (char)34 + " value=" +
                                 (char)34 + InterleavedList.Items[x].SubItems[3].Text + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_5" + (char)34 + " value=" +
                                 (char)34 + InterleavedList.Items[x].SubItems[4].Text + (char)34 + " />";
                }

                for (var x = 0; x <= SchedulingList.Items.Count - 1; x++)
                {
                    rulecount = rulecount + 1;
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_id" + (char)34 + " value=" +
                                 (char)34 + "3" + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_1" + (char)34 + " value=" +
                                 (char)34 + SchedulingList.Items[x].SubItems[0].Text + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_2" + (char)34 + " value=" +
                                 (char)34 + SchedulingList.Items[x].SubItems[1].Text + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_3" + (char)34 + " value=" +
                                 (char)34 + SchedulingList.Items[x].SubItems[2].Text + (char)34 + " />";
                    AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                 "Channel_" + ChannelNum + "_rule_" + rulecount + "_opt_4" + (char)34 + " value=" +
                                 (char)34 + SchedulingList.Items[x].SubItems[3].Text + (char)34 + " />";
                }

                //Update it has been changed to flag it?
                //<setting id="Channel_1_changed" value="True" />
                AppendInfo = AppendInfo + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                             "Channel_" + ChannelNum + "_changed" + (char)34 + " value=" + (char)34 +
                             "True" + (char)34 + " />";

                //Add type of channel to the top.
                var TopAppend = "\t" + "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_type" +
                                   (char)34 + " value=" + (char)34 + PlayListType.SelectedIndex + (char)34 +
                                   " />";

                if (PlayListType.SelectedIndex == 0)
                {
                    TopAppend += Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_1" + (char)34 + " value=" + (char)34 +
                                PlayListLocation.Text + (char)34 + " />";
                }
                else if (PlayListType.SelectedIndex == 7)
                {
                    TopAppend += Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_1" + (char)34 + " value=" + (char)34 +
                                PlayListLocation.Text + (char)34 + " />" + System.Environment.NewLine + "\t" +
                                "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_3" + (char)34 +
                                " value=" + (char)34 + MediaLimitBox.Text + (char)34 + " />" +
                                System.Environment.NewLine + "\t" + "<setting id=" + (char)34 + "Channel_" +
                                ChannelNum + "_4" + (char)34 + " value=" + (char)34 +
                                SortTypeBox.SelectedIndex + (char)34 + " />";
                }
                else if (PlayListType.SelectedIndex == 8 | PlayListType.SelectedIndex == 9)
                {
                    TopAppend += System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_1" + (char)34 + " value=" + (char)34 +
                                PlayListLocation.Text + (char)34 + " />" + System.Environment.NewLine + "\t" +
                                "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_2" + (char)34 +
                                " value=" + (char)34 + StrmUrlBox.Text + (char)34 + " />" +
                                System.Environment.NewLine + "\t" + "<setting id=" + (char)34 + "Channel_" +
                                ChannelNum + "_3" + (char)34 + " value=" + (char)34 + ShowTitleBox.Text +
                                (char)34 + " />" + System.Environment.NewLine + "\t" + "<setting id=" +
                                (char)34 + "Channel_" + ChannelNum + "_4" + (char)34 + " value=" +
                                (char)34 + ShowDescBox.Text + (char)34 + " />";
                }
                else if (PlayListType.SelectedIndex == 10)
                {
                    if (YouTubeType.SelectedIndex == 6 | YouTubeType.SelectedIndex == 7)
                    {
                        PlayListLocation.Text = YouTubeMulti;
                    }
                    
                    TopAppend += Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_1" + (char)34 + " value=" + (char)34 +
                                PlayListLocation.Text + (char)34 + " />" + System.Environment.NewLine + "\t" +
                                "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_2" + (char)34 +
                                " value=" + (char)34 + YouTubeType.SelectedIndex + 1 + (char)34 + " />" +
                                System.Environment.NewLine + "\t" + "<setting id=" + (char)34 + "Channel_" +
                                ChannelNum + "_3" + (char)34 + " value=" + (char)34 + MediaLimitBox.Text +
                                (char)34 + " />" + System.Environment.NewLine + "\t" + "<setting id=" +
                                (char)34 + "Channel_" + ChannelNum + "_4" + (char)34 + " value=" +
                                (char)34 + SortTypeBox.SelectedIndex + (char)34 + " />";
                }
                else if (PlayListType.SelectedIndex == 11)
                {
                    TopAppend += System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_1" + (char)34 + " value=" + (char)34 +
                                PlayListLocation.Text + (char)34 + " />" + System.Environment.NewLine + "\t" +
                                "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_2" + (char)34 +
                                " value=" + (char)34 + "1" + (char)34 + " />" + System.Environment.NewLine +
                                "\t" + "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_3" +
                                (char)34 + " value=" + (char)34 + MediaLimitBox.Text + (char)34 +
                                " />" + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_4" + (char)34 + " value=" + (char)34 +
                                SortTypeBox.SelectedIndex + (char)34 + " />";
                }
                else if (PlayListType.SelectedIndex == 13)
                {
                    TopAppend += Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_1" + (char)34 + " value=" + (char)34 +
                                SubChannelType.SelectedIndex + 1 + (char)34 + " />" + System.Environment.NewLine +
                                "\t" + "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_2" +
                                (char)34 + " value=" + (char)34 + PlayListLocation.Text + (char)34 +
                                " />" + System.Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_3" + (char)34 + " value=" + (char)34 +
                                MediaLimitBox.Text + (char)34 + " />" + System.Environment.NewLine + "\t" +
                                "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_4" + (char)34 +
                                " value=" + (char)34 + SortTypeBox.SelectedIndex + (char)34 + " />";
                }
                else if (PlayListType.SelectedIndex == 15)
                {
                    TopAppend += Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_1" + (char)34 + " value=" + (char)34 +
                                PlayListLocation.Text + (char)34 + " />" + System.Environment.NewLine + "\t" +
                                "<setting id=" + (char)34 + "Channel_" + ChannelNum + "_2" + (char)34 +
                                " value=" + (char)34 + PluginNotInclude + (char)34 + " />" +
                                System.Environment.NewLine + "\t" + "<setting id=" + (char)34 + "Channel_" +
                                ChannelNum + "_3" + (char)34 + " value=" + (char)34 + MediaLimitBox.Text +
                                (char)34 + " />" + System.Environment.NewLine + "\t" + "<setting id=" +
                                (char)34 + "Channel_" + ChannelNum + "_4" + (char)34 + " value=" +
                                (char)34 + SortTypeBox.SelectedIndex + (char)34 + " />";
                }
                else
                {
                    TopAppend += Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                                "Channel_" + ChannelNum + "_1" + (char)34 + " value=" + (char)34 +
                                Option2.Text + (char)34 + " />";
                }

                //Also append the Rulecount to the top, just underneath the channel type & 2nd value
                TopAppend += Environment.NewLine + "\t" + "<setting id=" + (char)34 +
                            "Channel_" + ChannelNum + "_rulecount" + (char)34 + " value=" + (char)34 +
                            rulecount + (char)34 + " />";

                AppendInfo = TopAppend + AppendInfo;

                //Combine the original text, plus the edited channel at the bottom, followed by ending the settings.
                TextFile += AppendInfo + System.Environment.NewLine + "</settings>";

                PseudoTvUtils.SaveFile(PseudoTvSettingsLocation, TextFile);

                //RefreshALL()
                var returnindex = TVGuideList.SelectedIndices[0];
                RefreshTVGuide();
                TVGuideList.Items[returnindex].Selected = true;
            }
        }

        private void Button6_Click(System.Object sender, System.EventArgs e)
        {
            if (TVShowList.SelectedItems.Count > 0)
            {
                //RefreshAllStudios()
                //TODO:
                //Form3.Visible = true;
                //Form3.Focus();
            }
        }

        private void Button8_Click(System.Object sender, System.EventArgs e)
        {
            if (InterleavedList.SelectedItems.Count > 0)
            {
                InterleavedList.Items[InterleavedList.SelectedIndices[0]].Remove();
            }
        }

        private void Button9_Click(System.Object sender, System.EventArgs e)
        {
            if (SchedulingList.SelectedItems.Count > 0)
            {
                SchedulingList.Items[SchedulingList.SelectedIndices[0]].Remove();
            }
        }

        private void Button7_Click(System.Object sender, System.EventArgs e)
        {
            //TODO:
            //Form4.Visible = true;
        }

        private void Button12_Click(System.Object sender, System.EventArgs e)
        {
            var Response = ShowInputDialog("Enter TV Show's Name", "TV Show Name");

            if (!string.IsNullOrEmpty(Response.Input))
            {
                NotShows.Items.Add(Response);
            }

        }

        private void Button11_Click(System.Object sender, System.EventArgs e)
        {
            if (NotShows.SelectedItems.Count > 0)
            {
                NotShows.Items.RemoveAt(NotShows.SelectedIndex);
            }
        }

        private void Button14_Click(System.Object sender, System.EventArgs e)
        {
            string[] NewItem = new string[6];

            NewItem[0] = ShowInputDialog("Enter Channel Number", "Enter Channel Number").Input;
            NewItem[1] = "1";
            NewItem[2] = null;
            NewItem[3] = null;
            NewItem[4] = null;
            NewItem[5] = null;

            ListViewItem itm = default(ListViewItem);
            itm = new ListViewItem(NewItem);

            bool InList = false;

            if (PseudoTvUtils.IsNumeric(NewItem[0]))
            {
                var firstItemValue = Convert.ToInt32(NewItem[0]);
                if (firstItemValue > 0 & firstItemValue <= 999)
                {
                    for (var x = 0; x <= TVGuideList.Items.Count - 1; x++)
                    {
                        if (TVGuideList.Items[x].SubItems[0].Text == NewItem[0])
                        {
                            MessageBox.Show("You already have a channel " + NewItem[0]);
                            InList = true;
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry, the channel has to be 1 - 999)");
                    InList = true;
                }
            }

            if (InList == false & PseudoTvUtils.IsNumeric(NewItem[0]))
            {
                TVGuideList.Items.Add(itm);

                //Now make that the selected item.
                for (var x = 0; x <= TVGuideList.Items.Count - 1; x++)
                {

                    if (TVGuideList.Items[x].SubItems[0].Text == NewItem[0])
                    {
                        TVGuideList.Items[x].Selected = true;
                    }
                    else if (TVGuideList.Items[x].Selected == true)
                    {
                        TVGuideList.Items[x].Selected = false;
                    }
                }
            }
        }


        private void Button13_Click(System.Object sender, System.EventArgs e)
        {

            if (TVGuideList.Items.Count != 1)
            {
                //Loop through config file.
                //Grab all comments MINUS the ones for selected #
                //Append this & our new content to the file.

                string FILE_NAME = PseudoTvSettingsLocation;
                string TextFile = "";

                var ChannelNum = TVGuideList.Items[TVGuideList.SelectedIndices[0]].SubItems[0].Text;

                //Loop through config file.
                //Grab all comments MINUS the ones for selected #

                if (System.IO.File.Exists(FILE_NAME) == true)
                {
                    System.IO.StreamReader objReader = new System.IO.StreamReader(FILE_NAME);


                    while (objReader.Peek() != -1)
                    {
                        var SingleLine = objReader.ReadLine();

                        if (
                            SingleLine.Contains("<setting id=" + (char)34 + "Channel_" + ChannelNum + "_") ||
                            SingleLine.Contains("</settings"))
                        {
                        }
                        else
                        {
                            TextFile = TextFile + SingleLine + System.Environment.NewLine;
                        }

                    }

                    objReader.Close();

                }
                else
                {
                    MessageBox.Show("File Does Not Exist");

                }

                PseudoTvUtils.SaveFile(PseudoTvSettingsLocation, TextFile);

                RefreshTVGuide();

                TVGuideList.SelectedItems.Clear();

                //Clear everything on the form.

                //Reset the checked options.
                ChkLogo.Checked = false;
                chkDontPlayChannel.Checked = false;
                ChkRandom.Checked = false;
                ChkRealTime.Checked = false;
                ChkResume.Checked = false;
                ChkIceLibrary.Checked = false;
                ChkExcludeBCT.Checked = false;
                ChkPopup.Checked = false;
                ChkUnwatched.Checked = false;
                ChkWatched.Checked = false;
                ChkPause.Checked = false;
                ChkPlayInOrder.Checked = false;
                ResetDays.Clear();
                ChannelName.Clear();

                //Clear other form items.
                TVGuideSubMenu.Items.Clear();
                Option2.Items.Clear();
                InterleavedList.Items.Clear();
                SchedulingList.Items.Clear();
                NotShows.Items.Clear();

            }
            else
            {
                MessageBox.Show("You must have at least one channel");
            }
        }

        private void Button10_Click(System.Object sender, System.EventArgs e)
        {
            //TODO: Form5.Visible = true;
        }

        private void AaaToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            //TODO:
            //Form6 mySettings = new Form6();
            //mySettings.Version = this.Version;
            //mySettings.Show();
        }

        private void DontShowToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            if (TVGuideSubMenu.SelectedItems.Count > 0)
            {
                NotShows.Items.Add(TVGuideSubMenu.Items[TVGuideSubMenu.SelectedIndices[0]].SubItems[0].Text);
                TVGuideSubMenu.Items[TVGuideSubMenu.SelectedIndices[0]].BackColor = Color.Red;
            }
        }

        private void ListMoviePosters_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            int x = ListMoviePosters.SelectedIndex;

            MoviePicture.ImageLocation = ListMoviePosters.Items[x].ToString();
            MoviePicture.Refresh();

        }

        private void MoviePosterSelect_Click(System.Object sender, System.EventArgs e)
        {
            int x = ListMoviePosters.SelectedIndex;
            string Type = "poster";
            string MediaType = "movie";


            if (MovieLocation.TextLength >= 6)
            {
                if (MovieLocation.Text.Substring(0, 6) == "smb://")
                {
                    MovieLocation.Text = MovieLocation.Text.Replace("/", "\\");
                    MovieLocation.Text = "\\\\" + MovieLocation.Text.Substring(6);
                }
            }

            string directoryName = "";
            directoryName = Path.GetDirectoryName(MovieLocation.Text);

            // Displays a SaveFileDialog so the user can save the Image
            // assigned to TVPosterSelect.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = directoryName;
            saveFileDialog1.Filter = "JPeg Image|*.jpg";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.FileName = "poster.jpg";
            saveFileDialog1.ShowDialog();

            string FileToSaveAs = System.IO.Path.Combine(saveFileDialog1.InitialDirectory, saveFileDialog1.FileName);
            MoviePicture.Image.Save(FileToSaveAs, System.Drawing.Imaging.ImageFormat.Jpeg);

            PseudoTvUtils.DbExecute("UPDATE art SET url = '" + ListMoviePosters.Items[x].ToString() +
                                    "' WHERE media_id = '" +
                                    MovieIDLabel.Text + "' and media_type = '" + MediaType + "' and type = '" + Type +
                                    "'");
            //TODO: VisualStyleElement.Status.Text = "Updated " + MovieLabel.Text + " " + MovieIDLabel.Text +
            //                                " Successfully with " + ListMoviePosters.Items[x].ToString() + "";

        }


        private void MovieList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (MovieList.SelectedItems.Count > 0)
            {
                MoviePicture.Update();

                ListViewItem ListItem = default(ListViewItem);
                ListItem = MovieList.SelectedItems[0];

                string MovieName = null;
                string MovieID = null;

                MovieID = ListItem.SubItems[1].Text;
                MovieName = ListItem.SubItems[0].Text;
                MovieIDLabel.Text = MovieID;


                var SelectArray = new[] { 16, 24, 20, 10 };

                //Shoot it over to the ReadRecord sub, 
                string[] ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                    "SELECT * FROM movie WHERE idMovie='" + MovieID + "'", SelectArray);

                string[] ReturnArraySplit = null;

                //We only have 1 response, since it searches by ID. So, just break it into parts. 
                ReturnArraySplit = ReturnArray[0].Split('~');

                string MoviePoster = ReturnArraySplit[3];
                ListMoviePosters.Items.Clear();

                if (MoviePoster.Contains("<thumb aspect=\"poster\" preview=\"") ||
                    MoviePoster.Contains("<thumb>"))
                {
                    string[] MoviePosterSplit = MoviePoster.Split(Convert.ToChar("<thumb aspect=\"poster\" preview=\""));

                    for (var x = 1; x <= MoviePosterSplit.Length; x++)
                    {
                        int i = MoviePosterSplit[x].IndexOf("<thumb aspect=\"poster\" preview=\"");
                        MoviePosterSplit[x] = MoviePosterSplit[x].Substring(i + 1, MoviePosterSplit[x].IndexOf("\">"));
                        ListMoviePosters.Items.Add(MoviePosterSplit[x]);
                    }

                    string[] MoviePosterSplit2 = MoviePoster.Split(Convert.ToChar("<thumb>"));

                    for (var x = 1; x <= MoviePosterSplit2.Length; x++)
                    {
                        int i = MoviePosterSplit2[x].IndexOf("<thumb>");
                        MoviePosterSplit2[x] = MoviePosterSplit2[x]
                            .Substring(i + 1, MoviePosterSplit2[x].IndexOf("</thumb>"));
                        ListMoviePosters.Items.Add(MoviePosterSplit2[x]);
                    }
                }
                else
                {
                    ListMoviePosters.Items.Add("Nothing Found");
                }

                string MovieGenres = ReturnArraySplit[0];

                MovieLabel.Text = MovieName;
                MovieLocation.Text = ReturnArraySplit[1];

                if (string.IsNullOrEmpty(ReturnArraySplit[2]))
                {
                    txtMovieNetwork.SelectedIndex = -1;
                }
                else
                {
                    txtMovieNetwork.Text = ReturnArraySplit[2];
                }

                //Loop through each Movie Genre, if there more than one.
                MovieGenresList.Items.Clear();
                if (MovieGenres.Contains(" / "))
                {
                    string[] MovieGenresSplit = MovieGenres.Split(Convert.ToChar(" / "));

                    for (var x = 0; x <= MovieGenresSplit.Length; x++)
                    {
                        MovieGenresList.Items.Add(MovieGenresSplit[x]);
                    }
                }
                else if (!string.IsNullOrEmpty(MovieGenres))
                {
                    MovieGenresList.Items.Add(MovieGenres);
                }


                if (MovieLocation.TextLength >= 6)
                {
                    if (MovieLocation.Text.Substring(0, 6) == "smb://")
                    {
                        MovieLocation.Text = MovieLocation.Text.Replace("/", "\\");
                        MovieLocation.Text = "\\\\" + MovieLocation.Text.Substring(6);
                    }
                }

                string directoryName = "";
                directoryName = Path.GetDirectoryName(MovieLocation.Text);

                if (System.IO.File.Exists(directoryName + "\\" + "poster.jpg"))
                {
                    MoviePicture.ImageLocation = (directoryName + "\\" + "poster.jpg");
                }
                else
                {
                    MoviePicture.ImageLocation =
                        "https://github.com/Lunatixz/script.pseudotv.live/raw/development/resources/images/poster.png";
                }

            }
        }



        private void Button16_Click(System.Object sender, System.EventArgs e)
        {
            //TODO:
            //Form7.Visible = true;
            //Form7.Focus();
        }

        private void Button15_Click(System.Object sender, System.EventArgs e)
        {

            if (MovieGenresList.SelectedIndex >= 0)
            {
                //Grab the 3rd column from the TVShowList, which is the TVShowID
                var GenreID = LookUpGenre(MovieGenresList.Items[MovieGenresList.SelectedIndex].ToString());

                //Now, remove the link in the database.
                //PseudoTvUtils.DbExecute("DELETE FROM genrelinktvshow WHERE idGenre = '" & GenreID & "' AND idShow ='" & TVShowList.Items(TVShowList.SelectedIndices[0]).SubItems[2].Text & "'")


                MovieGenresList.Items.RemoveAt(MovieGenresList.SelectedIndex);
                // SaveTVShow_Click(Nothing, Nothing)
                RefreshGenres();
            }
        }

        public void RefreshNetworkListMovies()
        {
            MovieNetworkList.Items.Clear();

            var SelectArray = new[] { 2, 20 };

            string[] ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                "SELECT * FROM movie ORDER BY c18 ASC",
                SelectArray);

            //Loop through each returned Movie
            for (var x = 0; x <= ReturnArray.Length - 1; x++)
            {
                if ((string.IsNullOrEmpty(ReturnArray[x])))
                {
                    continue;
                }

                string[] ReturnArraySplit = null;

                string ShowName = null;
                string ShowNetwork = null;

                ReturnArraySplit = ReturnArray[x].Split('~');

                ShowName = ReturnArraySplit[0];
                //Updated ReturnArraySplit for ShowNetwork to reflect MyVideos78.db schema
                ShowNetwork = ReturnArraySplit[1];

                bool NetworkListed = false;


                for (var y = 0; y <= MovieNetworkList.Items.Count - 1; y++)
                {
                    if (MovieNetworkList.Items[y].SubItems[0].Text == ShowNetwork)
                    {
                        NetworkListed = true;
                        MovieNetworkList.Items[y].SubItems[1].Text = MovieNetworkList.Items[y].SubItems[1].Text + 1;
                    }

                }

                if (NetworkListed == false)
                {
                    ListViewItem itm = default(ListViewItem);
                    string[] str = new string[3];

                    str[0] = ShowNetwork;
                    str[1] = "1";
                    //test
                    itm = new ListViewItem(str);

                    //Add the item to the TV show list.
                    MovieNetworkList.Items.Add(itm);

                }

            }

        }

        public void RefreshNetworkList()
        {
            NetworkList.Items.Clear();

            var SelectArray = new[] { 1, 15 };

            string[] ReturnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                "SELECT * FROM tvshow ORDER BY c14 ASC",
                SelectArray);

            //Loop through each returned TV show.

            for (var x = 0; x <= ReturnArray.Length - 1; x++)
            {

                string[] ReturnArraySplit = null;

                string ShowName = null;
                string ShowNetwork = null;

                ReturnArraySplit = ReturnArray[x].Split('~');

                ShowName = ReturnArraySplit[0];
                ShowNetwork = ReturnArraySplit[1];

                var NetworkListed = false;

                for (var y = 0; y <= NetworkList.Items.Count - 1; y++)
                {
                    if (NetworkList.Items[y].SubItems[0].Text == ShowNetwork)
                    {
                        NetworkListed = true;
                        NetworkList.Items[y].SubItems[1].Text = NetworkList.Items[y].SubItems[1].Text + 1;
                    }

                }

                if (NetworkListed == false)
                {
                    ListViewItem itm = default(ListViewItem);
                    string[] str = new string[3];

                    str[0] = ShowNetwork;
                    str[1] = "1";

                    itm = new ListViewItem(str);

                    //Add the item to the TV show list.
                    NetworkList.Items.Add(itm);

                }

            }

        }

        private void Button17_Click(System.Object sender, System.EventArgs e)
        {

            if (MovieList.SelectedItems.Count > 0)
            {
                // Fix any issues with shows and 's.
                string MovieName = MovieLabel.Text;
                //Convert show genres into the format ex:  genre1 / genre2 / etc.
                var MovieGenres = ConvertGenres(MovieGenresList);
                MovieName = MovieName.Replace("'", "''");
                //Grab the Network ID based on the name
                var NetworkID = LookUpNetwork(txtMovieNetwork.Text);
                string MovieID = MovieList.SelectedItems[0].SubItems[1].Text;

                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    PseudoTvUtils.DbExecute("DELETE FROM studio_link WHERE media_id = '" + MovieID + "'");
                    PseudoTvUtils.DbExecute("INSERT INTO studio_link (studio_id, media_id, media_type) VALUES ('" +
                                            NetworkID + "', '" +
                                            MovieID + "', 'movie')");
                }
                else
                {
                    PseudoTvUtils.DbExecute("DELETE FROM studiolinkmovie WHERE idMovie = '" + MovieID + "'");
                    PseudoTvUtils.DbExecute("INSERT INTO studiolinkmovie (idStudio, idMovie) VALUES ('" + NetworkID +
                                            "', '" + MovieID +
                                            "')");
                }

                PseudoTvUtils.DbExecute("UPDATE movie SET c14 = '" + MovieGenres + "', c18 ='" + txtMovieNetwork.Text +
                                        "' WHERE idMovie = '" + MovieID + "'");
                //TODO: VisualStyleElement.Status.Text = "Updated " + MovieLabel.Text + " Successfully";

                if ((KodiVersion >= KodiVersion.Isengard))
                {
                    //Remove all genres from tv show
                    PseudoTvUtils.DbExecute("DELETE FROM genre_link  WHERE media_id = '" + MovieID + "'");

                    //add each one.  one by one.
                    for (var x = 0; x <= MovieGenresList.Items.Count - 1; x++)
                    {
                        var GenreID = LookUpGenre(MovieGenresList.Items[x].ToString());
                        PseudoTvUtils.DbExecute("INSERT INTO genre_link  (genre_id, media_id, media_type) VALUES ('" +
                                                GenreID +
                                                "', '" + MovieID + "', 'movie')");
                    }
                }
                else
                {
                    //Remove all genres from tv show
                    PseudoTvUtils.DbExecute("DELETE FROM genrelinkmovie  WHERE idMovie = '" + MovieID + "'");

                    //add each one.  one by one.
                    for (var x = 0; x <= MovieGenresList.Items.Count - 1; x++)
                    {
                        var GenreID = LookUpGenre(MovieGenresList.Items[x].ToString());
                        PseudoTvUtils.DbExecute("INSERT INTO genrelinkmovie (idGenre, idMovie) VALUES ('" + GenreID +
                                                "', '" + MovieID +
                                                "')");
                    }
                }

                //Save our spot on the list.
                var SavedName = txtMovieNetwork.Text;

                //Refresh Things
                RefreshNetworkListMovies();
                RefreshGenres();

                var returnindex = MovieList.SelectedIndices[0];
                RefreshMovieList();
                MovieList.Items[returnindex].Selected = true;

            }
        }

        private void Button18_Click(System.Object sender, System.EventArgs e)
        {
            if (MovieList.SelectedItems.Count > 0)
            {
                RefreshAllStudios();
                //TODO:
                //Form8.Visible = true;
                //Form8.Focus();
            }
        }

        private void MovieNetworkList_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Get the new sorting column. 
            ColumnHeader new_sorting_column = MovieNetworkList.Columns[e.Column];
            // Figure out the new sorting order. 
            System.Windows.Forms.SortOrder sort_order = default(System.Windows.Forms.SortOrder);
            if (m_SortingColumn == null)
            {
                // New column. Sort ascending. 
                sort_order = SortOrder.Ascending;
                // See if this is the same column. 
            }
            else
            {
                if (new_sorting_column.Equals(m_SortingColumn))
                {
                    // Same column. Switch the sort order. 
                    if (m_SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending. 
                    sort_order = SortOrder.Ascending;
                }
                // Remove the old sort indicator. 
                m_SortingColumn.Text = m_SortingColumn.Text.Substring(2);
            }
            // Display the new sort order. 
            m_SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                m_SortingColumn.Text = "> " + m_SortingColumn.Text;
            }
            else
            {
                m_SortingColumn.Text = "< " + m_SortingColumn.Text;
            }
            // Create a comparer. 
            MovieNetworkList.ListViewItemSorter = new ClsListviewSorter(e.Column, sort_order);
            // Sort. 
            MovieNetworkList.Sort();
        }

        private void MovieNetworkList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            MovieNetworkListSubList.Items.Clear();


            if (MovieNetworkList.SelectedIndices.Count > 0)
            {
                var SelectArray = new[] { 2 };


                var returnArray = PseudoTvUtils.DbReadRecord(VideoDatabaseLocation,
                    "SELECT * FROM movie WHERE c18='" +
                    MovieNetworkList.Items[MovieNetworkList.SelectedIndices[0]].SubItems[0].Text + "'", SelectArray);

                for (var x = 0; x <= returnArray.Length - 1; x++)
                {
                    MovieNetworkListSubList.Items.Add(returnArray[x]);
                }

            }
        }

        private void Button19_Click(System.Object sender, System.EventArgs e)
        {
            LookUpGenre("aaccc");
        }

        private void YouTubeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (YouTubeType.SelectedIndex == 0)
            {
                Label6.Text = "Channel/User:";
                var _with32 = PlayListLocation;
                _with32.Location = new System.Drawing.Point(270, 120);
                PlayListLocation.Visible = true;
                NotShows.Visible = false;
                AddExcludeBtn.Visible = false;
                DelExcludeBtn.Visible = false;
                Label12.Visible = false;
            }
            else if (YouTubeType.SelectedIndex == 1)
            {
                Label6.Text = "Playlist:";
                var _with33 = PlayListLocation;
                _with33.Location = new System.Drawing.Point(220, 120);
                PlayListLocation.Visible = true;
                NotShows.Visible = false;
                AddExcludeBtn.Visible = false;
                DelExcludeBtn.Visible = false;
                Label12.Visible = false;
            }
            else if (YouTubeType.SelectedIndex == 2 | YouTubeType.SelectedIndex == 3)
            {
                Label6.Text = "Username:";
                var _with34 = PlayListLocation;
                _with34.Location = new System.Drawing.Point(245, 120);
                PlayListLocation.Visible = true;
                NotShows.Visible = false;
                AddExcludeBtn.Visible = false;
                DelExcludeBtn.Visible = false;
                Label12.Visible = false;
            }
            else if (YouTubeType.SelectedIndex == 4)
            {
                Label6.Text = "Search String:";
                var _with35 = PlayListLocation;
                _with35.Location = new System.Drawing.Point(270, 120);
                PlayListLocation.Visible = true;
                NotShows.Visible = false;
                AddExcludeBtn.Visible = false;
                DelExcludeBtn.Visible = false;
                Label12.Visible = false;
            }
            else if (YouTubeType.SelectedIndex == 6)
            {
                Label6.Text = "";
                var _with36 = PlayListLocation;
                _with36.Location = new System.Drawing.Point(295, 120);
                PlayListLocation.Visible = false;
                NotShows.Visible = true;
                AddExcludeBtn.Visible = true;
                DelExcludeBtn.Visible = true;
                Label12.Text = "Add/Remove Playlists";
                Label12.Visible = true;
            }
            else if (YouTubeType.SelectedIndex == 7)
            {
                Label6.Text = "";
                var _with37 = PlayListLocation;
                _with37.Location = new System.Drawing.Point(295, 120);
                PlayListLocation.Visible = false;
                NotShows.Visible = true;
                AddExcludeBtn.Visible = true;
                DelExcludeBtn.Visible = true;
                Label12.Text = "Add/Remove Channels";
                Label12.Visible = true;
            }
            else if (YouTubeType.SelectedIndex == 8)
            {
                Label6.Text = "GData Url:";
                var _with38 = PlayListLocation;
                _with38.Location = new System.Drawing.Point(245, 120);
                PlayListLocation.Visible = true;
                NotShows.Visible = false;
                AddExcludeBtn.Visible = false;
                DelExcludeBtn.Visible = false;
                Label12.Visible = false;
                GDataDemoLink.Links.Remove(GDataDemoLink.Links[0]);
                //TODO: Check this logic... it wants an int as the second param but in vb they were giving a bool?
                GDataDemoLink.Links.Add(0, GDataDemoLink.Text == "GDataDemo" ? 1 : 0,
                    "http://gdata.youtube.com/demo/index.html");
                GDataDemoLink.Visible = true;
            }
            else
            {
                Label6.Text = "Nothing Here";
                PlayListLocation.Visible = false;
                NotShows.Visible = false;
                AddExcludeBtn.Visible = false;
                DelExcludeBtn.Visible = false;
                Label12.Visible = false;
            }

        }

        private void SubChannelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PlayListType.SelectedIndex == 13 & SubChannelType.SelectedIndex == 0)
            {
                Label6.Text = "LastFM User:";
                var _with39 = PlayListLocation;
                _with39.Location = new System.Drawing.Point(270, 120);
            }
            else if (PlayListType.SelectedIndex == 13 & SubChannelType.SelectedIndex == 1)
            {
                Label6.Text = "Channel_##:";
                var _with40 = PlayListLocation;
                _with40.Location = new System.Drawing.Point(295, 120);
            }
        }

        private void AddBanner_Click(object sender, EventArgs e)
        {
            //TODO:
            //Form9.Visible = true;
            //Form9.Focus();
            //Form9.AddBannerPictureBox.ImageLocation =
            //    "http://github.com/Lunatixz/script.pseudotv.live/raw/development/resources/images/banner.png";
        }

        private void AddPoster_Click(object sender, EventArgs e)
        {
            //TODO:
            //Form10.Visible = true;
            //Form10.Focus();
            //Form10.AddPosterPictureBox.ImageLocation =
            //    "http://github.com/Lunatixz/script.pseudotv.live/raw/development/resources/images/poster.png";
        }


        private void AddMoviePosterButton_Click(object sender, EventArgs e)
        {
            //TODO:
            //Form11.Visible = true;
            //Form11.Focus();
            //Form11.AddMoviePosterPictureBox.ImageLocation =
            //    "http://github.com/Lunatixz/script.pseudotv.live/raw/development/resources/images/poster.png";
        }

        private void GDataDemoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
        }


        private static InputDialogResponse ShowInputDialog(string question, string inputTxt = null)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = inputTxt ?? question;

            System.Windows.Forms.TextBox textBox = new TextBox
            {
                Size = new System.Drawing.Size(size.Width - 10, 23),
                Location = new System.Drawing.Point(5, 5),
                Text = question
            };
            inputBox.Controls.Add(textBox);

            Button okButton = new Button
            {
                DialogResult = System.Windows.Forms.DialogResult.OK,
                Name = "okButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&OK",
                Location = new System.Drawing.Point(size.Width - 80 - 80, 39)
            };
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel,
                Name = "cancelButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&Cancel",
                Location = new System.Drawing.Point(size.Width - 80, 39)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();

            return new InputDialogResponse
            {
                DialogResult = result,
                Input = textBox.Text
            };
        }

        private void AddExcludeBtn_Click(object sender, EventArgs e)
        {

            if (PlayListType.SelectedIndex == 10)
            {
                var response = ShowInputDialog("Enter Playlist or User String").Input;

                if (!string.IsNullOrEmpty(response))
                {
                    NotShows.Items.Add(response);
                }

                if (string.IsNullOrEmpty(YouTubeMulti))
                {
                    YouTubeMulti = response;
                }
                else
                {
                    YouTubeMulti = YouTubeMulti + "|" + response;
                }
            }
            else
            {
                var response = ShowInputDialog("Enter Exclude String").Input;

                if (!string.IsNullOrEmpty(response))
                {
                    NotShows.Items.Add(response);
                }

                if (string.IsNullOrEmpty(PluginNotInclude))
                {
                    PluginNotInclude = response;
                }
                else
                {
                    PluginNotInclude = PluginNotInclude + "," + response;
                }
            }

        }

        private void DelExclutn_Click(object sender, EventArgs e)
        {
            if (PlayListType.SelectedIndex == 10)
            {
                if (NotShows.SelectedItems.Count > 0)
                {
                    NotShows.Items.RemoveAt(NotShows.SelectedIndex);
                }

                string[] items = NotShows.Items.OfType<object>().Select(item => item.ToString()).ToArray();
                string result = string.Join("|", items);

                YouTubeMulti = result;
            }
            else
            {
                if (NotShows.SelectedItems.Count > 0)
                {
                    NotShows.Items.RemoveAt(NotShows.SelectedIndex);
                }

                string[] items = NotShows.Items.OfType<object>().Select(item => item.ToString()).ToArray();
                string result = string.Join(",", items);

                PluginNotInclude = result;
            }
        }
    }

    public class InputDialogResponse
    {
        public DialogResult DialogResult { get; set; }
        public string Input { get; set; }
    }

    public class ClsListviewSorter : IComparer
    {
        public ClsListviewSorter(int i, SortOrder @ascending)
        {
            throw new NotImplementedException();
        }

        public int Compare(object x, object y)
        {
            throw new NotImplementedException();
        }
    }
}