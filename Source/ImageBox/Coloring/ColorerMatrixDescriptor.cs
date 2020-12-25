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

namespace ImageBox.Coloring
{
    public class ColorerMatrixDescriptor
    {
        public ColorerMatrixDescriptor(Func<float, ColorMatrix> action, float? value = null, float? minimumValue = null, float? maximumValue = null)
        {
            Action = action;

            if (value.HasValue)
            {
                IsValueRequired = true;
                ValueValue = value.Value;
            }

            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
        }

        public virtual bool IsValueRequired { get; protected set; }

        public virtual ColorMatrix Matrix
        {
            get
            {
                return Action(ValueValue);
            }
        }

        public virtual float? MaximumValue { get; protected set; }

        public virtual float? MinimumValue { get; protected set; }

        public virtual float Value
        {
            get
            {
                if (!IsValueRequired)
                {
                    throw new InvalidOperationException(string.Format("Accessing {0} is illegal when {1} is set to TRUE.", nameof(Value), nameof(IsValueRequired)));
                }

                return ValueValue;
            }
            set
            {
                if (!IsValueRequired)
                {
                    throw new InvalidOperationException(string.Format("Accessing {0} is illegal when {1} is set to TRUE.", nameof(Value), nameof(IsValueRequired)));
                }

                ValueValue = value;
            }
        }

        protected virtual Func<float, ColorMatrix> Action { get; set; }

        protected virtual float ValueValue { get; set; }
    }
}
