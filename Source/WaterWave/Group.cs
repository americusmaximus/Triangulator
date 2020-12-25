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

using System.Collections.Generic;

namespace WaterWave
{
    public class Group
    {
        public Group()
        {
            Curves = new List<Curve>();
            Curves2D = new List<Curve2D>();
            Faces = new List<Face>();
            Lines = new List<Line>();
            Points = new List<Point>();
            Surfaces = new List<Surface>();
        }

        public Group(string name) : this()
        {
            Name = name;
        }

        public virtual IList<Curve> Curves { get; protected set; }

        public virtual IList<Curve2D> Curves2D { get; protected set; }

        public virtual IList<Face> Faces { get; protected set; }

        public virtual IList<Line> Lines { get; protected set; }

        public virtual string Name { get; set; }

        public virtual IList<Point> Points { get; protected set; }

        public virtual IList<Surface> Surfaces { get; protected set; }
    }
}