using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using NLog;

namespace PseudoTV_Manager
{
	public partial class MainWindow : Form
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			Database.Instance.Connect();
		}
	}
}
