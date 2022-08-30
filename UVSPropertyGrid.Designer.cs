namespace LadeeViz
{
    partial class UVSPropertyGrid
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
            this.pgUVS = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // pgUVS
            // 
            this.pgUVS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgUVS.Location = new System.Drawing.Point(0, 0);
            this.pgUVS.Name = "pgUVS";
            this.pgUVS.Size = new System.Drawing.Size(385, 757);
            this.pgUVS.TabIndex = 0;
            // 
            // UVSPropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 757);
            this.Controls.Add(this.pgUVS);
            this.Name = "UVSPropertyGrid";
            this.Text = "UVSPropertyGrid";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgUVS;
    }
}