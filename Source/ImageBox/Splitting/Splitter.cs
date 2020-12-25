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
using System.Drawing;

namespace ImageBox.Splitting
{
    public class Splitter
    {
        public Splitter(Image image)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public virtual Image Image { get; protected set; }

        public virtual SplitterResult[][] Split(SplitterRequest request)
        {
            if (request == default) { throw new ArgumentNullException(nameof(request)); }

            if (request.Type == SplitType.Piece)
            {
                if (request.Horizontal <= 0) { throw new ArgumentOutOfRangeException(nameof(request.Horizontal), "Width has to be a positive number."); }
                if (request.Vertical <= 0) { throw new ArgumentOutOfRangeException(nameof(request.Vertical), "Height has to be a positive number."); }

                var width = (int)Math.Ceiling((float)Image.Width / request.Horizontal);
                var height = (int)Math.Ceiling((float)Image.Height / request.Vertical);

                return Split(width, height);
            }

            // Pixel
            return Split(request.Horizontal, request.Vertical);
        }

        protected virtual SplitterResult[][] Split(int width, int height)
        {
            if (width <= 0) { throw new ArgumentOutOfRangeException(nameof(width), "Width has to be a positive number."); }
            if (height <= 0) { throw new ArgumentOutOfRangeException(nameof(height), "Height has to be a positive number."); }

            if (Image.Width < width) { throw new ArgumentException("Width cannot be larger than the image width.", nameof(width)); }
            if (Image.Height < height) { throw new ArgumentException("Height cannot be larger than the image height.", nameof(height)); }

            var horizontalCount = (int)Math.Ceiling((float)Image.Width / width);
            var verticalCount = (int)Math.Ceiling((float)Image.Height / height);

            var result = new SplitterResult[horizontalCount][];

            for (int x = 0; x < horizontalCount; x++)
            {
                result[x] = new SplitterResult[verticalCount];

                for (int y = 0; y < verticalCount; y++)
                {

                    var bitmap = new Bitmap(width, height);

                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImage(Image, new Rectangle(0, 0, width, height), new Rectangle(x * width, y * height, width, height), GraphicsUnit.Pixel);
                    }

                    var item = new SplitterResult()
                    {
                        Image = bitmap,
                        X = x * width,
                        Y = y * height
                    };

                    result[x][y] = item;
                }
            }

            return result;
        }
    }
}
