using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using NumSharp;

namespace sample
{   
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
            colors =colors.astype(NPTypeCode.Byte);
            hue_values = hue_values.reshape(1, color_range);
            colors[": , : , 0"] = hue_values.astype(NPTypeCode.Byte);
            colors[": , : , 1"] = .8 * 255;
            colors[": , : , 2"] = 210;
            NDArray colours = colors.reshape(1 , color_range, W , 3);
            Bitmap bmp = colours.ToBitmap(W, color_range);
            Image<Bgr, byte> ImageCV = bmp.ToImage<Bgr, byte>();
            CvInvoke.Imshow("w1", ImageCV);
            Image<Bgr , byte> ImgCV = ImageCV.Convert<Bgr, byte>();
            CvInvoke.Imshow("w", ImageCV);
            bmp = ImgCV.ToBitmap<Bgr, byte>();
            colors = bmp.ToNDArray(flat: true, copy: false, discardAlpha: false);
            NDArray resColors = np.zeros((W, color_range, 3));
            for (int i=0; i < colors.shape[0]-3; i += 3){
                var color = colors[String.Format("{0}:{1}", i, i + 2)];
                int row, col;
                row = (i / 3) % W;
                col =  i / 3 / W;

                //Console.Write(row + "   " + col + "  " + i);
                //    Console.WriteLine(" ");
                resColors[row][col][0] = colors[i];
                resColors[row][col][1] = colors[i + 1];
                resColors[row][col][2] = colors[i + 2];
            }
            colours = resColors.reshape(1, color_range, W, 3);
            colours=colours.astype(NPTypeCode.Byte);
            bmp = colours.ToBitmap(W, color_range);
            ImageCV = bmp.ToImage<Bgr, byte>();
            CvInvoke.Imshow("w2", ImageCV);
            ImgCV = ImageCV.Convert<Bgr, byte>();
            CvInvoke.Imshow("w3", ImageCV);
            //-----------------------------------GRAY RANGE--------------------------------------------
            var gray = np.zeros((W, gray_range, 3));
            var gray_values = np.linspace(0, 180, gray_range);
            gray_values = gray_values.reshape(gray_range, 1);
            gray[": , :, :"] = gray_values.astype(NPTypeCode.Byte);
            //--------------------------------------STACKING TO GETHER---------------------------------
            Console.WriteLine("Hellp" + "   ");
            Console.WriteLine(color_range);
            Console.WriteLine(max );
            Console.WriteLine("GRAY:   "+gray.shape[0] + "   " + gray.shape[1]);
            Console.WriteLine("COlORs:   " + resColors.shape[0] + "   " + resColors.shape[1] +  "   " + resColors.shape[2]);
            Console.WriteLine("npMask:   " + npMask.shape[0] + "  " + npMask.shape[1] + "   " + npMask.shape[2]);
            var stacking=np.hstack(gray, resColors);
            Console.WriteLine("Stacking : "+stacking.shape[0] + "  " + stacking.shape[1] + "   " + stacking.shape[2]);
            var rangi = npMask[String.Format(":, {0}:{1},:", lower_range, upper_range)]; 
            Console.WriteLine("MAIN sample    "  +rangi.shape[0] + "   " + rangi.shape[1] + "    " + rangi.shape[2]);
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

        public static Tuple<List<List<int>> , int , int , int , int> read_csv(string path)
        {

            using (var reader = new StreamReader(path))
            {

                int[] numbers;
                List<int> listA = new List<int>();
                List<List<int>> listB = new List<List<int>>();
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
                    listB.Add(listA);
                    row++;
                }
                return Tuple.Create(listB, max, min , count , row);
            }

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
        {    //-----------------READ DATA --------------------------------
            Tuple<List<List<int>>, int, int, int, int> prominentRaw = sample.read_csv(@"E:\DORSA\Hal sensors\samples1\80_GIL_76.368_no_pro.csv");
            List<List<int>> csvList = prominentRaw.Item1;
            int max = prominentRaw.Item2;
            int min = prominentRaw.Item3;
            int W = prominentRaw.Item4;
            int H = prominentRaw.Item5;
            //--------------------------------------------------------------
            sample sample1 = new sample();
            //--------------------------------------------------------------
            NDArray npData  = sample.convertToNp(csvList, H, W, max, min);
            
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new myform(max , min));
        }
    }
}
