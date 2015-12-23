namespace PseudoTV_Manager.Forms
{
    partial class SettingsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.XbmcVersion = new System.Windows.Forms.ComboBox();
            this.XbmcVersionLbl = new System.Windows.Forms.Label();
            this.Button4 = new System.Windows.Forms.Button();
            this.TxtAddonDatabaseLocation = new System.Windows.Forms.TextBox();
            this.AddonDbLocDef = new System.Windows.Forms.Label();
            this.AddonDbLoc = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.TxtVideoDbLocation = new System.Windows.Forms.TextBox();
            this.Button1 = new System.Windows.Forms.Button();
            this.TabPage2 = new System.Windows.Forms.TabPage();
            this.TextBox7 = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.TextBox6 = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.TextBox5 = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.TextBox4 = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.TextBox3 = new System.Windows.Forms.TextBox();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.Button2 = new System.Windows.Forms.Button();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TxtPseudoTvSettingsLocation = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Button3 = new System.Windows.Forms.Button();
            this.TabPage1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // XbmcVersion
            // 
            this.XbmcVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.XbmcVersion.FormattingEnabled = true;
            this.XbmcVersion.Items.AddRange(new object[] {
            "Gotham",
            "Helix",
            "Isengard",
            "Jarvis"});
            this.XbmcVersion.Location = new System.Drawing.Point(80, 9);
            this.XbmcVersion.Name = "XbmcVersion";
            this.XbmcVersion.Size = new System.Drawing.Size(121, 21);
            this.XbmcVersion.TabIndex = 27;
            // 
            // XbmcVersionLbl
            // 
            this.XbmcVersionLbl.AutoSize = true;
            this.XbmcVersionLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.XbmcVersionLbl.Location = new System.Drawing.Point(14, 10);
            this.XbmcVersionLbl.Name = "XbmcVersionLbl";
            this.XbmcVersionLbl.Size = new System.Drawing.Size(60, 17);
            this.XbmcVersionLbl.TabIndex = 26;
            this.XbmcVersionLbl.Text = "Version:";
            // 
            // Button4
            // 
            this.Button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button4.Location = new System.Drawing.Point(425, 396);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(30, 18);
            this.Button4.TabIndex = 25;
            this.Button4.Text = "...";
            this.Button4.UseVisualStyleBackColor = true;
            // 
            // TxtAddonDatabaseLocation
            // 
            this.TxtAddonDatabaseLocation.Location = new System.Drawing.Point(13, 394);
            this.TxtAddonDatabaseLocation.Name = "TxtAddonDatabaseLocation";
            this.TxtAddonDatabaseLocation.Size = new System.Drawing.Size(406, 20);
            this.TxtAddonDatabaseLocation.TabIndex = 24;
            // 
            // AddonDbLocDef
            // 
            this.AddonDbLocDef.AutoSize = true;
            this.AddonDbLocDef.Location = new System.Drawing.Point(10, 365);
            this.AddonDbLocDef.Name = "AddonDbLocDef";
            this.AddonDbLocDef.Size = new System.Drawing.Size(399, 26);
            this.AddonDbLocDef.TabIndex = 23;
            this.AddonDbLocDef.Text = "Typically located :\r\nC:\\Users\\Username\\AppData\\Roaming\\XBMC\\userdata\\Database\\Add" +
    "ons19.db \r\n";
            // 
            // AddonDbLoc
            // 
            this.AddonDbLoc.AutoSize = true;
            this.AddonDbLoc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddonDbLoc.Location = new System.Drawing.Point(11, 349);
            this.AddonDbLoc.Name = "AddonDbLoc";
            this.AddonDbLoc.Size = new System.Drawing.Size(172, 16);
            this.AddonDbLoc.TabIndex = 22;
            this.AddonDbLoc.Text = "Addons Database Location";
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(3, 121);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(74, 26);
            this.Label9.TabIndex = 9;
            this.Label9.Text = "Port:\r\n(Default 3306)";
            // 
            // TabPage1
            // 
            this.TabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.TabPage1.Controls.Add(this.Label2);
            this.TabPage1.Controls.Add(this.Label1);
            this.TabPage1.Controls.Add(this.TxtVideoDbLocation);
            this.TabPage1.Controls.Add(this.Button1);
            this.TabPage1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.TabPage1.Location = new System.Drawing.Point(4, 22);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage1.Size = new System.Drawing.Size(454, 202);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "SQLite (Default)";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(7, 23);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(409, 26);
            this.Label2.TabIndex = 1;
            this.Label2.Text = "Typically located :\r\nC:\\Users\\Username\\AppData\\Roaming\\XBMC\\userdata\\Database\\MyV" +
    "ideos78.db ";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(7, 7);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(164, 16);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Video Database Location:";
            // 
            // TxtVideoDbLocation
            // 
            this.TxtVideoDbLocation.Location = new System.Drawing.Point(7, 52);
            this.TxtVideoDbLocation.Name = "TxtVideoDbLocation";
            this.TxtVideoDbLocation.Size = new System.Drawing.Size(406, 20);
            this.TxtVideoDbLocation.TabIndex = 2;
            // 
            // Button1
            // 
            this.Button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button1.Location = new System.Drawing.Point(417, 53);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(30, 18);
            this.Button1.TabIndex = 3;
            this.Button1.Text = "...";
            this.Button1.UseVisualStyleBackColor = true;
            // 
            // TabPage2
            // 
            this.TabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.TabPage2.Controls.Add(this.Label9);
            this.TabPage2.Controls.Add(this.TextBox7);
            this.TabPage2.Controls.Add(this.Label8);
            this.TabPage2.Controls.Add(this.TextBox6);
            this.TabPage2.Controls.Add(this.Label7);
            this.TabPage2.Controls.Add(this.TextBox5);
            this.TabPage2.Controls.Add(this.Label6);
            this.TabPage2.Controls.Add(this.TextBox4);
            this.TabPage2.Controls.Add(this.Label5);
            this.TabPage2.Controls.Add(this.TextBox3);
            this.TabPage2.Location = new System.Drawing.Point(4, 22);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage2.Size = new System.Drawing.Size(454, 202);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "MySQL";
            // 
            // TextBox7
            // 
            this.TextBox7.Location = new System.Drawing.Point(137, 127);
            this.TextBox7.Name = "TextBox7";
            this.TextBox7.Size = new System.Drawing.Size(215, 20);
            this.TextBox7.TabIndex = 8;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(3, 163);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(86, 26);
            this.Label8.TabIndex = 7;
            this.Label8.Text = "Video Database \r\nTable Name:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextBox6
            // 
            this.TextBox6.Location = new System.Drawing.Point(137, 167);
            this.TextBox6.Name = "TextBox6";
            this.TextBox6.Size = new System.Drawing.Size(215, 20);
            this.TextBox6.TabIndex = 6;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(3, 87);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(56, 13);
            this.Label7.TabIndex = 5;
            this.Label7.Text = "Password:";
            // 
            // TextBox5
            // 
            this.TextBox5.Location = new System.Drawing.Point(137, 84);
            this.TextBox5.Name = "TextBox5";
            this.TextBox5.Size = new System.Drawing.Size(215, 20);
            this.TextBox5.TabIndex = 4;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(3, 51);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(58, 13);
            this.Label6.TabIndex = 3;
            this.Label6.Text = "Username:";
            // 
            // TextBox4
            // 
            this.TextBox4.Location = new System.Drawing.Point(137, 48);
            this.TextBox4.Name = "TextBox4";
            this.TextBox4.Size = new System.Drawing.Size(215, 20);
            this.TextBox4.TabIndex = 2;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(3, 13);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(82, 13);
            this.Label5.TabIndex = 1;
            this.Label5.Text = "Server Address:";
            // 
            // TextBox3
            // 
            this.TextBox3.Location = new System.Drawing.Point(137, 10);
            this.TextBox3.Name = "TextBox3";
            this.TextBox3.Size = new System.Drawing.Size(215, 20);
            this.TextBox3.TabIndex = 0;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            this.TabControl1.Location = new System.Drawing.Point(13, 36);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(462, 228);
            this.TabControl1.TabIndex = 21;
            // 
            // Button2
            // 
            this.Button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button2.Location = new System.Drawing.Point(425, 326);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(30, 18);
            this.Button2.TabIndex = 19;
            this.Button2.Text = "...";
            this.Button2.UseVisualStyleBackColor = true;
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.FileName = "OpenFileDialog1";
            // 
            // TxtPseudoTvSettingsLocation
            // 
            this.TxtPseudoTvSettingsLocation.Location = new System.Drawing.Point(13, 326);
            this.TxtPseudoTvSettingsLocation.Name = "TxtPseudoTvSettingsLocation";
            this.TxtPseudoTvSettingsLocation.Size = new System.Drawing.Size(406, 20);
            this.TxtPseudoTvSettingsLocation.TabIndex = 18;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(10, 297);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(252, 26);
            this.Label3.TabIndex = 17;
            this.Label3.Text = "Typically located:\r\nuserdata\\addon_data\\script.pseudotv\\settings2.xml";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(10, 281);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(188, 16);
            this.Label4.TabIndex = 16;
            this.Label4.Text = "PseudoTV Settings2.XML File:";
            // 
            // Button3
            // 
            this.Button3.Location = new System.Drawing.Point(187, 420);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(75, 23);
            this.Button3.TabIndex = 20;
            this.Button3.Text = "Save";
            this.Button3.UseVisualStyleBackColor = true;
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 453);
            this.Controls.Add(this.XbmcVersion);
            this.Controls.Add(this.XbmcVersionLbl);
            this.Controls.Add(this.Button4);
            this.Controls.Add(this.TxtAddonDatabaseLocation);
            this.Controls.Add(this.AddonDbLocDef);
            this.Controls.Add(this.AddonDbLoc);
            this.Controls.Add(this.TabControl1);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.TxtPseudoTvSettingsLocation);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.Button3);
            this.Name = "SettingsWindow";
            this.Text = "SettingsWindow";
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.TabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ComboBox XbmcVersion;
        internal System.Windows.Forms.Label XbmcVersionLbl;
        internal System.Windows.Forms.Button Button4;
        internal System.Windows.Forms.TextBox TxtAddonDatabaseLocation;
        internal System.Windows.Forms.Label AddonDbLocDef;
        internal System.Windows.Forms.Label AddonDbLoc;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.TabPage TabPage1;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox TxtVideoDbLocation;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.TabPage TabPage2;
        internal System.Windows.Forms.TextBox TextBox7;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.TextBox TextBox6;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.TextBox TextBox5;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.TextBox TextBox4;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.TextBox TextBox3;
        internal System.Windows.Forms.TabControl TabControl1;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        internal System.Windows.Forms.TextBox TxtPseudoTvSettingsLocation;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Button Button3;
    }
}