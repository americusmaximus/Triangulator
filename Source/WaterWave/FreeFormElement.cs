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

using System.Collections.Generic;
using WaterWave.Enums;

namespace WaterWave
{
    public abstract class FreeFormElement : Element
    {
        public FreeFormElement()
        {
            InnerTrimmingCurves = new List<CurveIndex>();
            OuterTrimmingCurves = new List<CurveIndex>();
            SequenceCurves = new List<CurveIndex>();
            SpecialPoints = new List<int>();
            U = new List<float>();
            V = new List<float>();
        }

        public virtual float[] BasicMatrixU { get; set; }

        public virtual float[] BasicMatrixV { get; set; }

        public virtual IObjApproximationTechnique CurveApproximationTechnique { get; set; }

        public virtual int DegreeU { get; set; }

        public virtual int DegreeV { get; set; }

        public virtual FreeFormType FreeFormType { get; set; }

        public virtual IList<CurveIndex> InnerTrimmingCurves { get; protected set; }

        public virtual bool IsRationalForm { get; set; }

        public virtual int MergingGroupNumber { get; set; }

        public virtual IList<CurveIndex> OuterTrimmingCurves { get; protected set; }

        public virtual IList<CurveIndex> SequenceCurves { get; protected set; }

        public virtual IList<int> SpecialPoints { get; protected set; }

        public virtual float StepU { get; set; }

        public virtual float StepV { get; set; }

        public virtual IObjApproximationTechnique SurfaceApproximationTechnique { get; set; }

        public virtual IList<float> U { get; protected set; }

        public virtual IList<float> V { get; protected set; }
    }
}