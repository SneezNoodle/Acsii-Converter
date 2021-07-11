using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;

namespace AsciiConverterUI.Services
{
    public class AsciiConverter
    {
        public char[] Palette { get; set; }
        public int ThreadCount { get; set; }

        public AsciiConverter(int threadCount, char[] palette)
        {
            ThreadCount = threadCount;
            Palette = palette;

            //ThreadCount = 10;
            //ShadeChars = new char[]
            //{
            //    ' ',
            //    '.',
            //    '<',
            //    '(',
            //    '#',
            //    '%',
            //};
        }

        /// <summary>
        /// Returns the formatted ascii string representing the info given when constructing.
        /// </summary>
        /// <returns></returns>
        public string GetAsciiString(float[] pixels, int imageWidth)
        {
            char[] acsiiLine = new char[pixels.Length];

            // Create tasks
            var tasks = new AsciiTask[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                // Calculate start and end indices
                int charsPerThread = acsiiLine.Length / ThreadCount; // TODO: This might cause a minor rounding error

                int startIndex = i * charsPerThread;
                int endIndex = (i * charsPerThread) + charsPerThread;

                tasks[i] = new AsciiTask(acsiiLine, pixels, Palette, startIndex, endIndex);
            }

            #region Run tasks
            // Create threads and attach tasks
            var threads = new Thread[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
                threads[i] = new Thread(tasks[i].Execute);

            // Start threads
            for (int i = 0; i < ThreadCount; i++)
                threads[i].Start();

            // Join threads
            for (int i = 0; i < ThreadCount; i++)
                threads[i].Join();
            #endregion

            // Insert newlines
            var finalResult = new StringBuilder(new string(acsiiLine));
            for (int i = finalResult.Length - 1; i >= 0; i--)
            {
                if ((i + 1) % imageWidth == 0) // At the end of a line
                {
                    // Insert a newline after asciiResult[i]
                    finalResult.Insert(i + 1, '\n');
                }
            }

            return finalResult.ToString();
        }

        private class AsciiTask
        {
            public readonly char[] asciiResult;
            private readonly float[] pixels;
            private readonly char[] palette;
            private readonly int startIndex;
            private readonly int endIndex;

            public AsciiTask(char[] asciiResult, float[] pixels, char[] palette, int startIndex, int endIndex)
            {
                this.asciiResult = asciiResult;
                this.pixels = pixels;
                this.palette = palette;
                this.startIndex = startIndex;
                this.endIndex = endIndex;
            }

            public void Execute()
            {
                // From startIndex to endIndex, assign each pixel to it's repective symbol
                for (int i = startIndex; i < endIndex; i++)
                {
                    asciiResult[i] = GetAsciiSymbol(pixels[i]);
                }
            }

            private char GetAsciiSymbol(float shade)
            {
                // Used as a sort of "step size", where the shade changes based on the multiple of this value
                float palleteSizeReciprocal = 1.0f / palette.Length; // I have no clue what to name this
                // This variable is used as the distance between A and B
                // A  -  B  -  C  -  D  -  E  -  F
                // '     '.....'((((('#####'%%%%%'

                // Map the brightness of the shade to it's respective char
                for (int i = 0; i < palette.Length; i++)
                {
                    if (shade <= palleteSizeReciprocal * (i + 1))
                        return palette[i];
                }
                return palette[^1]; // Return the brightest char as a failsafe
            }
        }
    }
}
