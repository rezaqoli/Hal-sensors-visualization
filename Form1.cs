using NumSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace sample
{
    public partial class Form1 : Form
    {
        public TrackBar LowerRange;
        public TrackBar UpperRange;
        public MenuStrip strip1;
        public MenuStrip strip2;
        public TextBox texts;
        public Label upLabel;
        public Label lowLabel;

        //public int max { get; private set; }
        List<int[]> csvList;
        int max = 2000;
        int min = 0;
        int W = 2000;
        int H = 80;
        Image<Bgr, byte> output;
        bool load = false;
        bool mode_1 = false;
        bool mode_2 = false;
        bool mode_3 = false;

        int upper_range;
        int lower_range;
        Image<Bgr, byte> imgMask;
        Image<Bgr, byte> imgMask2;
        Image<Bgr, byte> imgMask3;
        Bitmap bMap;

        public Form1()
        {
            InitializeComponent();
            UpperRange = new System.Windows.Forms.TrackBar();
            LowerRange = new System.Windows.Forms.TrackBar();
            strip1 = new System.Windows.Forms.MenuStrip();
            strip2 = new System.Windows.Forms.MenuStrip();
            this.upLabel = new Label(); this.lowLabel = new Label(); lowLabel.Location= new Point(215, 240); upLabel.Location = new Point(215, 80);
            lowLabel.AutoSize = true; upLabel.AutoSize = true; upLabel.Size = new Size(48, 55); lowLabel.Size = new Size(48, 55);
            this.texts = new TextBox(); this.texts.Location = new System.Drawing.Point(12, 280); this.texts.Size = new System.Drawing.Size(400, 100);
            this.texts.Text = "Empty";
            ((System.ComponentModel.ISupportInitialize)(UpperRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(LowerRange)).BeginInit();
            strip2.SuspendLayout();
            SuspendLayout();
            //Location
            UpperRange.Location = new System.Drawing.Point(12, 50);
            LowerRange.Location = new System.Drawing.Point(13, 200);
            strip1.Location = new System.Drawing.Point(0, 28);
            //size   name   tab index
            UpperRange.Name = "UpperRange";
            UpperRange.Size = new System.Drawing.Size(193, 56);
            UpperRange.TabIndex = 0;

            LowerRange.Name = "LowerRange";
            LowerRange.Size = new System.Drawing.Size(193, 56);
            LowerRange.TabIndex = 1;
            LowerRange.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            //Control
            Controls.AddRange(new System.Windows.Forms.Control[] { UpperRange, LowerRange , this.texts , this.lowLabel , this.upLabel });
            LowerRange.Scroll += new EventHandler(this.LowerRange_Scroll);
            UpperRange.Scroll += new EventHandler(this.UpperRange_Scroll);

            ((System.ComponentModel.ISupportInitialize)(UpperRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(LowerRange)).EndInit();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public virtual void LowerRange_Scroll(object sender, EventArgs e )
        {
            LowerRange.Minimum = this.min;
            LowerRange.Maximum = this.max - 10;
            lower_range = LowerRange.Value;
            this.lowLabel.Text = lower_range.ToString();
            if (this.load) {
                this.imgMask = sample.allInOne(W: this.max, H: W, upper: upper_range, lower: lower_range);
                CvInvoke.Imshow("MASK SAMPLE", imgMask);
                bMap = imgMask.ToBitmap<Bgr, Byte>();
            }
            

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void UpperRange_Scroll(object sender, EventArgs e)
        {
            UpperRange.Maximum = this.max;
            upper_range = UpperRange.Value;
            if (upper_range < lower_range) { this.upper_range = this.lower_range + 11; }
            this.upLabel.Text = upper_range.ToString();
            if (this.load) {
                this.imgMask = sample.allInOne(W: this.max, H: W, upper: upper_range, lower: lower_range);
                CvInvoke.Imshow("MASK SAMPLE", imgMask);
                bMap = imgMask.ToBitmap<Bgr, Byte>();
            }
 
        }

        public void LoadButton_Click(object sender, EventArgs e)
        {
            load = false;
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Tuple<List<int[]>, int, int, int, int> prominentRaw = sample.read_csv(ofd.FileName);
                this.csvList = prominentRaw.Item1;
                this.max = prominentRaw.Item2;
                this.min = prominentRaw.Item3;
                this.W = prominentRaw.Item4;
                this.H = prominentRaw.Item5;
                

            }
            this.texts.Text = "MAX :" + this.max.ToString() + "  Min: " + this.min + "  W: " + this.W + " H :" +this.H.ToString();
            load = true;
        }


        private void makeSample_Click(object sender, EventArgs e)
        {
            this.imgMask = sample.allInOne(W: this.max, H: W, upper: upper_range, lower: lower_range);
            //Image<Bgr, byte> smallImgMask = new Image<Bgr, byte>(H , (int)W/2); 
            //CvInvoke.Resize(this.imgMask, smallImgMask, new Size(0, 0), fx: .5, fy: 1);
            if (this.mode_2) { this.imgMask2 = sample.allInOne1(W: this.max, H: W, upper: upper_range, lower: lower_range); CvInvoke.Imshow("mode1", imgMask2); }
            if (this.mode_3){ this.imgMask3 = sample.allInOne2(W: this.max, H: W, upper: upper_range, lower: lower_range); CvInvoke.Imshow("mode2", imgMask3); }
            CvInvoke.Imshow("MASK SAMPLE", imgMask);
            
            
            bMap = imgMask.ToBitmap<Bgr, Byte>();
        }

        private void Image_Click(object sender, EventArgs e)
        {

            output = sample.resultImage(imgMask, csvList, H, W);
            if (this.mode_2) { Image<Bgr, byte> output1 = sample.resultImage(imgMask2, csvList, H, W); CvInvoke.Imshow(" output2", output1); }
            if (this.mode_3) { Image<Bgr, byte> output2 = sample.resultImage(imgMask3, csvList, H, W); CvInvoke.Imshow(" output3", output2); }
            //Image<Bgr, byte> smallImgMask = new Image<Bgr, byte>(H, (int)W / 2);
            //CvInvoke.Resize(this.imgMask, smallImgMask, new Size(0, 0), fx: .5, fy: 1);
            CvInvoke.Imshow(" output", output);
            CvInvoke.Imwrite("photo.png", output);
        } 

        private void Sample_Click(object sender, EventArgs e)
        {
            Sample.Image = bMap;
        }

        private void mode1_CheckedChanged(object sender, EventArgs e)
        {
           this.mode_1  = mode1.Checked; 

        }

        private void Mode2_CheckedChanged(object sender, EventArgs e)
        {
            this.mode_2 = Mode2.Checked;
        }

        private void Mode3_CheckedChanged(object sender, EventArgs e)
        {
            this.mode_3 = Mode3.Checked;
        }
    }
}
