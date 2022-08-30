namespace attplan
{
    partial class MainWindow
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTrajectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLoadEphemeris = new System.Windows.Forms.Button();
            this.lbEphemeris = new System.Windows.Forms.Label();
            this.btnLoadSchedule = new System.Windows.Forms.Button();
            this.lbSchedule = new System.Windows.Forms.Label();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlTrackbar = new System.Windows.Forms.Panel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.tbCurrentTime = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbBioSentinel = new System.Windows.Forms.RadioButton();
            this.rbEarth = new System.Windows.Forms.RadioButton();
            this.rbMoon = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRange = new System.Windows.Forms.Button();
            this.btnStationAzEl = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbEphemeris = new System.Windows.Forms.RadioButton();
            this.rbPass = new System.Windows.Forms.RadioButton();
            this.btnOffBoresight = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbPassCount = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.lbIntervalStart = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbIntervalStop = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cbModel = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlTrackbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2076, 40);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadTrajectoryToolStripMenuItem,
            this.loadScheduleToolStripMenuItem,
            this.toolStripSeparator1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 36);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadTrajectoryToolStripMenuItem
            // 
            this.loadTrajectoryToolStripMenuItem.Name = "loadTrajectoryToolStripMenuItem";
            this.loadTrajectoryToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.loadTrajectoryToolStripMenuItem.Text = "Load &Trajectory";
            // 
            // loadScheduleToolStripMenuItem
            // 
            this.loadScheduleToolStripMenuItem.Name = "loadScheduleToolStripMenuItem";
            this.loadScheduleToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.loadScheduleToolStripMenuItem.Text = "Load &Schedule";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(356, 6);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbSchedule);
            this.panel1.Controls.Add(this.lbEphemeris);
            this.panel1.Controls.Add(this.btnLoadSchedule);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnLoadEphemeris);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2076, 90);
            this.panel1.TabIndex = 1;
            // 
            // btnLoadEphemeris
            // 
            this.btnLoadEphemeris.Location = new System.Drawing.Point(3, 3);
            this.btnLoadEphemeris.Name = "btnLoadEphemeris";
            this.btnLoadEphemeris.Size = new System.Drawing.Size(233, 35);
            this.btnLoadEphemeris.TabIndex = 0;
            this.btnLoadEphemeris.Text = "Load Ephemeris";
            this.btnLoadEphemeris.UseVisualStyleBackColor = true;
            // 
            // lbEphemeris
            // 
            this.lbEphemeris.BackColor = System.Drawing.Color.White;
            this.lbEphemeris.Location = new System.Drawing.Point(242, 3);
            this.lbEphemeris.Name = "lbEphemeris";
            this.lbEphemeris.Size = new System.Drawing.Size(859, 35);
            this.lbEphemeris.TabIndex = 1;
            this.lbEphemeris.Text = "<ehemeris>";
            this.lbEphemeris.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLoadSchedule
            // 
            this.btnLoadSchedule.Location = new System.Drawing.Point(3, 44);
            this.btnLoadSchedule.Name = "btnLoadSchedule";
            this.btnLoadSchedule.Size = new System.Drawing.Size(233, 35);
            this.btnLoadSchedule.TabIndex = 0;
            this.btnLoadSchedule.Text = "Load DSN Schedule";
            this.btnLoadSchedule.UseVisualStyleBackColor = true;
            // 
            // lbSchedule
            // 
            this.lbSchedule.BackColor = System.Drawing.Color.White;
            this.lbSchedule.Location = new System.Drawing.Point(242, 44);
            this.lbSchedule.Name = "lbSchedule";
            this.lbSchedule.Size = new System.Drawing.Size(859, 35);
            this.lbSchedule.TabIndex = 1;
            this.lbSchedule.Text = "<schedule>";
            this.lbSchedule.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(119, 36);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // pnlTrackbar
            // 
            this.pnlTrackbar.Controls.Add(this.trackBar1);
            this.pnlTrackbar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTrackbar.Location = new System.Drawing.Point(0, 1170);
            this.pnlTrackbar.Name = "pnlTrackbar";
            this.pnlTrackbar.Size = new System.Drawing.Size(2076, 51);
            this.pnlTrackbar.TabIndex = 2;
            // 
            // trackBar1
            // 
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar1.Location = new System.Drawing.Point(0, 0);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(2076, 51);
            this.trackBar1.TabIndex = 0;
            // 
            // tbCurrentTime
            // 
            this.tbCurrentTime.Location = new System.Drawing.Point(11, 124);
            this.tbCurrentTime.Name = "tbCurrentTime";
            this.tbCurrentTime.Size = new System.Drawing.Size(271, 31);
            this.tbCurrentTime.TabIndex = 1;
            this.tbCurrentTime.TextChanged += new System.EventHandler(this.tbCurrentTime_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox5);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox4);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 130);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(288, 1040);
            this.panel2.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbModel);
            this.groupBox1.Controls.Add(this.rbMoon);
            this.groupBox1.Controls.Add(this.rbEarth);
            this.groupBox1.Controls.Add(this.rbBioSentinel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 148);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Look At";
            // 
            // rbBioSentinel
            // 
            this.rbBioSentinel.AutoSize = true;
            this.rbBioSentinel.Checked = true;
            this.rbBioSentinel.Location = new System.Drawing.Point(12, 30);
            this.rbBioSentinel.Name = "rbBioSentinel";
            this.rbBioSentinel.Size = new System.Drawing.Size(152, 29);
            this.rbBioSentinel.TabIndex = 0;
            this.rbBioSentinel.TabStop = true;
            this.rbBioSentinel.Text = "BioSentinel";
            this.rbBioSentinel.UseVisualStyleBackColor = true;
            this.rbBioSentinel.CheckedChanged += new System.EventHandler(this.RbBioSentinel_CheckedChanged);
            // 
            // rbEarth
            // 
            this.rbEarth.AutoSize = true;
            this.rbEarth.Location = new System.Drawing.Point(12, 65);
            this.rbEarth.Name = "rbEarth";
            this.rbEarth.Size = new System.Drawing.Size(94, 29);
            this.rbEarth.TabIndex = 0;
            this.rbEarth.Text = "Earth";
            this.rbEarth.UseVisualStyleBackColor = true;
            this.rbEarth.CheckedChanged += new System.EventHandler(this.RbEarth_CheckedChanged);
            // 
            // rbMoon
            // 
            this.rbMoon.AutoSize = true;
            this.rbMoon.Location = new System.Drawing.Point(12, 100);
            this.rbMoon.Name = "rbMoon";
            this.rbMoon.Size = new System.Drawing.Size(97, 29);
            this.rbMoon.TabIndex = 0;
            this.rbMoon.Text = "Moon";
            this.rbMoon.UseVisualStyleBackColor = true;
            this.rbMoon.CheckedChanged += new System.EventHandler(this.RbMoon_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnOffBoresight);
            this.groupBox2.Controls.Add(this.btnStationAzEl);
            this.groupBox2.Controls.Add(this.btnRange);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 364);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(288, 177);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Plot";
            // 
            // btnRange
            // 
            this.btnRange.Location = new System.Drawing.Point(12, 30);
            this.btnRange.Name = "btnRange";
            this.btnRange.Size = new System.Drawing.Size(94, 40);
            this.btnRange.TabIndex = 0;
            this.btnRange.Text = "Range";
            this.btnRange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRange.UseVisualStyleBackColor = true;
            // 
            // btnStationAzEl
            // 
            this.btnStationAzEl.Location = new System.Drawing.Point(15, 76);
            this.btnStationAzEl.Name = "btnStationAzEl";
            this.btnStationAzEl.Size = new System.Drawing.Size(155, 40);
            this.btnStationAzEl.TabIndex = 0;
            this.btnStationAzEl.Text = "Station Az/El";
            this.btnStationAzEl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStationAzEl.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbPass);
            this.groupBox3.Controls.Add(this.rbEphemeris);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 148);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 111);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Time Interval";
            // 
            // rbEphemeris
            // 
            this.rbEphemeris.AutoSize = true;
            this.rbEphemeris.Checked = true;
            this.rbEphemeris.Location = new System.Drawing.Point(15, 30);
            this.rbEphemeris.Name = "rbEphemeris";
            this.rbEphemeris.Size = new System.Drawing.Size(145, 29);
            this.rbEphemeris.TabIndex = 0;
            this.rbEphemeris.TabStop = true;
            this.rbEphemeris.Text = "Ephemeris";
            this.rbEphemeris.UseVisualStyleBackColor = true;
            // 
            // rbPass
            // 
            this.rbPass.AutoSize = true;
            this.rbPass.Location = new System.Drawing.Point(15, 65);
            this.rbPass.Name = "rbPass";
            this.rbPass.Size = new System.Drawing.Size(91, 29);
            this.rbPass.TabIndex = 0;
            this.rbPass.Text = "Pass";
            this.rbPass.UseVisualStyleBackColor = true;
            // 
            // btnOffBoresight
            // 
            this.btnOffBoresight.Location = new System.Drawing.Point(15, 122);
            this.btnOffBoresight.Name = "btnOffBoresight";
            this.btnOffBoresight.Size = new System.Drawing.Size(155, 40);
            this.btnOffBoresight.TabIndex = 0;
            this.btnOffBoresight.Text = "Off Boresight";
            this.btnOffBoresight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOffBoresight.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.numericUpDown1);
            this.groupBox4.Controls.Add(this.lbPassCount);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(0, 259);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(288, 105);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Pass #";
            // 
            // lbPassCount
            // 
            this.lbPassCount.AutoSize = true;
            this.lbPassCount.Location = new System.Drawing.Point(6, 27);
            this.lbPassCount.Name = "lbPassCount";
            this.lbPassCount.Size = new System.Drawing.Size(141, 25);
            this.lbPassCount.TabIndex = 0;
            this.lbPassCount.Text = "<pass count>";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(15, 55);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(149, 31);
            this.numericUpDown1.TabIndex = 1;
            // 
            // lbIntervalStart
            // 
            this.lbIntervalStart.BackColor = System.Drawing.Color.White;
            this.lbIntervalStart.Location = new System.Drawing.Point(12, 52);
            this.lbIntervalStart.Name = "lbIntervalStart";
            this.lbIntervalStart.Size = new System.Drawing.Size(270, 35);
            this.lbIntervalStart.TabIndex = 1;
            this.lbIntervalStart.Text = "<interval start>";
            this.lbIntervalStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tbCurrentTime);
            this.groupBox5.Controls.Add(this.lbIntervalStop);
            this.groupBox5.Controls.Add(this.lbIntervalStart);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox5.Location = new System.Drawing.Point(0, 541);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(288, 252);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Time";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 25);
            this.label3.TabIndex = 0;
            this.label3.Text = "Start";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 25);
            this.label4.TabIndex = 0;
            this.label4.Text = "Current";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 25);
            this.label5.TabIndex = 0;
            this.label5.Text = "Stop";
            // 
            // lbIntervalStop
            // 
            this.lbIntervalStop.BackColor = System.Drawing.Color.White;
            this.lbIntervalStop.Location = new System.Drawing.Point(12, 183);
            this.lbIntervalStop.Name = "lbIntervalStop";
            this.lbIntervalStop.Size = new System.Drawing.Size(270, 35);
            this.lbIntervalStop.TabIndex = 1;
            this.lbIntervalStop.Text = "<interval stop>";
            this.lbIntervalStop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1189, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(233, 35);
            this.button1.TabIndex = 0;
            this.button1.Text = "Write COMM Attitudes";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cbModel
            // 
            this.cbModel.AutoSize = true;
            this.cbModel.Checked = true;
            this.cbModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbModel.Location = new System.Drawing.Point(170, 30);
            this.cbModel.Name = "cbModel";
            this.cbModel.Size = new System.Drawing.Size(115, 29);
            this.cbModel.TabIndex = 1;
            this.cbModel.Text = "Model?";
            this.cbModel.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2076, 1221);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlTrackbar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.pnlTrackbar.ResumeLayout(false);
            this.pnlTrackbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTrajectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadScheduleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbSchedule;
        private System.Windows.Forms.Label lbEphemeris;
        private System.Windows.Forms.Button btnLoadSchedule;
        private System.Windows.Forms.Button btnLoadEphemeris;
        private System.Windows.Forms.Panel pnlTrackbar;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TextBox tbCurrentTime;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOffBoresight;
        private System.Windows.Forms.Button btnStationAzEl;
        private System.Windows.Forms.Button btnRange;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label lbPassCount;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbPass;
        private System.Windows.Forms.RadioButton rbEphemeris;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbMoon;
        private System.Windows.Forms.RadioButton rbEarth;
        private System.Windows.Forms.RadioButton rbBioSentinel;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lbIntervalStop;
        private System.Windows.Forms.Label lbIntervalStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbModel;
    }
}