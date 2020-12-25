#region License
/*
MIT License

Copyright (c) 2020 Americus Maximus

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageBox.Statistics
{
    public class Stats
    {
        public Stats(Image image)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public virtual Image Image { get; protected set; }

        public virtual StatsResult Count()
        {
            var darkest = Color.White;
            var darkestValue = int.MaxValue;

            var brightest = Color.Black;
            var brightestValue = int.MinValue;

            var isGrayScale = true;

            var pixels = new Dictionary<int, StatsCount>(Image.Width * Image.Height);

            using (var bitmap = new Bitmap(Image))
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    for (var y = 0; y < bitmap.Height; y++)
                    {
                        var pixel = bitmap.GetPixel(x, y);

                        // Brightness
                        var colorBrightness = CalculateBrightness(pixel);

                        if (colorBrightness < darkestValue)
                        {
                            darkestValue = colorBrightness;
                            darkest = pixel;
                        }

                        if (colorBrightness > brightestValue)
                        {
                            brightestValue = colorBrightness;
                            brightest = pixel;
                        }

                        // GrayScale
                        if (pixel.R != pixel.G && pixel.G != pixel.B && pixel.B != pixel.R)
                        {
                            isGrayScale = false;
                        }

                        // Counting
                        var argb = pixel.ToArgb();
                        if (pixels.ContainsKey(argb))
                        {
                            pixels[argb].Count++;
                        }
                        else
                        {
                            pixels.Add(argb, new StatsCount() { Color = pixel, Count = 1 });
                        }
                    }
                }
            }

            return new StatsResult()
            {
                Brightest = brightest,
                Counts = pixels.Values.OrderByDescending(p => p.Count).ToArray(),
                Darkest = darkest,
                IsGrayScale = isGrayScale
            };
        }

        protected virtual int CalculateBrightness(Color color)
        {
            return (int)Math.Sqrt(color.R * color.R * 0.241 + color.G * color.G * 0.691 + color.B * color.B * 0.068);
        }
    }
}
