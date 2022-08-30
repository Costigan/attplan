namespace LadeeViz.Utilities
{
    partial class VectorEditor
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
            this.pgVector = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // pgVector
            // 
            this.pgVector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgVector.Location = new System.Drawing.Point(0, 0);
            this.pgVector.Name = "pgVector";
            this.pgVector.Size = new System.Drawing.Size(160, 277);
            this.pgVector.TabIndex = 0;
            // 
            // VectorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 277);
            this.Controls.Add(this.pgVector);
            this.Name = "VectorEditor";
            this.Text = "VectorEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgVector;
    }
}