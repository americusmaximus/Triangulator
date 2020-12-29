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

using ImageBox.Flipping;
using ImageBox.Splitting;
using System;
using System.Drawing;

namespace Triangulator
{
    public class Request
    {
        public Request()
        {
            Color = Color.Transparent;

            IgnoreTransparent = true;

            MinimumHeightValue = 0;
            MaximumHeightValue = 100;

            ScaleXValue = 1;
            ScaleZValue = 1;

            SplitXValue = 1;
            SplitYValue = 1;
        }

        public virtual float Angle { get; set; }

        public virtual Color Color { get; set; }

        public virtual FlipType FlipType { get; set; }

        public virtual bool IgnoreTransparent { get; set; }

        public virtual float MaximumHeight
        {
            get { return MaximumHeightValue; }
            set
            {
                if (value <= MinimumHeight) { throw new InvalidOperationException(string.Format("{0} has to be greater than {1}.", nameof(MaximumHeight), nameof(MinimumHeight))); }

                MaximumHeightValue = value;
            }
        }

        public virtual float MinimumHeight
        {
            get { return MinimumHeightValue; }
            set
            {
                if (MaximumHeight <= value) { throw new InvalidOperationException(string.Format("{0} has to be less than {1}.", nameof(MinimumHeight), nameof(MaximumHeight))); }

                MinimumHeightValue = value;
            }
        }

        public virtual float OffsetX { get; set; }

        public virtual float OffsetZ { get; set; }

        public virtual float ScaleX
        {
            get { return ScaleXValue; }
            set
            {
                if (value <= 0) { throw new InvalidOperationException(string.Format("{0} has to be a positive value.", nameof(ScaleX))); }

                ScaleXValue = value;
            }
        }

        public virtual float ScaleZ
        {
            get { return ScaleZValue; }
            set
            {
                if (value <= 0) { throw new InvalidOperationException(string.Format("{0} has to be a positive value.", nameof(ScaleZ))); }

                ScaleZValue = value;
            }
        }

        public virtual SplitType SplitType { get; set; }

        public virtual int SplitX
        {
            get { return SplitXValue; }
            set
            {
                if (value < 1) { throw new InvalidOperationException(string.Format("Minimum value for {0} is 1.", nameof(SplitX))); }

                SplitXValue = value;
            }
        }

        public virtual int SplitY
        {
            get { return SplitYValue; }
            set
            {
                if (value < 1) { throw new InvalidOperationException(string.Format("Minimum value for {0} is 1.", nameof(SplitY))); }

                SplitYValue = value;
            }
        }

        protected virtual float MaximumHeightValue { get; set; }

        protected virtual float MinimumHeightValue { get; set; }

        protected virtual float ScaleXValue { get; set; }

        protected virtual float ScaleZValue { get; set; }

        protected virtual int SplitXValue { get; set; }

        protected virtual int SplitYValue { get; set; }
    }
}
