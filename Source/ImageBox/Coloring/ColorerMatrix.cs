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
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;

namespace ImageBox.Coloring
{
    public static class ColorerMatrix
    {
        [ColorerMatrix(ColorerMatrixType.Achromatomaly)]
        public static ColorMatrix Achromatomaly()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.618f, 0.163f, 0.163f, 0, 0 },
                     new float[] { 0.320f, 0.775f, 0.320f, 0, 0 },
                     new float[] { 0.062f, 0.062f, 0.516f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Achromatopsia)]
        public static ColorMatrix Achromatopsia()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                     new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                     new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.AverageGrayScale)]
        public static ColorMatrix AverageGrayScale()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.33f, 0.33f, 0.33f, 0, 0 },
                     new float[] { 0.33f, 0.33f, 0.33f, 0, 0 },
                     new float[] { 0.33f, 0.33f, 0.33f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.BlackAndWhite)]
        public static ColorMatrix BlackAndWhite()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 1.5f, 1.5f, 1.5f, 0, 0 },
                     new float[] { 1.5f, 1.5f, 1.5f, 0, 0 },
                     new float[] { 1.5f, 1.5f, 1.5f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { -1, -1, -1, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Brightness, true, 0, -1, 1)]
        public static ColorMatrix Brightness(float brightness)
        {
            if (brightness < -1) { throw new ArgumentOutOfRangeException(nameof(brightness), string.Format("The value must be in the range from {0} to {1}", -1, 1)); }
            if (brightness > 1) { throw new ArgumentOutOfRangeException(nameof(brightness), string.Format("The value must be in the range from {0} to {1}", -1, 1)); }

            return new ColorMatrix(new float[][]
               {
                     new float[] { 1, 0, 0, 0, 0 },
                     new float[] { 0, 1, 0, 0, 0 },
                     new float[] { 0, 0, 1, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { brightness, brightness, brightness, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Cold)]
        public static ColorMatrix Cold()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.99f, 0, 0, 0, 0 },
                     new float[] { 0, 0.93f, 0, 0, 0 },
                     new float[] { 0, 0, 0.93f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Contrast, true, 1, 0, 2)]
        public static ColorMatrix Contrast(float contrast)
        {
            if (contrast < 0) { throw new ArgumentOutOfRangeException(nameof(contrast), string.Format("The value must be in the range from {0} to {1}", 0, 2)); }
            if (contrast > 2) { throw new ArgumentOutOfRangeException(nameof(contrast), string.Format("The value must be in the range from {0} to {1}", 0, 2)); }

            var t = 0.5f * (1f - contrast);

            return new ColorMatrix(new float[][]
               {
                     new float[] { contrast, 0, 0, 0, 0 },
                     new float[] { 0, contrast, 0, 0, 0 },
                     new float[] { 0, 0, contrast, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { t, t, t, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Deuteranomaly)]
        public static ColorMatrix Deuteranomaly()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.8f, 0.258f, 0, 0, 0 },
                     new float[] { 0.2f, 0.742f, 0.142f, 0, 0 },
                     new float[] { 0, 0, 0.858f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Deuteranopia)]
        public static ColorMatrix Deuteranopia()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.625f, 0.7f, 0, 0, 0 },
                     new float[] { 0.375f, 0.3f, 0.3f, 0, 0 },
                     new float[] { 0, 0, 0.7f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Exposure, true, 1, 0, 4)]
        public static ColorMatrix Exposure(float exposure)
        {
            if (exposure < 0) { throw new ArgumentOutOfRangeException(nameof(exposure), string.Format("The value must be in the range from {0} to {1}", 0, 4)); }
            if (exposure > 4) { throw new ArgumentOutOfRangeException(nameof(exposure), string.Format("The value must be in the range from {0} to {1}", 0, 4)); }

            return new ColorMatrix(new float[][]
               {
                     new float[] { exposure, 0, 0, 0, 0 },
                     new float[] { 0, exposure, 0, 0, 0 },
                     new float[] { 0, 0, exposure, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        public static ColorerMatrixDescriptor Get(ColorerMatrixType type, float? value = default)
        {
            var methods = typeof(ColorerMatrix).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.GetCustomAttributes(typeof(ColorerMatrixAttribute), false).Length != 0).ToArray();

            foreach (var m in methods)
            {
                var attrs = m.GetCustomAttributes(typeof(ColorerMatrixAttribute), false).OfType<ColorerMatrixAttribute>().ToArray();

                foreach (var attr in attrs)
                {
                    if (attr.Type == type)
                    {
                        if (attr.IsValueRequired && value.HasValue)
                        {
                            if (value.Value < attr.MinimumValue) { throw new ArgumentException(string.Format("Value has to be between {0} and {1}.", attr.MinimumValue, attr.MaximumValue), nameof(value)); }
                            if (value.Value > attr.MaximumValue) { throw new ArgumentException(string.Format("Value has to be between {0} and {1}.", attr.MinimumValue, attr.MaximumValue), nameof(value)); }
                        }

                        return new ColorerMatrixDescriptor(
                                        v => (ColorMatrix)m.Invoke(default, attr.IsValueRequired ? new object[] { v } : default),
                                        attr.IsValueRequired ? (float?)(value ?? attr.DefaultValue) : null,
                                        attr.IsValueRequired ? (float?)attr.MinimumValue : null,
                                        attr.IsValueRequired ? (float?)attr.MaximumValue : null);
                    }
                }
            }

            throw new InvalidOperationException(string.Format("Unable to find a matrix for {0}.", type));
        }

        [ColorerMatrix(ColorerMatrixType.GrayScale)]
        public static ColorMatrix GrayScale()
        {
            return new ColorMatrix(new float[][]
                   {
                     new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                     new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
                     new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
                   });
        }

        [ColorerMatrix(ColorerMatrixType.Achromatomaly)]
        public static ColorMatrix Identity()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 1, 0, 0, 0, 0 },
                     new float[] { 0, 1, 0, 0, 0 },
                     new float[] { 0, 0, 1, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Inverted)]
        public static ColorMatrix Inverted()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { -1, 0, 0, 0, 0 },
                     new float[] { 0, -1, 0, 0, 0 },
                     new float[] { 0, 0, -1, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 1, 1, 1, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.LuminanceToAlpha)]
        public static ColorMatrix LuminanceToAlpha()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0, 0, 0, 0, 0 },
                     new float[] { 0, 0, 0, 0, 0 },
                     new float[] { 0, 0, 0, 0, 0 },
                     new float[] { 0.2125f, 0.7154f, 0.0721f, 0, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Negative)]
        public static ColorMatrix Negavtive()
        {
            return Inverted();
        }

        [ColorerMatrix(ColorerMatrixType.NightVision)]
        public static ColorMatrix NightVision()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.1f, 0.3f, 0, 0, 0 },
                     new float[] { 0.4f, 1, 0.4f, 0, 0 },
                     new float[] { 0, 0.3f, 0.1f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Normal)]
        public static ColorMatrix Normal()
        {
            return Identity();
        }

        [ColorerMatrix(ColorerMatrixType.Polaroid)]
        public static ColorMatrix Polaroid()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 1.438f, -0.062f, -0.062f, 0, 0 },
                     new float[] { -0.122f, 1.378f, -0.122f, 0, 0 },
                     new float[] { -0.016f, -0.016f, 1.483f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { -0.03f, 0.05f, -0.02f, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Protanomaly)]
        public static ColorMatrix Protanomaly()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.817f, 0.333f, 0, 0, 0 },
                     new float[] { 0.183f, 0.667f, 0.125f, 0, 0 },
                     new float[] { 0, 0, 0.875f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Protanopia)]
        public static ColorMatrix Protanopia()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.567f, 0.558f, 0, 0, 0 },
                     new float[] { 0.433f, 0.442f, 0.242f, 0, 0 },
                     new float[] { 0, 0, 0.758f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.RGBBGR)]
        public static ColorMatrix RGBBGR()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0, 0, 1, 0, 0 },
                     new float[] { 0, 1, 0, 0, 0 },
                     new float[] { 1, 0, 0, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Saturation, true, 1, 0, 10)]
        public static ColorMatrix Saturation(float saturation)
        {
            if (saturation < 0) { throw new ArgumentOutOfRangeException(nameof(saturation), string.Format("The value must be in the range from {0} to {1}", 0, 10)); }
            if (saturation > 10) { throw new ArgumentOutOfRangeException(nameof(saturation), string.Format("The value must be in the range from {0} to {1}", 0, 10)); }

            var sr = (1 - saturation) * 0.3086f; // lumR = 0.3086  or  0.2125
            var sg = (1 - saturation) * 0.6094f; // lumG = 0.6094  or  0.7154
            var sb = (1 - saturation) * 0.0820f; // lumB = 0.0820  or  0.0721

            return new ColorMatrix(new float[][]
               {
                     new float[] { sr + saturation, sr, sr, 0, 0 },
                     new float[] { sg, sg + saturation, sg, 0, 0 },
                     new float[] { sb, sb, sb + saturation, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Sepia)]
        public static ColorMatrix Sepia()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.393f, 0.349f, 0.272f, 0, 0 },
                     new float[] { 0.769f, 0.686f, 0.534f, 0, 0 },
                     new float[] { 0.189f, 0.168f, 0.131f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Temperature, true, 0, 0, 4)]
        public static ColorMatrix Temperature(float temperature)
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 1 + temperature, 0, 0, 0, 0 },
                     new float[] { 0, 1, 0, 0, 0 },
                     new float[] { 0, 0, 1 - temperature, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Threshold, true, 0, 0, 4)]
        public static ColorMatrix Threshold(float threshold)
        {
            if (threshold < 0) { throw new ArgumentOutOfRangeException(nameof(threshold), string.Format("The value must be in the range from {0} to {1}", 0, 4)); }
            if (threshold > 4) { throw new ArgumentOutOfRangeException(nameof(threshold), string.Format("The value must be in the range from {0} to {1}", 0, 4)); }

            var rLum = 0.3086f; // 0.212671
            var gLum = 0.6094f; // 0.715160
            var bLum = 0.0820f; // 0.072169

            return new ColorMatrix(new float[][]
               {
                     new float[] { rLum, rLum, rLum, 0, 0 },
                     new float[] { gLum, gLum, gLum, 0, 0 },
                     new float[] { bLum, bLum, bLum, 0, 0 },
                     new float[] { -1 * threshold, -1 * threshold, -1 * threshold, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Tint, true, 0, 0, 4)]
        public static ColorMatrix Tint(float tint)
        {
            if (tint < 0) { throw new ArgumentOutOfRangeException(nameof(tint), string.Format("The value must be in the range from {0} to {1}", 0, 4)); }
            if (tint > 4) { throw new ArgumentOutOfRangeException(nameof(tint), string.Format("The value must be in the range from {0} to {1}", 0, 4)); }

            return new ColorMatrix(new float[][]
               {
                     new float[] { 1 + tint, 0, 0, 0, 0 },
                     new float[] { 0, 1, 0, 0, 0 },
                     new float[] { 0, 0, 1 + tint, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Tritanomaly)]
        public static ColorMatrix Tritanomaly()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.967f, 0, 0, 0, 0 },
                     new float[] { 0.033f, 0.733f, 0.183f, 0, 0 },
                     new float[] { 0, 0, 0.267f, 0.817f, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Tritanopia)]
        public static ColorMatrix Tritanopia()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 0.95f, 0, 0, 0, 0 },
                     new float[] { 0.05f, 0.433f, 0.475f, 0, 0 },
                     new float[] { 0, 0.567f, 0.525f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.Warm)]
        public static ColorMatrix Warm()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 1.06f, 0, 0, 0, 0 },
                     new float[] { 0, 1.01f, 0, 0, 0 },
                     new float[] { 0, 0, 0.93f, 0, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }

        [ColorerMatrix(ColorerMatrixType.WhiteToAlpha)]
        public static ColorMatrix WhiteToAlpha()
        {
            return new ColorMatrix(new float[][]
               {
                     new float[] { 1, 0, 0, -1, 0 },
                     new float[] { 0, 1, 0, -1, 0 },
                     new float[] { 0, 0, 1, -1, 0 },
                     new float[] { 0, 0, 0, 1, 0 },
                     new float[] { 0, 0, 0, 0, 1 }
               });
        }
    }
}
