using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using NumSharp;

namespace sample
{

    public static class MatExtension
    {
        public static dynamic GetValue(this Mat mat, int row, int col, int channel)
        {
            var value = CreateElement(mat.Depth);
            Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize + channel, value, 0, 1);
            return value[0];
        }

        public static void SetValue(this Mat mat, int row, int col, dynamic value, int channel, bool fill = false)
        {
            var target = CreateElement(mat.Depth, value);
            if (fill) { Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize + channel, 1); }
            else { Marshal.Copy(target, 0, mat.DataPointer + (col * (mat.Rows) + row) * mat.ElementSize + channel, 1); }
            //Marshal.Copy(target, 0, mat.DataPointer + ((mat.Rows * mat.Cols )+ (row * mat.Cols + col)) * mat.ElementSize, 1);
        }
        public static dynamic CreateElement(DepthType depthType, dynamic value)
        {
            var element = CreateElement(depthType);
            element[0] = value;
            return element;
        }

        public static dynamic CreateElement(DepthType depthType)
        {
            if (depthType == DepthType.Cv8S)
            {
                return new sbyte[1];
            }
            if (depthType == DepthType.Cv8U)
            {
                return new byte[1];
            }
            if (depthType == DepthType.Cv16S)
            {
                return new short[1];
            }
            if (depthType == DepthType.Cv16U)
            {
                return new ushort[1];
            }
            if (depthType == DepthType.Cv32S)
            {
                return new int[1];
            }
            if (depthType == DepthType.Cv32F)
            {
                return new float[1];
            }
            if (depthType == DepthType.Cv64F)
            {
                return new double[1];
            }
            return new float[1];
        }
    }


    class sample
    {
        private int W;
        private int H;
        int upper_range;
        int lower_range;
        public bool reverse;
        public sample( )
        {
        }

        public static Mat convertTOmat(NDArray data)
        {
            int rows = data.shape[0]-1;
            int cols = data.shape[1]-1;
            Mat img1 = new Mat(rows, cols, Emgu.CV.CvEnum.DepthType.Cv32S,3);
            for(int i = 0; i<rows; i++)
            {
                for(int j =0; j<cols; j++)
                {
                    for (int k=0; k<3; k++)
                    {
                        byte c = data[i][j][k].astype(NPTypeCode.Byte).GetByte();
                        img1.SetValue(i, j, c, k);
                    }
                }
            }
            return img1;
        }

        public static NDArray GetBitmapBytes(Bitmap image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                unsafe
                {
                    //Create a 1d vector without filling it's values to zero (similar to np.empty)
                    var nd = new NDArray(NPTypeCode.Byte, Shape.Vector(bmpData.Stride * image.Height), fillZeros: false);

                    // Get the respective addresses
                    byte* src = (byte*)bmpData.Scan0;
                    byte* dst = (byte*)nd.Unsafe.Address; //we can use unsafe because we just allocated that array and we know for sure it is contagious.

                    // Copy the RGB values into the array.
                    Buffer.MemoryCopy(src, dst, nd.size, nd.size); //faster than Marshal.Copy
                    return nd.reshape(1, image.Height, image.Width, 3);
                }
            }
            finally
            {
                image.UnlockBits(bmpData);
            }
        }

        public static NDArray mask_maker(int W , int  H,int upper_range, int lower_range, int max  ,bool reverse = false )
        {
            Mat mask = new Mat(W, H, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
            var npMask = np.zeros((W, max, 3));
            int color_range;
            int gray_range;
            if (reverse) {
                color_range = upper_range;
                gray_range = lower_range;
            }
            else
            {
                if ( (upper_range - lower_range) % 2 == 0 ) { color_range = (upper_range - lower_range)/2; gray_range = color_range ; }
                else { color_range = (upper_range - lower_range)/2; gray_range = color_range+1; }
            }
            //---------------------------------COLOR RANGE----------------------------------------------
            var colors = np.zeros((W, color_range,3));
            var hue_values = np.linspace(130, 0, color_range);

            //hue_values = hue_values.reshape(1, color_range);
            colors[": , : , 0"] = hue_values.astype(NPTypeCode.Byte);
            colors[": , : , 1"] = 220;
            colors[": , : , 2"] = 210;
            Mat mymat = convertTOmat(colors);
            Image<Hsv , byte > myimg = mymat.ToImage<Hsv, byte>();
            Image<Bgr, byte> myBgr = new Image<Bgr, byte>(mymat.Width, mymat.Height);
            CvInvoke.CvtColor(myimg, myBgr, ColorConversion.Hsv2Bgr);

            //NDArray colours = colors.reshape(1 , color_range, W , 3);
            Bitmap bmp;
            bmp = myBgr.ToBitmap<Bgr, byte>();
            colors = bmp.ToNDArray(flat: true, copy: false, discardAlpha: false);
            NDArray resColors = np.zeros((W, color_range, 3));
            for (int i=0; i < colors.shape[0]-3; i += 3){
                var color = colors[String.Format("{0}:{1}", i, i + 2)];
                int row, col;
                row = (i / 3) % W;
                col =  i / 3 / W;
                resColors[row][col][0] = colors[i];
                resColors[row][col][1] = colors[i + 1];
                resColors[row][col][2] = colors[i + 2];
            }
            //-----------------------------------GRAY RANGE--------------------------------------------
            var gray = np.zeros((W, gray_range, 3));
            var gray_values = np.linspace(0, 180, gray_range);
            gray_values = gray_values.reshape(gray_range, 1);
            gray[": , :, :"] = gray_values.astype(NPTypeCode.Byte);
            //--------------------------------------STACKING TO GETHER---------------------------------
            
            var stacking=np.hstack(gray, resColors);
            
            var rangi = npMask[String.Format(":, {0}:{1},:", lower_range, upper_range)]; 
           
            npMask[String.Format(":, {0}:{1},:", lower_range , upper_range)] = np.hstack(gray, resColors); 
            npMask[String.Format(":,:{0},:",lower_range)] = gray[0][0];
            npMask[String.Format(":, {0}: ,:", upper_range)] = resColors[-1][-1];
            npMask[String.Format(":, {0}:{1},:",lower_range-5 ,lower_range)] = new byte[] { 255, 255, 255 };
            npMask[String.Format(":,{0}:{1} ,:", upper_range , upper_range+5)] = new byte[] { 255, 255, 255 };

            return npMask;
        }   

        public static NDArray mapping(NDArray input , NDArray mask , int min_cell)
        {
            NDArray output = mask[0][input - 1];
            return output;
        }

        public static NDArray convertToNp(List<List<int>> data, int row, int count, int max, int min)
        {
            int rows = row;
            int cols = count;
            NDArray npData = np.zeros((rows, cols));

            for (int i=0; i < rows;i++)
            {
                for (int j=0; j<cols; j++)
                {
                    npData[i][j] = data[i][j];
                }
            }
            return npData;
        }

        public static Tuple<List<int[]> , int , int , int , int> read_csv(string path)
        {

            using (var reader = new StreamReader(path))
            {

                int[] numbers;
                List<int> listA = new List<int>();
                List<int[]> listB = new List<int[]>();
                int row = 0;
                int min = 2500;
                int max = 0;
                int count = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    count = 0;
                    numbers = Array.ConvertAll(values, s => int.Parse(s));
                    foreach (int number in numbers)
                    {
                        listA.Add(number);
                        count++;
                        if (number > max) max = number;
                        if (number < min) min = number;
                    }
                    int[] arr = new int[listA.Count];
                    listA.CopyTo(arr);
                    listB.Add(arr);
                    row++;
                    listA.Clear();
                }
                return Tuple.Create(listB, max, min , count , row);
            }

        }

        public static Image<Bgr, byte> colormask(int w, int h, int low_hue, int high_hue)
        {
            //if (low_hue > high_hue) { throw new InvalidOperationException("low hue is bigger than high hue"); }
            if (high_hue > 180) { throw new InvalidOperationException("Invalid hue range"); }
            
            Image<Bgr, byte> img = new Image<Bgr, byte>(w, h);
            
            var nparr = np.zeros((h, w, 3));
            nparr.astype(NPTypeCode.Byte);
            var hue = np.linspace(low_hue, high_hue, w);
            hue = hue.astype(NPTypeCode.Byte);
            hue = hue.reshape(1, w);
            //nparr[": , : , 0"] = hue;
            //// nparr[": , : , 1"] = 220;
            //nparr[": , : , 2"] = 210;
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {

                    byte c = hue[0][j].astype(NPTypeCode.Byte).GetByte();
                    //byte c = (byte)hue_list[j];
                    //  tst.SetValue(i, j, c, 0);
                    //  tst.SetValue(i, j, (byte)220, 1);
                    // tst.SetValue(i, j, (byte)210, 2);
                    img.Data[i, j, 0] = hue[0][j].astype(NPTypeCode.Byte).GetByte();
                    img.Data[i, j, 1] = 220;
                    img.Data[i, j, 2] = 210;
                }
            }
         
            Image<Bgr, byte> img2 = new Image<Bgr, byte>(w, h);
            CvInvoke.CvtColor(img, img2, ColorConversion.Hsv2Bgr);       
            return img2;
        }

        public static Image<Bgr, byte> graymask(int w, int h, int gray_low, int gray_high)
        {
            Mat tst;
            Image<Bgr, byte> img = new Image<Bgr, byte>(w, h);
            tst = img.Mat;
            var nparr = np.zeros((h, w, 3));
            nparr.astype(NPTypeCode.Byte);
            var gray = np.linspace(gray_low, gray_high, w);
            gray = gray.reshape(1, w);
            gray = gray.astype(NPTypeCode.Byte);
            nparr[": , : , 0"] = gray;
            nparr[": , : , 1"] = gray;
            nparr[": , : , 2"] = gray;

            byte a = 255;
            byte b = 0;

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        byte c = nparr[i][j][k].astype(NPTypeCode.Byte).GetByte();

                        //tst.SetValue(i, j, c, k);
                        img.Data[i, j, 0] = c;
                        img.Data[i, j, 1] = c;
                        img.Data[i, j, 2] = c;
                    }
                }
            }
            //img = tst.ToImage<Bgr, byte>();
            return img;
        }

        public static Image<Bgr, byte> concatenator(Image<Bgr, byte> img1, Image<Bgr, byte> img2)
        {
            Mat newMat;
            if (img2.Height != img1.Height) { throw new InvalidOperationException("Shape does not match"); }
            Image<Bgr, byte> neuImage = new Image<Bgr, byte>(img1.Width + img2.Width, img1.Height);
            newMat = neuImage.Mat;
            Rectangle ROI1 = new Rectangle(0, 0, img1.Width, img1.Height);
            Mat srcROI = new Mat(img1.Mat, ROI1);
            Mat dstROI = new Mat(newMat, ROI1);
            srcROI.CopyTo(dstROI);
            //dstROI = srcROI;
            ROI1 = new Rectangle(0, 0, img2.Width, img2.Height);
            Mat srcROI2 = new Mat(img2.Mat, ROI1);
            Rectangle ROI2 = new Rectangle(img1.Width, 0, neuImage.Width - img1.Width, neuImage.Height);
            dstROI = new Mat(newMat, ROI2);
            srcROI2.CopyTo(dstROI);
            neuImage = newMat.ToImage<Bgr, byte>();
            return neuImage;
        }

        public static Image<Bgr, Byte> overRange(int w, int h, byte sampleB, byte sampleG, byte sampleR)
        {
            Image<Bgr, byte> overrange = new Image<Bgr, byte>(w, h);
            //Mat matOR = overrange.Mat;
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    overrange.Data[i, j, 0] = sampleB;
                    overrange.Data[i, j, 1] = sampleG;
                    overrange.Data[i, j, 2] = sampleR;

                    //matOR.SetValue(i, j, sampleB, 0, fill: true);
                    //matOR.SetValue(i, j, sampleG, 1, fill: true);
                    //matOR.SetValue(i, j, sampleR, 2, fill: true);
                }
            }
            //overrange = matOR.ToImage<Bgr, byte>();
            return overrange;
        }

        public static Image<Bgr, Byte> underRange(int w, int h, byte sampleB, byte sampleG, byte sampleR)
        {
            Image<Bgr, byte> underrange = new Image<Bgr, byte>(w, h);
            //Mat matUR = underrange.Mat;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    //matUR.SetValue(i, j, sampleB, 0, fill: true);
                    //matUR.SetValue(i, j, sampleG, 1, fill: true);
                    // matUR.SetValue(i, j, sampleR, 2, fill: true);
                    underrange.Data[i, j, 0] = sampleB;
                    underrange.Data[i, j, 1] = sampleG;
                    underrange.Data[i, j, 2] = sampleR;
                }
            }
            //underrange = matUR.ToImage<Bgr, byte>();
            return underrange;
        }

        

        public static Image<Bgr, byte> resultImage(Image<Bgr, byte> mask, List<int[]> data, int H, int W)
        {
            //if (mask.Height != W || mask.Width != H) { throw new InvalidOperationException("wrong mask shape"); }
            int newH = W;
            int newW = H;
            Image<Bgr, byte> img = new Image<Bgr, byte>(newW, newH);
            byte B;
            byte G;
            byte R;
            
            for (int i = 0; i < newW; i++)
            {
                for (int j = 0; j < newH; j++)
                {                   
                    img.Data[j, i, 0] = mask.Data[0, data[i][j], 0];
                    img.Data[j, i, 1] = mask.Data[0, data[i][j], 1];
                    img.Data[j, i, 2] = mask.Data[0, data[i][j], 2];     
                }
                
            }
            return img;
        }

        public static Image<Bgr, byte> allInOne(int W, int H, int upper, int lower )
        {
            
            if (lower >= upper-2 ) { upper = lower + 8; }
            int color_range;
            int gray_range;
            int OverRange = lower; 
            int UnderRange = W - upper;
            Image<Bgr, byte> out1 ;
            //if ((W - (upper - lower)) % 2 == 0) { oth = (W - (upper - lower)) / 2; oth1 = (W - (upper - lower)) / 2; }
            //else { oth = (W - (upper - lower)) / 2; oth1 = oth + 1; }
            if ((upper - lower) % 2 == 0) { color_range = (upper - lower) / 2; gray_range = color_range; }
            else { color_range = (upper - lower) / 2; gray_range = color_range + 1; }
            Image<Bgr, byte> color = sample.colormask(color_range, H, 130, 0);     //Make color mask
            Image<Bgr, byte> gray = sample.graymask(gray_range, H, 0, 180);      //make gray mask

             out1 = sample.concatenator(gray, color);

            byte last = gray.Mat.GetValue(0, 0, 0);
            if (OverRange > 0)
            {
                Image<Bgr, byte> OR = sample.overRange(OverRange, H, last, last, last);

                out1 = sample.concatenator(OR, out1);
            }

            if (UnderRange > 0) {
                byte first = color.Data[0, color.Width - 1, 0]; //color.Mat.GetValue(color.Height-1, color.Width-1, 0);
                byte first1 = color.Data[0, color.Width-1, 1]; //color.Mat.GetValue(color.Height - 1, color.Width - 1, 1);
                byte first2 = color.Data[0, color.Width - 1, 2];//color.Mat.GetValue(color.Height - 1, color.Width - 1, 2);
                Image<Bgr, byte> UR = sample.underRange(UnderRange, H, first, first1, first2);
                out1 = sample.concatenator(out1, UR);
            }
            

            return out1;
        }

        public static Image<Bgr, byte> allInOne1(int W, int H, int upper, int lower)
        {

            if (lower >= upper - 2) { upper = lower + 8; }
            int color_range;
            int gray_range;
            int OverRange = W - upper;
            int UnderRange = lower;
            Image<Bgr, byte> out1;
            if ((upper - lower) % 2 == 0) { color_range = (upper - lower) / 2; gray_range = color_range; }
            else { color_range = (upper - lower) / 2; gray_range = color_range + 1; }
            Image<Bgr, byte> color = sample.colormask(color_range, H, 0, 130);     //Make color mask
            Image<Bgr, byte> gray = sample.graymask(gray_range, H, 180, 0);      //make gray mask

            out1 = sample.concatenator(color, gray);

            byte last = gray.Mat.GetValue(0, gray.Width-1, 0);
            if (OverRange > 0)
            {
                Image<Bgr, byte> OR = sample.overRange(OverRange, H, last, last, last);

                out1 = sample.concatenator(out1, OR);
            }

            if (UnderRange > 0)
            {
                byte first = color.Data[0, 0, 0]; //color.Mat.GetValue(color.Height-1, color.Width-1, 0);
                byte first1 = color.Data[0, 0, 1]; //color.Mat.GetValue(color.Height - 1, color.Width - 1, 1);
                byte first2 = color.Data[0, 0, 2];//color.Mat.GetValue(color.Height - 1, color.Width - 1, 2);
                Image<Bgr, byte> UR = sample.underRange(UnderRange, H, first, first1, first2);
                out1 = sample.concatenator(UR, out1);
            }


            return out1;
        }

        public static Image<Bgr, byte> allInOne2(int W, int H, int upper, int lower)
        {

            if (lower >= upper - 2) { upper = lower + 8; }
            int color_range=upper -lower;
            
            int OverRange = W - upper;
            int UnderRange = lower;
            
            
            

            Image<Bgr, byte> out1 = sample.colormask(color_range, H, 130, 150);     //Make color mask
            

            
            if (OverRange > 0)
            {
                byte first = out1.Data[0, out1.Width - 1, 0]; //color.Mat.GetValue(color.Height-1, color.Width-1, 0);
                byte first1 = out1.Data[0, out1.Width - 1, 1]; //color.Mat.GetValue(color.Height - 1, color.Width - 1, 1);
                byte first2 = out1.Data[0, out1.Width - 1, 2];//color.Mat.GetValue(color.Height - 1, color.Width - 1, 2);
                Image<Bgr, byte> OR = sample.overRange(OverRange, H, first, first1, first2);


                out1 = sample.concatenator(out1, OR);
            }

            if (UnderRange > 0)
            {
                byte first = out1.Data[0, 0, 0]; //color.Mat.GetValue(color.Height-1, color.Width-1, 0);
                byte first1 = out1.Data[0, 0, 1]; //color.Mat.GetValue(color.Height - 1, color.Width - 1, 1);
                byte first2 = out1.Data[0, 0, 2];//color.Mat.GetValue(color.Height - 1, color.Width - 1, 2);
                Image<Bgr, byte> UR = sample.underRange(UnderRange, H, first, first1, first2);
                out1 = sample.concatenator(UR, out1);
            }


            return out1;
        }

    }

    class myform : Form1
    {
        public myform(int max, int min) : base() {
            int Max=max;
            int Min=min;
            
        }
        public override void LowerRange_Scroll(object sender, EventArgs e )
        {
            base.LowerRange_Scroll(sender, e);
            //LowerRange.Maximum = 
            
            
        }


    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {   
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
