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

using ImageBox.Coloring;
using ImageBox.Flipping;
using ImageBox.Rotation;
using ImageBox.Splitting;
using ImageBox.Statistics;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageBox
{
    public class ImageBox : IDisposable
    {
        public ImageBox(Image image)
        {
            if (image == default) { throw new ArgumentNullException(nameof(image)); }

            Image = new Bitmap(image);
        }

        ~ImageBox()
        {
            if (!IsDisposed)
            {
                Dispose();
            }
        }

        public virtual Image Image { get; protected set; }
        public virtual bool IsDisposed { get; protected set; }

        public virtual SplitterResult[][] SplitResult { get; protected set; }

        public virtual StatsResult Stats { get; protected set; }

        public virtual ImageBox Color(ColorMatrix colorMatrix)
        {
            var result = new Colorer(Image).Color(colorMatrix);

            Image.Dispose();
            Image = result;

            return this;
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                Image?.Dispose();

                if (SplitResult != default)
                {
                    for(var x = 0; x < SplitResult.Length; x++)
                    {
                        for(var y = 0; x < SplitResult[x].Length; y++)
                        {
                            SplitResult[x][y].Image?.Dispose();
                        }
                    }
                }

                GC.SuppressFinalize(this);
            }
        }

        public virtual ImageBox Flip(FlipType type)
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(ImageBox)); }

            var result = new Flipper(Image).Flip(type);

            Image.Dispose();
            Image = result;

            return this;
        }

        public virtual ImageBox Rotate(float angle, Color color)
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(ImageBox)); }

            var result = new Rotor(Image).Rotate(angle, color);

            Image.Dispose();
            Image = result;

            return this;
        }

        public virtual ImageBox Split(SplitterRequest request)
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(ImageBox)); }

            SplitResult = new Splitter(Image).Split(request);

            return this;
        }

        public virtual ImageBox Statistics()
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(ImageBox)); }

            Stats = new Stats(Image).Count();

            return this;
        }
    }
}
