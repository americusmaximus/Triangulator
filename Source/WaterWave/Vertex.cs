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

namespace WaterWave
{
    public class Vertex : IEquatable<Vertex>
    {
        public Vertex(XYZW position)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
        }

        public Vertex(float x, float y, float z, float w)
        {
            Position = new XYZW(x, y, z, w);
        }

        public Vertex(float x, float y, float z, float r, float g, float b)
        {
            Position = new XYZW(x, y, z, 1.0f);
            Color = new XYZW(r, g, b, 1.0f);
        }

        public Vertex(float x, float y, float z, float r, float g, float b, float a)
        {
            Position = new XYZW(x, y, z, 1.0f);
            Color = new XYZW(r, g, b, a);
        }

        public virtual XYZW Color { get; set; }

        public virtual XYZW Position { get; set; }

        public static bool operator !=(Vertex left, Vertex right)
        {
            return !(left == right);
        }

        public static bool operator ==(Vertex left, Vertex right)
        {
            if (ReferenceEquals(left, default))
            {
                if (ReferenceEquals(right, default)) { return true; }

                return false;
            }

            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is Vertex vertex && Equals(vertex);
        }

        public bool Equals(Vertex other)
        {
            if (ReferenceEquals(other, default)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return Position == other.Position && Color == other.Color;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = -2056440846 * -1521134295 + Position.GetHashCode();

                if (Color != default)
                {
                    hashCode = hashCode * -1521134295 + Color.GetHashCode();
                }

                return hashCode;
            }
        }
    }
}