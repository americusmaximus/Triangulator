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

namespace WaterWave
{
    public class MaterialColor
    {
        public MaterialColor()
        {
            SpectralFactor = 1.0f;
            Color = new XYZ();
        }

        public MaterialColor(float x, float y, float z) : this()
        {
            Color = new XYZ(x, y, z);
        }

        public MaterialColor(string spectralFileName) : this()
        {
            SpectralFileName = spectralFileName;
        }

        public MaterialColor(string spectralFileName, float factor) : this()
        {
            SpectralFileName = spectralFileName;
            SpectralFactor = factor;
        }

        public virtual XYZ Color { get; set; }

        public virtual bool IsRGB
        {
            get { return !IsSpectral && !UseXYZColorSpace; }
        }

        public virtual bool IsSpectral
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SpectralFileName);
            }
        }

        public virtual bool IsXYZ
        {
            get { return !IsSpectral && UseXYZColorSpace; }
        }

        public virtual float SpectralFactor { get; set; }

        public virtual string SpectralFileName { get; set; }

        public virtual bool UseXYZColorSpace { get; set; }
    }
}