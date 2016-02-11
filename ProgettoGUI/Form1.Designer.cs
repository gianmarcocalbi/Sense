namespace ProgettoGUI {
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label6 = new System.Windows.Forms.Label();
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label6);
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
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(1006, 564);
			this.splitContainer1.SplitterDistance = 264;
			this.splitContainer1.TabIndex = 0;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(65, 9);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(135, 25);
			this.label6.TabIndex = 13;
			this.label6.Text = "Server Side";
			// 
			// buttonServerStart
			// 
			this.buttonServerStart.ForeColor = System.Drawing.Color.Black;
			this.buttonServerStart.Location = new System.Drawing.Point(53, 157);
			this.buttonServerStart.Name = "buttonServerStart";
			this.buttonServerStart.Size = new System.Drawing.Size(147, 23);
			this.buttonServerStart.TabIndex = 12;
			this.buttonServerStart.Text = "START";
			this.buttonServerStart.UseVisualStyleBackColor = true;
			this.buttonServerStart.Click += new System.EventHandler(this.buttonServerStartClick);
			// 
			// textBoxIP4
			// 
			this.textBoxIP4.Location = new System.Drawing.Point(206, 118);
			this.textBoxIP4.Name = "textBoxIP4";
			this.textBoxIP4.Size = new System.Drawing.Size(29, 20);
			this.textBoxIP4.TabIndex = 11;
			this.textBoxIP4.Text = "1";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(190, 125);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(10, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = ".";
			// 
			// textBoxIP3
			// 
			this.textBoxIP3.Location = new System.Drawing.Point(155, 118);
			this.textBoxIP3.Name = "textBoxIP3";
			this.textBoxIP3.Size = new System.Drawing.Size(29, 20);
			this.textBoxIP3.TabIndex = 9;
			this.textBoxIP3.Text = "0";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(139, 125);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(10, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = ".";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(84, 125);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(10, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = ".";
			// 
			// textBoxIP2
			// 
			this.textBoxIP2.Location = new System.Drawing.Point(100, 118);
			this.textBoxIP2.Name = "textBoxIP2";
			this.textBoxIP2.Size = new System.Drawing.Size(29, 20);
			this.textBoxIP2.TabIndex = 6;
			this.textBoxIP2.Text = "0";
			// 
			// textBoxIP1
			// 
			this.textBoxIP1.Location = new System.Drawing.Point(46, 118);
			this.textBoxIP1.Name = "textBoxIP1";
			this.textBoxIP1.Size = new System.Drawing.Size(32, 20);
			this.textBoxIP1.TabIndex = 5;
			this.textBoxIP1.Text = "127";
			this.textBoxIP1.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 121);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "IP";
			// 
			// textBoxPort
			// 
			this.textBoxPort.Location = new System.Drawing.Point(46, 94);
			this.textBoxPort.Name = "textBoxPort";
			this.textBoxPort.Size = new System.Drawing.Size(100, 20);
			this.textBoxPort.TabIndex = 3;
			this.textBoxPort.Text = "45555";
			this.textBoxPort.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 97);
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
			this.richTextConsole.Location = new System.Drawing.Point(3, 238);
			this.richTextConsole.Name = "richTextConsole";
			this.richTextConsole.ReadOnly = true;
			this.richTextConsole.Size = new System.Drawing.Size(266, 323);
			this.richTextConsole.TabIndex = 1;
			this.richTextConsole.Text = "";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(3, 3);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(732, 558);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(724, 532);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(724, 532);
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
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox textBoxPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox richTextConsole;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox textBoxIP4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxIP3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxIP2;
		private System.Windows.Forms.TextBox textBoxIP1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonServerStart;
		private System.Windows.Forms.Label label6;
	}
}

