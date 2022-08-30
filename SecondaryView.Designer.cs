namespace LadeeViz
{
    partial class SecondaryView
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
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uVSTelescopeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uVSSolarViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Wrapper = new LadeeViz.Viz.OpenGLControlWrapper();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(652, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uVSTelescopeToolStripMenuItem,
            this.uVSSolarViewerToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // uVSTelescopeToolStripMenuItem
            // 
            this.uVSTelescopeToolStripMenuItem.Name = "uVSTelescopeToolStripMenuItem";
            this.uVSTelescopeToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.uVSTelescopeToolStripMenuItem.Text = "UVS Telescope";
            this.uVSTelescopeToolStripMenuItem.Click += new System.EventHandler(this.uVSTelescopeToolStripMenuItem_Click);
            // 
            // uVSSolarViewerToolStripMenuItem
            // 
            this.uVSSolarViewerToolStripMenuItem.Name = "uVSSolarViewerToolStripMenuItem";
            this.uVSSolarViewerToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.uVSSolarViewerToolStripMenuItem.Text = "UVS Solar Viewer";
            this.uVSSolarViewerToolStripMenuItem.Click += new System.EventHandler(this.uVSSolarViewerToolStripMenuItem_Click);
            // 
            // Wrapper
            // 
            this.Wrapper.BackColor = System.Drawing.Color.Black;
            this.Wrapper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Wrapper.Location = new System.Drawing.Point(0, 24);
            this.Wrapper.Name = "Wrapper";
            this.Wrapper.Size = new System.Drawing.Size(652, 690);
            this.Wrapper.TabIndex = 1;
            this.Wrapper.VSync = false;
            this.Wrapper.Load += new System.EventHandler(this.Wrapper_Load);
            this.Wrapper.Paint += new System.Windows.Forms.PaintEventHandler(this.Wrapper_Paint);
            this.Wrapper.MouseClick += new System.Windows.Forms.MouseEventHandler(this.openGLControlWrapper1_MouseClick);
            // 
            // SecondaryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 714);
            this.Controls.Add(this.Wrapper);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SecondaryView";
            this.Text = "SecondaryView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SecondaryView_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uVSTelescopeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uVSSolarViewerToolStripMenuItem;
        public LadeeViz.Viz.OpenGLControlWrapper Wrapper;
    }
}