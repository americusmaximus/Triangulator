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
using System.Globalization;

namespace WaterWave
{
    public class CurveIndex : IEquatable<CurveIndex>
    {
        public CurveIndex(float start, float end, int curve2DIndex)
        {
            Start = start;
            End = end;
            Curve2D = curve2DIndex;
        }

        public virtual int Curve2D { get; set; }

        public virtual float End { get; set; }

        public virtual float Start { get; set; }

        public static bool operator !=(CurveIndex left, CurveIndex right)
        {
            return !(left == right);
        }

        public static bool operator ==(CurveIndex left, CurveIndex right)
        {
            if (ReferenceEquals(left, right)) { return true; }

            return left.Equals(right);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, default)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return other is CurveIndex index && Equals(index);
        }

        public bool Equals(CurveIndex other)
        {
            if (ReferenceEquals(this, default)) { return false; }

            return Start == other.Start && End == other.End && Curve2D == other.Curve2D;
        }

        public override int GetHashCode()
        {
            var result = -540012629;

            result = result * -1521134295 + Start.GetHashCode();
            result = result * -1521134295 + End.GetHashCode();
            result = result * -1521134295 + Curve2D.GetHashCode();

            return result;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Start.ToString("F6", CultureInfo.InvariantCulture), End.ToString("F6", CultureInfo.InvariantCulture), Curve2D.ToString(CultureInfo.InvariantCulture));
        }
    }
}