namespace unaltrotentativo
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.angoli = new System.Windows.Forms.Button();
            this.roll = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.smooth = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(12, 12);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(524, 410);
            this.zedGraphControl1.TabIndex = 0;
            // 
            // angoli
            // 
            this.angoli.Location = new System.Drawing.Point(827, 12);
            this.angoli.Name = "angoli";
            this.angoli.Size = new System.Drawing.Size(190, 33);
            this.angoli.TabIndex = 1;
            this.angoli.Text = "Angoli Di Eulero";
            this.angoli.UseVisualStyleBackColor = true;
            this.angoli.Click += new System.EventHandler(this.button1_Click);
            // 
            // roll
            // 
            this.roll.Location = new System.Drawing.Point(827, 51);
            this.roll.Name = "roll";
            this.roll.Size = new System.Drawing.Size(61, 34);
            this.roll.TabIndex = 2;
            this.roll.Text = "Roll";
            this.roll.UseVisualStyleBackColor = true;
            this.roll.UseWaitCursor = true;
            this.roll.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(894, 51);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(59, 33);
            this.button3.TabIndex = 3;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(959, 51);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(58, 33);
            this.button4.TabIndex = 4;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(0, 0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 5;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // smooth
            // 
            this.smooth.Location = new System.Drawing.Point(1102, 12);
            this.smooth.Name = "smooth";
            this.smooth.Size = new System.Drawing.Size(118, 72);
            this.smooth.TabIndex = 6;
            this.smooth.Text = "button6";
            this.smooth.UseVisualStyleBackColor = true;
            this.smooth.Click += new System.EventHandler(this.smooth_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1337, 561);
            this.Controls.Add(this.smooth);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.roll);
            this.Controls.Add(this.angoli);
            this.Controls.Add(this.zedGraphControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.Button angoli;
        private System.Windows.Forms.Button roll;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button smooth;
    }
}

