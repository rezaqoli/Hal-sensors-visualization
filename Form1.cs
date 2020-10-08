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

        int max = 2000;
        int min = 0;
        int W = 2000;
        int H = 80;
        NDArray npData;
        int upper_range;
        int lower_range;
        NDArray npMask;
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
            this.texts = new TextBox(); this.texts.Location = new System.Drawing.Point(12, 280); this.texts.Size = new System.Drawing.Size(48, 100);
            this.texts.Text = "FUCK YOU";
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


             List<List<int>> csvList;
            

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
            if (upper_range < lower_range) { upper_range = lower_range + 10; }
            this.upLabel.Text = upper_range.ToString();
            
        }

        public void LoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Tuple<List<List<int>>, int, int, int, int> prominentRaw = sample.read_csv(ofd.FileName);
                List<List<int>> csvList = prominentRaw.Item1;
                this.max = prominentRaw.Item2;
                this.min = prominentRaw.Item3;
                this.W = prominentRaw.Item4;
                this.H = prominentRaw.Item5;
                NDArray npData = sample.convertToNp(csvList, H, W, max, min);

            }
            this.texts.Text = "MAX :" + this.max.ToString() + "  Min: " + this.min + "  W: " + this.W + " H :" +this.H.ToString(); 
        }


        private void makeSample_Click(object sender, EventArgs e)
        {
            npMask=sample.mask_maker(W, H, upper_range, lower_range, max  , reverse: false );
            npMask.astype(NPTypeCode.Byte);
            npMask = npMask.reshape(1, this.max, this.W, 3);
            npMask=npMask.astype(NPTypeCode.Byte);
            Bitmap bmp = npMask.ToBitmap();
            Image<Bgr, byte> ImageCV = bmp.ToImage<Bgr, byte>();
            CvInvoke.Imshow("ANGOH", ImageCV);
            bMap = bmp; 
        }

        private void Image_Click(object sender, EventArgs e)
        {
            NDArray npOut =sample.mapping(npData, npMask, min);
            Bitmap bmp = npOut.ToBitmap();
            Image<Bgr, byte> ImageCV = bmp.ToImage<Bgr, byte>();
            CvInvoke.Imshow("OUTPUT WINDOW", ImageCV);
        } 

        private void Sample_Click(object sender, EventArgs e)
        {
            Sample.Image = bMap;
        }
    }
}
