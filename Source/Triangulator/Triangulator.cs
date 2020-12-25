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
using ImageBox.Splitting;
using System;
using System.Drawing;
using WaterWave;

namespace Triangulator
{
    public class Triangulator
    {
        public virtual Obj[][] Triangulate(Image image, TriangulatorRequest request)
        {
            var parts = Split(Modify(image, request), request);

            var objs = new Obj[parts.Length][];

            for (var x = 0; x < parts.Length; x++)
            {
                var line = parts[x];
                objs[x] = new Obj[line.Length];

                for (var y = 0; y < line.Length; y++)
                {
                    var offsetX = request.OffsetX + x * (line[y].Image.Width - 1) * request.ScaleX;
                    var offsetZ = request.OffsetZ + y * (line[y].Image.Height - 1) * request.ScaleZ;

                    objs[x][y] = TriangulateImage(line[y].Image, new TriangulatorRequest()
                    {
                        IgnoreTransparent = request.IgnoreTransparent,

                        OffsetX = offsetX,
                        OffsetZ = offsetZ,

                        MinimumHeight = request.MinimumHeight,
                        MaximumHeight = request.MaximumHeight,

                        ScaleX = request.ScaleX,
                        ScaleZ = request.ScaleZ
                    });
                }
            }

            return objs;
        }

        protected virtual Image Modify(Image image, TriangulatorRequest request)
        {
            var imageBox = new ImageBox.ImageBox(image).Statistics();

            return imageBox.Color(!imageBox.Stats.IsGrayScale ? ColorerMatrix.GrayScale() : ColorerMatrix.Identity())
                         .Rotate(request.Angle, request.Color)
                         .Flip(request.FlipType).Image;
        }

        protected virtual SplitterResult[][] Split(Image image, TriangulatorRequest request)
        {
            if (image == default) { throw new ArgumentNullException(nameof(image)); }

            // Single
            if ((request.SplitType == SplitType.Piece && request.SplitX == 1 && request.SplitY == 1) || (request.SplitType == SplitType.Pixel && request.SplitX == image.Width && request.SplitY == image.Height))
            {
                return new SplitterResult[][] { new SplitterResult[] { new SplitterResult() { Image = image } } };
            }

            // Multiple
            return new Splitter(image).Split(new SplitterRequest()
            {
                Type = request.SplitType,
                Horizontal = request.SplitX,
                Vertical = request.SplitY
            });
        }

        protected virtual Obj TriangulateImage(Image image, TriangulatorRequest request)
        {
            if (image == default) { throw new ArgumentNullException(nameof(image)); }
            if (request == default) { throw new ArgumentNullException(nameof(request)); }

            var obj = new Obj();

            using (var bmp = new Bitmap(image))
            {
                var height = image.Height;
                var width = image.Width;

                var vertexIndex = 0;
                var vertexIndexes = new int[height * width];

                // Vertices
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var pixel = bmp.GetPixel(x, y);

                        if (request.IgnoreTransparent && pixel.A == 0)
                        {
                            vertexIndexes[y * width + x] = -1;
                            continue;
                        }

                        vertexIndexes[y * width + x] = vertexIndex;
                        vertexIndex++;

                        obj.Vertices.Add(new Vertex(request.OffsetX + x * request.ScaleX, request.MinimumHeight + (request.MaximumHeight - request.MinimumHeight) * ((float)pixel.R / byte.MaxValue), request.OffsetZ + y * request.ScaleZ, 1f));

                        obj.VertexNormals.Add(new XYZ(0, 0, 0));
                        obj.TextureVertices.Add(new XYZ(0, 0, 0));
                    }
                }

                // Faces
                for (var y = 0; y < height - 1; y++)
                {
                    for (var x = 0; x < width - 1; x++)
                    {
                        var linearIndexA1 = vertexIndexes[y * width + x + 1] + 1;
                        var linearIndexB1 = vertexIndexes[y * width + x] + 1;
                        var linearIndexC1 = vertexIndexes[(y + 1) * width + x] + 1;

                        if (linearIndexA1 != 0 && linearIndexB1 != 0 && linearIndexC1 != 0)
                        {
                            var face1 = new Face();

                            face1.Vertices.Add(new Triplet(linearIndexA1, linearIndexA1, linearIndexA1));
                            face1.Vertices.Add(new Triplet(linearIndexB1, linearIndexB1, linearIndexB1));
                            face1.Vertices.Add(new Triplet(linearIndexC1, linearIndexC1, linearIndexC1));

                            obj.Faces.Add(face1);
                        }

                        var linearIndexA2 = vertexIndexes[(y + 1) * width + x] + 1;
                        var linearIndexB2 = vertexIndexes[(y + 1) * width + x + 1] + 1;
                        var linearIndexC2 = vertexIndexes[y * width + x + 1] + 1;

                        if (linearIndexA2 != 0 && linearIndexB2 != 0 && linearIndexC2 != 0)
                        {
                            var face2 = new Face();

                            face2.Vertices.Add(new Triplet(linearIndexA2, linearIndexA2, linearIndexA2));
                            face2.Vertices.Add(new Triplet(linearIndexB2, linearIndexB2, linearIndexB2));
                            face2.Vertices.Add(new Triplet(linearIndexC2, linearIndexC2, linearIndexC2));

                            obj.Faces.Add(face2);
                        }
                    }
                }
            }

            return obj;
        }
    }
}
