using System;
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
			this.comboBoxFrequenza.SelectedIndex = comboBoxFrequenza.FindStringExact("50");
			parser = new Parser(Int32.Parse(textBoxPort.Text), String.Format("{0}.{1}.{2}.{3}", textBoxIP1.Text, textBoxIP2.Text, textBoxIP3.Text, textBoxIP4.Text), printToServerConsoleProtected, setButtonServerStartProtected);
			threadParser = new Thread(parser.StartServer);
			threadParser.IsBackground = true;
			threadParser.Start();
		}

		private void label1_Click(object sender, EventArgs e) {

		}

		private void textBox1_TextChanged(object sender, EventArgs e) {

		}

		private void textBox2_TextChanged(object sender, EventArgs e) {

		}

		private void buttonServerStartClick(object sender, EventArgs e) {
			if (parser.serverIsActive) {
				//STOPPING SERVER
				//bisogna fermare il server
				//parser.Server.Close();
				parser.DeactivateServer();

				//buttonServerStart.Text = "START";
			} else {
				//STARTING SERVER
				try {
					//richTextConsole.AppendText(String.Format("Server Started on port {0} at IP {1}\n", parser.Port, parser.LocalAddr));		
					//Invoke(/*delegato*/);
					parser.ActivateServer(Int32.Parse(textBoxPort.Text), String.Format("{0}.{1}.{2}.{3}", textBoxIP1.Text, textBoxIP2.Text, textBoxIP3.Text, textBoxIP4.Text));
					//buttonServerStart.Text = "STOP";
				} catch (SocketException exc) {
					richTextConsole.AppendText(String.Format("{0}\n", exc));
				}
			}
		}

		private void Form1_Load(object sender, EventArgs e) {

		}
		
		public delegate void printToServerConsoleDelegate(string s);

		public void printToServerConsoleProtected(string s) {
			if (this.richTextConsole.InvokeRequired) {
				Invoke(new printToServerConsoleDelegate(printToServerConsoleProtected), new object[] { s });
			} else {
				richTextConsole.AppendText(s);
				if (checkBoxConsoleAutoFlow.Checked) {
					richTextConsole.SelectionStart = richTextConsole.Text.Length;
					richTextConsole.ScrollToCaret();
				}
			}
		}

		public delegate void setButtonServerStartDelegate(bool b);

		public void setButtonServerStartProtected(bool b) {
			if (this.buttonServerStart.InvokeRequired) {
				Invoke(new setButtonServerStartDelegate(setButtonServerStartProtected), new object[] { b });
			} else {
				if(b) {
					buttonServerStart.Text = "STOP";
				} else {
					buttonServerStart.Text = "START";
				}
				
			}
		}
	}
}
