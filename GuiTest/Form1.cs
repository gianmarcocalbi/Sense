﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuiTest {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e) {

		}

		private void button1_Click(object sender, EventArgs e) {
			DialogResult result = folderBrowserDialog1.ShowDialog();
		}
	}
}
