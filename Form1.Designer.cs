namespace sample
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
            this.LoadButton = new System.Windows.Forms.Button();
            this.makeSample = new System.Windows.Forms.Button();
            this.Image = new System.Windows.Forms.Button();
            this.Sample = new System.Windows.Forms.PictureBox();
            this.mode1 = new System.Windows.Forms.CheckBox();
            this.Mode2 = new System.Windows.Forms.CheckBox();
            this.Mode3 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Sample)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(12, 12);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(106, 42);
            this.LoadButton.TabIndex = 0;
            this.LoadButton.Text = "Load CSV";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // makeSample
            // 
            this.makeSample.Location = new System.Drawing.Point(147, 12);
            this.makeSample.Name = "makeSample";
            this.makeSample.Size = new System.Drawing.Size(138, 42);
            this.makeSample.TabIndex = 1;
            this.makeSample.Text = "Adjust sample";
            this.makeSample.UseVisualStyleBackColor = true;
            this.makeSample.Click += new System.EventHandler(this.makeSample_Click);
            // 
            // Image
            // 
            this.Image.Location = new System.Drawing.Point(340, 13);
            this.Image.Name = "Image";
            this.Image.Size = new System.Drawing.Size(126, 41);
            this.Image.TabIndex = 2;
            this.Image.Text = "Out Image";
            this.Image.UseVisualStyleBackColor = true;
            this.Image.Click += new System.EventHandler(this.Image_Click);
            // 
            // Sample
            // 
            this.Sample.Location = new System.Drawing.Point(492, 28);
            this.Sample.Name = "Sample";
            this.Sample.Size = new System.Drawing.Size(611, 381);
            this.Sample.TabIndex = 3;
            this.Sample.TabStop = false;
            this.Sample.Click += new System.EventHandler(this.Sample_Click);
            // 
            // mode1
            // 
            this.mode1.AutoSize = true;
            this.mode1.Location = new System.Drawing.Point(3, 411);
            this.mode1.Name = "mode1";
            this.mode1.Size = new System.Drawing.Size(81, 21);
            this.mode1.TabIndex = 4;
            this.mode1.Text = "Mode  1";
            this.mode1.UseVisualStyleBackColor = true;
            this.mode1.CheckedChanged += new System.EventHandler(this.mode1_CheckedChanged);
            // 
            // Mode2
            // 
            this.Mode2.AutoSize = true;
            this.Mode2.Location = new System.Drawing.Point(106, 411);
            this.Mode2.Name = "Mode2";
            this.Mode2.Size = new System.Drawing.Size(73, 21);
            this.Mode2.TabIndex = 5;
            this.Mode2.Text = "Mode2";
            this.Mode2.UseVisualStyleBackColor = true;
            this.Mode2.CheckedChanged += new System.EventHandler(this.Mode2_CheckedChanged);
            // 
            // Mode3
            // 
            this.Mode3.AutoSize = true;
            this.Mode3.Location = new System.Drawing.Point(217, 411);
            this.Mode3.Name = "Mode3";
            this.Mode3.Size = new System.Drawing.Size(73, 21);
            this.Mode3.TabIndex = 6;
            this.Mode3.Text = "Mode3";
            this.Mode3.UseVisualStyleBackColor = true;
            this.Mode3.CheckedChanged += new System.EventHandler(this.Mode3_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 434);
            this.Controls.Add(this.Mode3);
            this.Controls.Add(this.Mode2);
            this.Controls.Add(this.mode1);
            this.Controls.Add(this.Sample);
            this.Controls.Add(this.Image);
            this.Controls.Add(this.makeSample);
            this.Controls.Add(this.LoadButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Sample)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Button makeSample;
        private System.Windows.Forms.Button Image;
        private System.Windows.Forms.PictureBox Sample;
        private System.Windows.Forms.CheckBox mode1;
        private System.Windows.Forms.CheckBox Mode2;
        private System.Windows.Forms.CheckBox Mode3;
    }
}

