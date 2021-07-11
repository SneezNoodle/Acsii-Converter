using System.Windows.Media.Imaging;

namespace AsciiConverterUI.Services
{
    public static class ImageReader
    {
        /// <summary>
        /// Converts a bitmap image into a linear sequence of Color objects.
        /// </summary>
        public static System.Drawing.Color[] GetPixels(BitmapSource image)
        {
            // I hate this whole ass thing

            // Create array to hold results
            var pixels = new System.Drawing.Color[image.PixelWidth * image.PixelHeight];
            byte[] pixelBytes = new byte[image.PixelWidth * image.PixelHeight * 4];

            // Get pixels as bytes
            int stride = ((image.PixelWidth * image.Format.BitsPerPixel + 31) / 32) * 4; // I stole this line from stackoverflow so it might break
            image.CopyPixels(pixelBytes, stride, 0);

            // Convert bytes to RGB
            for (int i = 0; i < pixels.Length; i++)
            {
                int byteStartIndex = i * 4;

                int r = pixelBytes[byteStartIndex];
                int g = pixelBytes[byteStartIndex + 1];
                int b = pixelBytes[byteStartIndex + 2];
                int a = pixelBytes[byteStartIndex + 3];

                pixels[i] = System.Drawing.Color.FromArgb(a, r, g, b);
            }

            return pixels;
        }
        /// <summary>
        /// Converts a sequence of Color objects into their lightness values.
        /// </summary>
        public static float[] GetLightnessMap(System.Drawing.Color[] pixels)
        {
            // Get and save each pixel's brightness
            float[] lightnessMap = new float[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                // Calculate lightness
                //float r = pixels[i].R / 255;
                //float g = pixels[i].G / 255;
                //float b = pixels[i].B / 255;

                //float cMax = Max(r, g, b);
                //float cMin = Min(r, g, b);

                //float lightness = (cMax + cMin) / 2;
                float lightness = pixels[i].GetBrightness();

                // Add the current pixel's lightness value to the map
                lightnessMap[i] = lightness;
            }
            return lightnessMap;
        }

        private static float Max(params float[] numbers)
        {
            float max = 0;
            foreach (float number in numbers)
                max = number > max ? number : max;
            return max;
        }
        private static float Min(params float[] numbers)
        {
            float min = 0;
            foreach (float number in numbers)
                min = number < min ? number : min;
            return min;
        }
    }
}
