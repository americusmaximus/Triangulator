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
using System.Drawing.Imaging;

namespace ImageBox.Coloring
{
    public class Colorer
    {
        public Colorer(Image image)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public virtual Image Image { get; protected set; }

        public virtual Image Color(ColorMatrix colorMatrix)
        {
            if (colorMatrix == default) { throw new ArgumentNullException(nameof(colorMatrix)); }

            var result = new Bitmap(Image.Width, Image.Height);

            using (var g = Graphics.FromImage(result))
            {
                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);

                    g.DrawImage(Image, new Rectangle(0, 0, Image.Width, Image.Height), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            return result;
        }
    }
}
