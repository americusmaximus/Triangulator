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
using System.Drawing.Drawing2D;

namespace ImageBox.Rotation
{
    public class Rotor
    {
        public Rotor(Image image)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public virtual Image Image { get; protected set; }

        public virtual Image Rotate(float angle, Color color)
        {
            if (angle == 0) { return new Bitmap(Image); }

            var matrix = new Matrix();

            matrix.Translate(Image.Width / -2, Image.Height / -2, MatrixOrder.Append);
            matrix.RotateAt(angle, new Point(0, 0), MatrixOrder.Append);

            using (var gp = new GraphicsPath())
            {
                // Transform image points by rotation matrix
                gp.AddPolygon(new[] { new Point(0, 0), new Point(Image.Width, 0), new Point(0, Image.Height) });
                gp.Transform(matrix);

                var pts = gp.PathPoints;

                // Create destination bitmap sized to contain rotated source image
                var bbox = GetBoundingBox(matrix);
                var result = new Bitmap(bbox.Width, bbox.Height);

                using (var g = Graphics.FromImage(result))
                {
                    g.Clear(color);

                    var mDest = new Matrix();
                    mDest.Translate(result.Width / 2, result.Height / 2, MatrixOrder.Append);

                    g.Transform = mDest;
                    g.DrawImage(Image, pts);
                }

                return result;
            }
        }

        protected virtual Rectangle GetBoundingBox(Matrix matrix)
        {
            var gu = new GraphicsUnit();
            var rectangle = Rectangle.Round(Image.GetBounds(ref gu));

            // Transform the four points of the image, to get the resized bounding box
            var topLeft = new Point(rectangle.Left, rectangle.Top);
            var topRight = new Point(rectangle.Right, rectangle.Top);
            var bottomRight = new Point(rectangle.Right, rectangle.Bottom);
            var bottomLeft = new Point(rectangle.Left, rectangle.Bottom);
            var points = new Point[] { topLeft, topRight, bottomRight, bottomLeft };

            using (var gp = new GraphicsPath(points, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line }))
            {
                gp.Transform(matrix);

                return Rectangle.Round(gp.GetBounds());
            }
        }
    }
}
