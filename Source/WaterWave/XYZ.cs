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
    public class XYZ : IEquatable<XYZ>
    {
        public XYZ(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public XYZ() { }

        public virtual float X { get; set; }

        public virtual float Y { get; set; }

        public virtual float Z { get; set; }

        public static bool operator !=(XYZ left, XYZ right)
        {
            return !(left == right);
        }

        public static bool operator ==(XYZ left, XYZ right)
        {
            if (ReferenceEquals(left, default))
            {
                if (ReferenceEquals(right, default)) { return true; }

                return false;
            }

            return left.Equals(right);
        }

        public override bool Equals(object other)
        {
            return other is XYZ xyz && Equals(xyz);
        }

        public bool Equals(XYZ other)
        {
            if (ReferenceEquals(other, default)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 373119288;

                result = result * -1521134295 + X.GetHashCode();
                result = result * -1521134295 + Y.GetHashCode();
                result = result * -1521134295 + Z.GetHashCode();

                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}, Z: {2}", X, Y, Z);
        }
    }
}