namespace Sense {
	partial class Form1 {
		/// <summary>
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Pulire le risorse in uso.
		/// </summary>
		/// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Codice generato da Progettazione Windows Form

		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.checkBoxConsoleAutoFlow = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textBoxFinestra = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.comboBoxFrequenza = new System.Windows.Forms.ComboBox();
			this.labelServerSide = new System.Windows.Forms.Label();
			this.buttonServerStart = new System.Windows.Forms.Button();
			this.textBoxIP4 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxIP3 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxIP2 = new System.Windows.Forms.TextBox();
			this.textBoxIP1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxPort = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.richTextConsole = new System.Windows.Forms.RichTextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
			this.label11 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.checkBoxConsoleAutoFlow);
			this.splitContainer1.Panel1.Controls.Add(this.label6);
			this.splitContainer1.Panel1.Controls.Add(this.label9);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxFinestra);
			this.splitContainer1.Panel1.Controls.Add(this.label8);
			this.splitContainer1.Panel1.Controls.Add(this.label7);
			this.splitContainer1.Panel1.Controls.Add(this.comboBoxFrequenza);
			this.splitContainer1.Panel1.Controls.Add(this.labelServerSide);
			this.splitContainer1.Panel1.Controls.Add(this.buttonServerStart);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxIP4);
			this.splitContainer1.Panel1.Controls.Add(this.label5);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxIP3);
			this.splitContainer1.Panel1.Controls.Add(this.label4);
			this.splitContainer1.Panel1.Controls.Add(this.label3);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxIP2);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxIP1);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxPort);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.richTextConsole);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.label10);
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(1006, 564);
			this.splitContainer1.SplitterDistance = 263;
			this.splitContainer1.TabIndex = 0;
			// 
			// checkBoxConsoleAutoFlow
			// 
			this.checkBoxConsoleAutoFlow.AutoSize = true;
			this.checkBoxConsoleAutoFlow.Checked = true;
			this.checkBoxConsoleAutoFlow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxConsoleAutoFlow.Location = new System.Drawing.Point(63, 540);
			this.checkBoxConsoleAutoFlow.Name = "checkBoxConsoleAutoFlow";
			this.checkBoxConsoleAutoFlow.Size = new System.Drawing.Size(114, 17);
			this.checkBoxConsoleAutoFlow.TabIndex = 20;
			this.checkBoxConsoleAutoFlow.Text = "Console Auto Flow";
			this.checkBoxConsoleAutoFlow.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(122, 106);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(20, 13);
			this.label6.TabIndex = 19;
			this.label6.Text = "Hz";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(123, 134);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(44, 13);
			this.label9.TabIndex = 18;
			this.label9.Text = "secondi";
			// 
			// textBoxFinestra
			// 
			this.textBoxFinestra.Location = new System.Drawing.Point(75, 131);
			this.textBoxFinestra.Name = "textBoxFinestra";
			this.textBoxFinestra.Size = new System.Drawing.Size(41, 20);
			this.textBoxFinestra.TabIndex = 17;
			this.textBoxFinestra.Text = "10";
			this.textBoxFinestra.TextChanged += new System.EventHandler(this.textBoxFinestra_TextChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(13, 134);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(44, 13);
			this.label8.TabIndex = 16;
			this.label8.Text = "Finestra";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(13, 106);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(57, 13);
			this.label7.TabIndex = 15;
			this.label7.Text = "Frequenza";
			// 
			// comboBoxFrequenza
			// 
			this.comboBoxFrequenza.FormattingEnabled = true;
			this.comboBoxFrequenza.Items.AddRange(new object[] {
            "50",
            "100",
            "200"});
			this.comboBoxFrequenza.Location = new System.Drawing.Point(75, 103);
			this.comboBoxFrequenza.Name = "comboBoxFrequenza";
			this.comboBoxFrequenza.Size = new System.Drawing.Size(41, 21);
			this.comboBoxFrequenza.TabIndex = 14;
			this.comboBoxFrequenza.SelectedIndexChanged += new System.EventHandler(this.comboBoxFrequenza_SelectedIndexChanged);
			// 
			// labelServerSide
			// 
			this.labelServerSide.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelServerSide.Location = new System.Drawing.Point(3, 9);
			this.labelServerSide.Name = "labelServerSide";
			this.labelServerSide.Size = new System.Drawing.Size(257, 25);
			this.labelServerSide.TabIndex = 13;
			this.labelServerSide.Text = "Server Side";
			this.labelServerSide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonServerStart
			// 
			this.buttonServerStart.ForeColor = System.Drawing.Color.Black;
			this.buttonServerStart.Location = new System.Drawing.Point(53, 166);
			this.buttonServerStart.Name = "buttonServerStart";
			this.buttonServerStart.Size = new System.Drawing.Size(147, 23);
			this.buttonServerStart.TabIndex = 12;
			this.buttonServerStart.Text = "START";
			this.buttonServerStart.UseVisualStyleBackColor = true;
			this.buttonServerStart.Click += new System.EventHandler(this.buttonServerStartClick);
			// 
			// textBoxIP4
			// 
			this.textBoxIP4.Location = new System.Drawing.Point(199, 74);
			this.textBoxIP4.Name = "textBoxIP4";
			this.textBoxIP4.Size = new System.Drawing.Size(25, 20);
			this.textBoxIP4.TabIndex = 11;
			this.textBoxIP4.Text = "1";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(183, 81);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(10, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = ".";
			// 
			// textBoxIP3
			// 
			this.textBoxIP3.Location = new System.Drawing.Point(152, 74);
			this.textBoxIP3.Name = "textBoxIP3";
			this.textBoxIP3.Size = new System.Drawing.Size(25, 20);
			this.textBoxIP3.TabIndex = 9;
			this.textBoxIP3.Text = "0";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(136, 81);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(10, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = ".";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(89, 81);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(10, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = ".";
			// 
			// textBoxIP2
			// 
			this.textBoxIP2.Location = new System.Drawing.Point(105, 74);
			this.textBoxIP2.Name = "textBoxIP2";
			this.textBoxIP2.Size = new System.Drawing.Size(25, 20);
			this.textBoxIP2.TabIndex = 6;
			this.textBoxIP2.Text = "0";
			// 
			// textBoxIP1
			// 
			this.textBoxIP1.Location = new System.Drawing.Point(58, 74);
			this.textBoxIP1.Name = "textBoxIP1";
			this.textBoxIP1.Size = new System.Drawing.Size(25, 20);
			this.textBoxIP1.TabIndex = 5;
			this.textBoxIP1.Text = "127";
			this.textBoxIP1.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "IP";
			// 
			// textBoxPort
			// 
			this.textBoxPort.Location = new System.Drawing.Point(58, 44);
			this.textBoxPort.Name = "textBoxPort";
			this.textBoxPort.Size = new System.Drawing.Size(100, 20);
			this.textBoxPort.TabIndex = 3;
			this.textBoxPort.Text = "45555";
			this.textBoxPort.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 47);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Porta";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// richTextConsole
			// 
			this.richTextConsole.BackColor = System.Drawing.Color.Black;
			this.richTextConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextConsole.ForeColor = System.Drawing.Color.Lime;
			this.richTextConsole.Location = new System.Drawing.Point(3, 209);
			this.richTextConsole.Name = "richTextConsole";
			this.richTextConsole.ReadOnly = true;
			this.richTextConsole.Size = new System.Drawing.Size(266, 325);
			this.richTextConsole.TabIndex = 1;
			this.richTextConsole.Text = "";
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(7, 5);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(724, 32);
			this.label10.TabIndex = 14;
			this.label10.Text = "Chart";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(3, 44);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(732, 517);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.comboBox3);
			this.tabPage1.Controls.Add(this.comboBox2);
			this.tabPage1.Controls.Add(this.label13);
			this.tabPage1.Controls.Add(this.label12);
			this.tabPage1.Controls.Add(this.zedGraphControl1);
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this.comboBox1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(724, 491);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Chart";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// comboBox3
			// 
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
			this.comboBox3.Location = new System.Drawing.Point(326, 10);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(31, 21);
			this.comboBox3.TabIndex = 6;
			// 
			// comboBox2
			// 
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
            "Acc",
            "Gyr",
            "Mag",
            "Qua"});
			this.comboBox2.Location = new System.Drawing.Point(178, 11);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(48, 21);
			this.comboBox2.TabIndex = 5;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(249, 14);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(71, 13);
			this.label13.TabIndex = 4;
			this.label13.Text = "Num Sensore";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(126, 15);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(46, 13);
			this.label12.TabIndex = 3;
			this.label12.Text = "Sensore";
			// 
			// zedGraphControl1
			// 
			this.zedGraphControl1.Location = new System.Drawing.Point(6, 40);
			this.zedGraphControl1.Name = "zedGraphControl1";
			this.zedGraphControl1.ScrollGrace = 0D;
			this.zedGraphControl1.ScrollMaxX = 0D;
			this.zedGraphControl1.ScrollMaxY = 0D;
			this.zedGraphControl1.ScrollMaxY2 = 0D;
			this.zedGraphControl1.ScrollMinX = 0D;
			this.zedGraphControl1.ScrollMinY = 0D;
			this.zedGraphControl1.ScrollMinY2 = 0D;
			this.zedGraphControl1.Size = new System.Drawing.Size(712, 445);
			this.zedGraphControl1.TabIndex = 2;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(20, 15);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(32, 13);
			this.label11.TabIndex = 1;
			this.label11.Text = "Chart";
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
			this.comboBox1.Location = new System.Drawing.Point(58, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(47, 21);
			this.comboBox1.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(724, 491);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1006, 564);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Sense";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox textBoxPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox richTextConsole;
		private System.Windows.Forms.TextBox textBoxIP4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxIP3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxIP2;
		private System.Windows.Forms.TextBox textBoxIP1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonServerStart;
		private System.Windows.Forms.Label labelServerSide;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboBoxFrequenza;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textBoxFinestra;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkBoxConsoleAutoFlow;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label10;
		private ZedGraph.ZedGraphControl zedGraphControl1;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.ComboBox comboBox3;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label12;
	}
}

