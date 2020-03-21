using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace AcrobatImage
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFileName = "dog.bmp",
                targetFileName = "result.bmp";
            int width = 400,
                height = 300;

            // Convert the same file to string
            var convertedText = ConvertImageToText(sourceFileName);

            // Convert stirng to file. You have to know the resolution
            CovertTextToImage(convertedText, width, height, targetFileName);
        }

        static string ConvertImageToText(string filePath)
        {
            Bitmap bmp = new Bitmap(filePath);

            int width = bmp.Width;
            int height = bmp.Height;

            int totalPixels = width * height;

            List<Color> pixelColors = new List<Color>(totalPixels);

            // 0,0 is top left corner of image
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color pixelColor = bmp.GetPixel(j, i);
                    pixelColors.Add(pixelColor);
                }
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < totalPixels; i++)
            {
                Color currentPixel = pixelColors[i];

                // actually all are byte
                byte decA = currentPixel.A;
                byte decR = currentPixel.R;
                byte decG = currentPixel.G;
                byte decB = currentPixel.B;

                string hexValue = decA.ToString("X2") + decR.ToString("X2") + decG.ToString("X2") + decB.ToString("X2");

                sb.Append(hexValue);
            }

            return sb.ToString();
        }

        static void CovertTextToImage(string text, int width, int height, string outputFileName)
        {
            if (text == null)
            {
                Console.WriteLine("Incorrect input");
                return;
            }

            if (text.Length % 8 != 0)
            {
                Console.WriteLine("Incorrect number of characters {0}", text.Length);
                return;
            }

            int totalBytes = text.Length / 2; //1 byte = 2 hex
            int totalPixels = totalBytes / 4; // 1 pixel = ARGB = 4 bytes = 8 hex

            if (totalPixels != width * height)
            {
                Console.WriteLine("Incorrect resolution");
                return;
            }

            List<Color> pixelColors = new List<Color>(totalPixels);

            for (int i = 0; i < text.Length; i += 8)
            {
                string pixelEncoding = text.Substring(i, 8);

                int alpha = int.Parse(pixelEncoding.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);//00
                int red = int.Parse(pixelEncoding.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);//00
                int green = int.Parse(pixelEncoding.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);//00
                int blue = int.Parse(pixelEncoding.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);//00

                Color targetColor = Color.FromArgb(alpha, red, green, blue);
                pixelColors.Add(targetColor);
            }

            var b = new Bitmap(width, height);

            Bitmap bmp = new Bitmap(width, height);
            // 0,0 is top left corner of image
            int pixelIndex = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bmp.SetPixel(j, i, pixelColors[pixelIndex++]);
                }
            }

            bmp.Save(outputFileName, ImageFormat.Jpeg);

            Console.WriteLine("File \"{0}\" saved!", outputFileName);
        }
    }
}