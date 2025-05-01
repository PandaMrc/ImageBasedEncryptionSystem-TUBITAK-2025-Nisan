using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ImageBasedEncryptionSystem.TypeLayer;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// Resim ve veri analizi için çeşitli metotlar içeren sınıf
    /// </summary>


    public class Analysis
    {
        public static Bitmap GetImageDifference(Bitmap img1, Bitmap img2)
        {
            Bitmap diff = new Bitmap(img1.Width, img1.Height);
            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    Color c1 = img1.GetPixel(x, y);
                    Color c2 = img2.GetPixel(x, y);

                    int r = Math.Abs(c1.R - c2.R);
                    int g = Math.Abs(c1.G - c2.G);
                    int b = Math.Abs(c1.B - c2.B);

                    diff.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return diff;
        }

        public static double CalculateMSE(Bitmap img1, Bitmap img2)
        {
            double mse = 0;
            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    Color c1 = img1.GetPixel(x, y);
                    Color c2 = img2.GetPixel(x, y);

                    mse += Math.Pow(c1.R - c2.R, 2);
                    mse += Math.Pow(c1.G - c2.G, 2);
                    mse += Math.Pow(c1.B - c2.B, 2);
                }
            }
            return mse / (img1.Width * img1.Height * 3.0);
        }

        public static double CalculatePSNR(double mse)
        {
            return mse == 0 ? double.PositiveInfinity : 10 * Math.Log10(255 * 255 / mse);
        }

        public static double CalculateEntropy(Bitmap bmp)
        {
            int[] histogram = new int[256];
            double entropy = 0.0;

            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                    histogram[bmp.GetPixel(x, y).R]++;

            int total = bmp.Width * bmp.Height;
            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] == 0) continue;
                double p = (double)histogram[i] / total;
                entropy -= p * Math.Log(p, 2);
            }
            return entropy;
        }
    }

}
