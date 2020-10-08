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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 434);
            this.Controls.Add(this.Sample);
            this.Controls.Add(this.Image);
            this.Controls.Add(this.makeSample);
            this.Controls.Add(this.LoadButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Sample)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Button makeSample;
        private System.Windows.Forms.Button Image;
        private System.Windows.Forms.PictureBox Sample;
    }
}

