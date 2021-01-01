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
    public class XYZW : IEquatable<XYZW>
    {
        public XYZW(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public virtual float W { get; set; }

        public virtual float X { get; set; }

        public virtual float Y { get; set; }

        public virtual float Z { get; set; }

        public static bool operator !=(XYZW left, XYZW right)
        {
            return !(left == right);
        }

        public static bool operator ==(XYZW left, XYZW right)
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
            return other is XYZW xyzw && Equals(xyzw);
        }

        public bool Equals(XYZW other)
        {
            if (ReferenceEquals(other, default)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = -1743314642;

                result = result * -1521134295 + X.GetHashCode();
                result = result * -1521134295 + Y.GetHashCode();
                result = result * -1521134295 + Z.GetHashCode();
                result = result * -1521134295 + W.GetHashCode();

                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}, Z: {2}, W: {3}", X, Y, Z, W);
        }
    }
}