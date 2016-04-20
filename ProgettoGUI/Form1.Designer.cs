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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.checkBoxSaveCsv = new System.Windows.Forms.CheckBox();
			this.label14 = new System.Windows.Forms.Label();
			this.numericUpDownClientsAmount = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.buttonClearConsole = new System.Windows.Forms.Button();
			this.numericUpDownFinestra = new System.Windows.Forms.NumericUpDown();
			this.buttonSelectFolder = new System.Windows.Forms.Button();
			this.textBoxCSVPath = new System.Windows.Forms.TextBox();
			this.checkBoxConsoleAutoFlow = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.numericUpDownSmoothing = new System.Windows.Forms.NumericUpDown();
			this.checkBoxPlotDomain = new System.Windows.Forms.CheckBox();
			this.checkBoxSegmentation = new System.Windows.Forms.CheckBox();
			this.checkBoxSmoothing = new System.Windows.Forms.CheckBox();
			this.comboBoxNumSensore = new System.Windows.Forms.ComboBox();
			this.comboBoxTipoSensore = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
			this.label11 = new System.Windows.Forms.Label();
			this.comboBoxChart = new System.Windows.Forms.ComboBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownClientsAmount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownFinestra)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSmoothing)).BeginInit();
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
			this.splitContainer1.Panel1.Controls.Add(this.checkBoxSaveCsv);
			this.splitContainer1.Panel1.Controls.Add(this.label14);
			this.splitContainer1.Panel1.Controls.Add(this.numericUpDownClientsAmount);
			this.splitContainer1.Panel1.Controls.Add(this.label10);
			this.splitContainer1.Panel1.Controls.Add(this.buttonClearConsole);
			this.splitContainer1.Panel1.Controls.Add(this.numericUpDownFinestra);
			this.splitContainer1.Panel1.Controls.Add(this.buttonSelectFolder);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxCSVPath);
			this.splitContainer1.Panel1.Controls.Add(this.checkBoxConsoleAutoFlow);
			this.splitContainer1.Panel1.Controls.Add(this.label6);
			this.splitContainer1.Panel1.Controls.Add(this.label9);
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
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(1006, 564);
			this.splitContainer1.SplitterDistance = 263;
			this.splitContainer1.TabIndex = 0;
			// 
			// checkBoxSaveCsv
			// 
			this.checkBoxSaveCsv.AutoSize = true;
			this.checkBoxSaveCsv.Location = new System.Drawing.Point(7, 197);
			this.checkBoxSaveCsv.Name = "checkBoxSaveCsv";
			this.checkBoxSaveCsv.Size = new System.Drawing.Size(15, 14);
			this.checkBoxSaveCsv.TabIndex = 28;
			this.checkBoxSaveCsv.UseVisualStyleBackColor = true;
			this.checkBoxSaveCsv.CheckedChanged += new System.EventHandler(this.checkBoxSaveCsv_CheckedChanged);
			this.checkBoxSaveCsv.MouseEnter += new System.EventHandler(this.checkBoxSaveCsv_MouseEnter);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(122, 167);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(46, 13);
			this.label14.TabIndex = 27;
			this.label14.Text = "subjects";
			// 
			// numericUpDownClientsAmount
			// 
			this.numericUpDownClientsAmount.Location = new System.Drawing.Point(70, 164);
			this.numericUpDownClientsAmount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownClientsAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownClientsAmount.Name = "numericUpDownClientsAmount";
			this.numericUpDownClientsAmount.Size = new System.Drawing.Size(41, 20);
			this.numericUpDownClientsAmount.TabIndex = 26;
			this.numericUpDownClientsAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownClientsAmount.ValueChanged += new System.EventHandler(this.numericUpDownClientsAmount_ValueChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(12, 167);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(44, 13);
			this.label10.TabIndex = 25;
			this.label10.Text = "Analyze";
			// 
			// buttonClearConsole
			// 
			this.buttonClearConsole.Location = new System.Drawing.Point(161, 532);
			this.buttonClearConsole.Name = "buttonClearConsole";
			this.buttonClearConsole.Size = new System.Drawing.Size(75, 23);
			this.buttonClearConsole.TabIndex = 24;
			this.buttonClearConsole.Text = "CLEAR";
			this.buttonClearConsole.UseVisualStyleBackColor = true;
			this.buttonClearConsole.Click += new System.EventHandler(this.buttonClearConsole_Click);
			// 
			// numericUpDownFinestra
			// 
			this.numericUpDownFinestra.Location = new System.Drawing.Point(70, 134);
			this.numericUpDownFinestra.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.numericUpDownFinestra.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownFinestra.Name = "numericUpDownFinestra";
			this.numericUpDownFinestra.Size = new System.Drawing.Size(41, 20);
			this.numericUpDownFinestra.TabIndex = 23;
			this.numericUpDownFinestra.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownFinestra.ValueChanged += new System.EventHandler(this.numericUpDownFinestra_ValueChanged);
			// 
			// buttonSelectFolder
			// 
			this.buttonSelectFolder.Location = new System.Drawing.Point(236, 193);
			this.buttonSelectFolder.Name = "buttonSelectFolder";
			this.buttonSelectFolder.Size = new System.Drawing.Size(24, 22);
			this.buttonSelectFolder.TabIndex = 22;
			this.buttonSelectFolder.Text = "...";
			this.buttonSelectFolder.UseVisualStyleBackColor = true;
			this.buttonSelectFolder.Click += new System.EventHandler(this.buttonSelectFolder_Click);
			// 
			// textBoxCSVPath
			// 
			this.textBoxCSVPath.Location = new System.Drawing.Point(24, 194);
			this.textBoxCSVPath.Name = "textBoxCSVPath";
			this.textBoxCSVPath.Size = new System.Drawing.Size(212, 20);
			this.textBoxCSVPath.TabIndex = 21;
			this.textBoxCSVPath.Text = "CSV Location";
			// 
			// checkBoxConsoleAutoFlow
			// 
			this.checkBoxConsoleAutoFlow.AutoSize = true;
			this.checkBoxConsoleAutoFlow.Checked = true;
			this.checkBoxConsoleAutoFlow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxConsoleAutoFlow.Location = new System.Drawing.Point(24, 536);
			this.checkBoxConsoleAutoFlow.Name = "checkBoxConsoleAutoFlow";
			this.checkBoxConsoleAutoFlow.Size = new System.Drawing.Size(118, 17);
			this.checkBoxConsoleAutoFlow.TabIndex = 20;
			this.checkBoxConsoleAutoFlow.Text = "Console Auto Scroll";
			this.checkBoxConsoleAutoFlow.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(122, 107);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(20, 13);
			this.label6.TabIndex = 19;
			this.label6.Text = "Hz";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(122, 137);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(47, 13);
			this.label9.TabIndex = 18;
			this.label9.Text = "seconds";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(10, 137);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(46, 13);
			this.label8.TabIndex = 16;
			this.label8.Text = "Window";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(10, 107);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(58, 13);
			this.label7.TabIndex = 15;
			this.label7.Text = "Frequence";
			// 
			// comboBoxFrequenza
			// 
			this.comboBoxFrequenza.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFrequenza.FormattingEnabled = true;
			this.comboBoxFrequenza.Items.AddRange(new object[] {
            "50",
            "100",
            "200"});
			this.comboBoxFrequenza.Location = new System.Drawing.Point(70, 104);
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
			this.buttonServerStart.Location = new System.Drawing.Point(75, 224);
			this.buttonServerStart.Name = "buttonServerStart";
			this.buttonServerStart.Size = new System.Drawing.Size(114, 23);
			this.buttonServerStart.TabIndex = 12;
			this.buttonServerStart.Text = "START";
			this.buttonServerStart.UseVisualStyleBackColor = true;
			this.buttonServerStart.Click += new System.EventHandler(this.buttonServerStartClick);
			// 
			// textBoxIP4
			// 
			this.textBoxIP4.Location = new System.Drawing.Point(211, 74);
			this.textBoxIP4.Name = "textBoxIP4";
			this.textBoxIP4.Size = new System.Drawing.Size(25, 20);
			this.textBoxIP4.TabIndex = 11;
			this.textBoxIP4.Text = "1";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(195, 81);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(10, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = ".";
			// 
			// textBoxIP3
			// 
			this.textBoxIP3.Location = new System.Drawing.Point(164, 74);
			this.textBoxIP3.Name = "textBoxIP3";
			this.textBoxIP3.Size = new System.Drawing.Size(25, 20);
			this.textBoxIP3.TabIndex = 9;
			this.textBoxIP3.Text = "0";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(148, 81);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(10, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = ".";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(101, 81);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(10, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = ".";
			// 
			// textBoxIP2
			// 
			this.textBoxIP2.Location = new System.Drawing.Point(117, 74);
			this.textBoxIP2.Name = "textBoxIP2";
			this.textBoxIP2.Size = new System.Drawing.Size(25, 20);
			this.textBoxIP2.TabIndex = 6;
			this.textBoxIP2.Text = "0";
			// 
			// textBoxIP1
			// 
			this.textBoxIP1.Location = new System.Drawing.Point(70, 74);
			this.textBoxIP1.Name = "textBoxIP1";
			this.textBoxIP1.Size = new System.Drawing.Size(25, 20);
			this.textBoxIP1.TabIndex = 5;
			this.textBoxIP1.Text = "127";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "IP";
			// 
			// textBoxPort
			// 
			this.textBoxPort.Location = new System.Drawing.Point(70, 44);
			this.textBoxPort.Name = "textBoxPort";
			this.textBoxPort.Size = new System.Drawing.Size(41, 20);
			this.textBoxPort.TabIndex = 3;
			this.textBoxPort.Text = "45555";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 47);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Port";
			// 
			// richTextConsole
			// 
			this.richTextConsole.BackColor = System.Drawing.Color.Black;
			this.richTextConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextConsole.ForeColor = System.Drawing.Color.Lime;
			this.richTextConsole.Location = new System.Drawing.Point(3, 253);
			this.richTextConsole.Name = "richTextConsole";
			this.richTextConsole.ReadOnly = true;
			this.richTextConsole.Size = new System.Drawing.Size(266, 277);
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
			this.tabPage1.Controls.Add(this.numericUpDownSmoothing);
			this.tabPage1.Controls.Add(this.checkBoxPlotDomain);
			this.tabPage1.Controls.Add(this.checkBoxSegmentation);
			this.tabPage1.Controls.Add(this.checkBoxSmoothing);
			this.tabPage1.Controls.Add(this.comboBoxNumSensore);
			this.tabPage1.Controls.Add(this.comboBoxTipoSensore);
			this.tabPage1.Controls.Add(this.label13);
			this.tabPage1.Controls.Add(this.label12);
			this.tabPage1.Controls.Add(this.zedGraphControl1);
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this.comboBoxChart);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(724, 532);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Chart";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// numericUpDownSmoothing
			// 
			this.numericUpDownSmoothing.Location = new System.Drawing.Point(398, 508);
			this.numericUpDownSmoothing.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownSmoothing.Name = "numericUpDownSmoothing";
			this.numericUpDownSmoothing.Size = new System.Drawing.Size(35, 20);
			this.numericUpDownSmoothing.TabIndex = 11;
			this.numericUpDownSmoothing.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownSmoothing.ValueChanged += new System.EventHandler(this.numericUpDownSmoothing_ValueChanged);
			// 
			// checkBoxPlotDomain
			// 
			this.checkBoxPlotDomain.AutoSize = true;
			this.checkBoxPlotDomain.Location = new System.Drawing.Point(92, 509);
			this.checkBoxPlotDomain.Name = "checkBoxPlotDomain";
			this.checkBoxPlotDomain.Size = new System.Drawing.Size(124, 17);
			this.checkBoxPlotDomain.TabIndex = 10;
			this.checkBoxPlotDomain.Text = "Plot only last window";
			this.checkBoxPlotDomain.UseVisualStyleBackColor = true;
			this.checkBoxPlotDomain.CheckedChanged += new System.EventHandler(this.checkBoxPlotDomain_CheckedChanged);
			// 
			// checkBoxSegmentation
			// 
			this.checkBoxSegmentation.AutoSize = true;
			this.checkBoxSegmentation.Location = new System.Drawing.Point(536, 509);
			this.checkBoxSegmentation.Name = "checkBoxSegmentation";
			this.checkBoxSegmentation.Size = new System.Drawing.Size(91, 17);
			this.checkBoxSegmentation.TabIndex = 8;
			this.checkBoxSegmentation.Text = "Segmentation";
			this.checkBoxSegmentation.UseVisualStyleBackColor = true;
			this.checkBoxSegmentation.CheckedChanged += new System.EventHandler(this.checkBoxSegmentation_CheckedChanged);
			// 
			// checkBoxSmoothing
			// 
			this.checkBoxSmoothing.AutoSize = true;
			this.checkBoxSmoothing.Checked = true;
			this.checkBoxSmoothing.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxSmoothing.Location = new System.Drawing.Point(320, 509);
			this.checkBoxSmoothing.Name = "checkBoxSmoothing";
			this.checkBoxSmoothing.Size = new System.Drawing.Size(76, 17);
			this.checkBoxSmoothing.TabIndex = 7;
			this.checkBoxSmoothing.Text = "Smoothing";
			this.checkBoxSmoothing.UseVisualStyleBackColor = true;
			this.checkBoxSmoothing.CheckedChanged += new System.EventHandler(this.checkBoxSmoothing_CheckedChanged);
			// 
			// comboBoxNumSensore
			// 
			this.comboBoxNumSensore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNumSensore.FormattingEnabled = true;
			this.comboBoxNumSensore.Items.AddRange(new object[] {
            "1 (Bacino)",
            "2 (Polso Dx)",
            "3 (Polso Sx)",
            "4 (Caviglia Dx)",
            "5 (Caviglia Sx)"});
			this.comboBoxNumSensore.Location = new System.Drawing.Point(558, 11);
			this.comboBoxNumSensore.Name = "comboBoxNumSensore";
			this.comboBoxNumSensore.Size = new System.Drawing.Size(120, 21);
			this.comboBoxNumSensore.TabIndex = 6;
			this.comboBoxNumSensore.SelectedIndexChanged += new System.EventHandler(this.comboBoxNumSensore_SelectedIndexChanged);
			// 
			// comboBoxTipoSensore
			// 
			this.comboBoxTipoSensore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTipoSensore.FormattingEnabled = true;
			this.comboBoxTipoSensore.Items.AddRange(new object[] {
            "Acc",
            "Gyr",
            "Mag",
            "Qua"});
			this.comboBoxTipoSensore.Location = new System.Drawing.Point(305, 11);
			this.comboBoxTipoSensore.Name = "comboBoxTipoSensore";
			this.comboBoxTipoSensore.Size = new System.Drawing.Size(120, 21);
			this.comboBoxTipoSensore.TabIndex = 5;
			this.comboBoxTipoSensore.SelectedIndexChanged += new System.EventHandler(this.comboBoxTipoSensore_SelectedIndexChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(472, 14);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(80, 13);
			this.label13.TabIndex = 4;
			this.label13.Text = "Sensor Position";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(232, 14);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(67, 13);
			this.label12.TabIndex = 3;
			this.label12.Text = "Sensor Type";
			// 
			// zedGraphControl1
			// 
			this.zedGraphControl1.Location = new System.Drawing.Point(6, 38);
			this.zedGraphControl1.Margin = new System.Windows.Forms.Padding(6);
			this.zedGraphControl1.Name = "zedGraphControl1";
			this.zedGraphControl1.ScrollGrace = 0D;
			this.zedGraphControl1.ScrollMaxX = 0D;
			this.zedGraphControl1.ScrollMaxY = 0D;
			this.zedGraphControl1.ScrollMaxY2 = 0D;
			this.zedGraphControl1.ScrollMinX = 0D;
			this.zedGraphControl1.ScrollMinY = 0D;
			this.zedGraphControl1.ScrollMinY2 = 0D;
			this.zedGraphControl1.Size = new System.Drawing.Size(712, 465);
			this.zedGraphControl1.TabIndex = 2;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(35, 14);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(32, 13);
			this.label11.TabIndex = 1;
			this.label11.Text = "Chart";
			// 
			// comboBoxChart
			// 
			this.comboBoxChart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChart.FormattingEnabled = true;
			this.comboBoxChart.Items.AddRange(new object[] {
            "Modulo",
            "Derivata",
            "Eulero",
            "Deviazione",
            "ArcTan(magnY/magnZ)",
            "Dead Reckoning"});
			this.comboBoxChart.Location = new System.Drawing.Point(73, 11);
			this.comboBoxChart.Name = "comboBoxChart";
			this.comboBoxChart.Size = new System.Drawing.Size(120, 21);
			this.comboBoxChart.TabIndex = 0;
			this.comboBoxChart.SelectedIndexChanged += new System.EventHandler(this.comboBoxChart_SelectedIndexChanged);
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(724, 532);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Help";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(1006, 564);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Sense";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownClientsAmount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownFinestra)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSmoothing)).EndInit();
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
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkBoxConsoleAutoFlow;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private ZedGraph.ZedGraphControl zedGraphControl1;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox comboBoxChart;
		private System.Windows.Forms.ComboBox comboBoxNumSensore;
		private System.Windows.Forms.ComboBox comboBoxTipoSensore;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Button buttonSelectFolder;
		private System.Windows.Forms.TextBox textBoxCSVPath;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.NumericUpDown numericUpDownFinestra;
		private System.Windows.Forms.Button buttonClearConsole;
		private System.Windows.Forms.CheckBox checkBoxSegmentation;
		private System.Windows.Forms.CheckBox checkBoxSmoothing;
		private System.Windows.Forms.CheckBox checkBoxPlotDomain;
		private System.Windows.Forms.NumericUpDown numericUpDownSmoothing;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown numericUpDownClientsAmount;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkBoxSaveCsv;
	}
}

