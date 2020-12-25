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

namespace ImageBox.Coloring
{
    internal class ColorerMatrixAttribute : Attribute
    {
        public ColorerMatrixAttribute(ColorerMatrixType type) : this(type, false) { }

        public ColorerMatrixAttribute(ColorerMatrixType type, bool isValueRequired, float defaultValue = 0, float minimumValue = 0, float maximumValue = 0)
        {
            Type = type;
            IsValueRequired = isValueRequired;

            if (defaultValue < minimumValue) { throw new ArgumentException(string.Format("The value of {0} cannot be smaller than the value of {1}.", nameof(defaultValue), nameof(minimumValue)), nameof(defaultValue)); }
            if (maximumValue < defaultValue) { throw new ArgumentException(string.Format("The value of {0} cannot be smaller than the value of {1}.", nameof(maximumValue), nameof(defaultValue)), nameof(defaultValue)); }

            DefaultValueValue = defaultValue;
            MinimumValueValue = minimumValue;
            MaximumValueValue = maximumValue;
        }

        public float DefaultValue
        {
            get
            {
                if (!IsValueRequired)
                {
                    throw new InvalidOperationException(string.Format("Accessing {0} is illegal when {1} is set to TRUE.", nameof(DefaultValue), nameof(IsValueRequired)));
                }

                return DefaultValueValue;
            }
        }

        public bool IsValueRequired { get; protected set; }

        public float MaximumValue
        {
            get
            {
                if (!IsValueRequired)
                {
                    throw new InvalidOperationException(string.Format("Accessing {0} is illegal when {1} is set to TRUE.", nameof(MaximumValue), nameof(IsValueRequired)));
                }

                return MaximumValueValue;
            }
        }

        public float MinimumValue
        {
            get
            {
                if (!IsValueRequired)
                {
                    throw new InvalidOperationException(string.Format("Accessing {0} is illegal when {1} is set to TRUE.", nameof(MinimumValue), nameof(IsValueRequired)));
                }

                return MinimumValueValue;
            }
        }

        public ColorerMatrixType Type { get; protected set; }

        protected float DefaultValueValue { get; set; }

        protected float MaximumValueValue { get; set; }

        protected float MinimumValueValue { get; set; }
    }
}
