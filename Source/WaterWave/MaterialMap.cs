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


using WaterWave.Enums;

namespace WaterWave
{
    public class MaterialMap
    {
        public MaterialMap()
        {
            IsHorizontalBlendingEnabled = true;
            IsVerticalBlendingEnabled = true;
            ModifierBase = 0.0f;
            ModifierGain = 1.0f;
            Offset = new XYZ(0.0f, 0.0f, 0.0f);
            ScalarChannel = MapChannel.Luminance;
            Scale = new XYZ(1.0f, 1.0f, 1.0f);
            Turbulence = new XYZ(0.0f, 0.0f, 0.0f);
        }

        public MaterialMap(string fileName) : this()
        {
            FileName = fileName;
        }

        public virtual float Boost { get; set; }

        public virtual float BumpMultiplier { get; set; }

        public virtual string FileName { get; set; }

        public virtual bool IsClampingEnabled { get; set; }

        public virtual bool IsColorCorrectionEnabled { get; set; }

        public virtual bool IsHorizontalBlendingEnabled { get; set; }

        public virtual bool IsVerticalBlendingEnabled { get; set; }

        public virtual float ModifierBase { get; set; }

        public virtual float ModifierGain { get; set; }

        public virtual XYZ Offset { get; set; }

        public virtual MapChannel ScalarChannel { get; set; }

        public virtual XYZ Scale { get; set; }

        public virtual int TextureResolution { get; set; }

        public virtual XYZ Turbulence { get; set; }
    }
}