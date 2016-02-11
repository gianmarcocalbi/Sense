﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgettoGUI {
	public partial class Form1 : Form {
		private Parser parser;
		Thread threadParser;

		public Form1() {
			InitializeComponent();
			threadParser = new Thread(threadParserFunction);
		}

		private void threadParserFunction() {
			parser = new Parser(Int32.Parse(textBoxPort.Text), String.Format("{0}.{1}.{2}.{3}", textBoxIP1.Text, textBoxIP2.Text, textBoxIP3.Text, textBoxIP4.Text), printToServerConsoleProtected);
		}

		private void label1_Click(object sender, EventArgs e) {

		}

		private void textBox1_TextChanged(object sender, EventArgs e) {

		}

		private void textBox2_TextChanged(object sender, EventArgs e) {

		}

		private void buttonServerStartClick(object sender, EventArgs e) {
			if (threadParser.ThreadState == ThreadState.Running) {
				//STOPPING SERVER
				//bisogna fermare il server
				//parser.Server.Close();
				buttonServerStart.Text = "START";
			} else {
				//STARTING SERVER
				try {
					//richTextConsole.AppendText(String.Format("Server Started on port {0} at IP {1}\n", parser.Port, parser.LocalAddr));		
					threadParser.Start();
					buttonServerStart.Text = "STOP";
				} catch (SocketException exc) {
					richTextConsole.AppendText(String.Format("{0}\n", exc));
				}
			}
		}

		private void Form1_Load(object sender, EventArgs e) {

		}
		
		public delegate void printToServerConsoleDelegate(string s);

		public void printToServerConsoleProtected(string s) {
			if (this.richTextConsole.InvokeRequired)
				Invoke(new printToServerConsoleDelegate(printToServerConsoleProtected), new object[] { s });
			else
				richTextConsole.AppendText(s);
		}
	}
}
