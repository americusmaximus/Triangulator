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
    public class Triplet : IEquatable<Triplet>
    {
        public Triplet(int vertexIndex, int textureIndex, int normalIndex)
        {
            Vertex = vertexIndex;
            Texture = textureIndex;
            Normal = normalIndex;
        }

        public virtual int Normal { get; set; }

        public virtual int Texture { get; set; }

        public virtual int Vertex { get; set; }

        public static bool operator !=(Triplet left, Triplet right)
        {
            return !(left == right);
        }

        public static bool operator ==(Triplet left, Triplet right)
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
            return obj is Triplet triplet && Equals(triplet);
        }

        public bool Equals(Triplet other)
        {
            if (ReferenceEquals(other, default)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return Vertex == other.Vertex && Texture == other.Texture && Normal == other.Normal;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = -683219715 * -1521134295 + Vertex.GetHashCode();

                result = result * -1521134295 + Texture.GetHashCode();
                result = result * -1521134295 + Normal.GetHashCode();

                return result;
            }
        }

        public override string ToString()
        {
            if (Normal == 0)
            {
                if (Texture == 0)
                {
                    return Vertex.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    return string.Format("{0}/{1}", Vertex.ToString(CultureInfo.InvariantCulture), Texture.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
            {
                if (Texture == 0)
                {
                    return string.Format("{0}//{1}", Vertex.ToString(CultureInfo.InvariantCulture), Normal.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    return string.Format("{0}/{1}/{2}", Vertex.ToString(CultureInfo.InvariantCulture), Texture.ToString(CultureInfo.InvariantCulture), Normal.ToString(CultureInfo.InvariantCulture));
                }
            }
        }
    }
}