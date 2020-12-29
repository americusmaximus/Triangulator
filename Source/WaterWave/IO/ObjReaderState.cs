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
using System.Collections.Generic;
using System.Linq;
using WaterWave.Approximations;
using WaterWave.Enums;

namespace WaterWave.IO
{
    public class ObjReaderState : IObjReaderState
    {
        public ObjReaderState(Obj obj, ILineReader reader)
        {
            Obj = obj ?? throw new ArgumentNullException(nameof(obj));
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));

            GroupNames = new List<string>();
        }

        public virtual float[] BasicMatrixU { get; set; }

        public virtual float[] BasicMatrixV { get; set; }

        public virtual IApproximationTechnique CurveApproximationTechnique { get; set; }

        public virtual int DegreeU { get; set; }

        public virtual int DegreeV { get; set; }

        public virtual FreeFormElement FreeFormElement { get; set; }

        public virtual FreeFormType FreeFormType { get; set; }

        public virtual IList<string> GroupNames { get; protected set; }

        public virtual string Header { get { return string.Join(Environment.NewLine, Reader.Headers.ToArray()); } }

        public virtual bool IsBevelInterpolationEnabled { get; set; }

        public virtual bool IsColorInterpolationEnabled { get; set; }

        public virtual bool IsDissolveInterpolationEnabled { get; set; }

        public virtual bool IsRationalForm { get; set; }

        public virtual int LevelOfDetail { get; set; }

        public virtual long LineNumber { get { return Reader.LineNumber; } }

        public virtual string MapName { get; set; }

        public virtual string MaterialName { get; set; }

        public virtual int MergingGroupNumber { get; set; }

        public virtual Obj Obj { get; protected set; }

        public virtual string ObjectName { get; set; }

        public virtual int SmoothingGroupNumber { get; set; }

        public virtual float StepU { get; set; }

        public virtual float StepV { get; set; }

        public virtual IApproximationTechnique SurfaceApproximationTechnique { get; set; }

        protected virtual ILineReader Reader { get; set; }

        public virtual void ApplyAttributesToElement(Element element)
        {
            element.ObjectName = ObjectName;
            element.LevelOfDetail = LevelOfDetail;
            element.MapName = MapName;
            element.MaterialName = MaterialName;
        }

        public virtual void ApplyAttributesToFreeFormElement(FreeFormElement element)
        {
            element.MergingGroupNumber = MergingGroupNumber;
            element.FreeFormType = FreeFormType;
            element.IsRationalForm = IsRationalForm;
            element.DegreeU = DegreeU;
            element.DegreeV = DegreeV;
            element.BasicMatrixU = BasicMatrixU;
            element.BasicMatrixV = BasicMatrixV;
            element.StepU = StepU;
            element.StepV = StepV;
            element.CurveApproximationTechnique = CurveApproximationTechnique;
            element.SurfaceApproximationTechnique = SurfaceApproximationTechnique;
        }

        public virtual void ApplyAttributesToPolygonalElement(PolygonalElement element)
        {
            element.SmoothingGroupNumber = SmoothingGroupNumber;
            element.IsBevelInterpolationEnabled = IsBevelInterpolationEnabled;
            element.IsColorInterpolationEnabled = IsColorInterpolationEnabled;
            element.IsDissolveInterpolationEnabled = IsDissolveInterpolationEnabled;
        }

        public virtual IList<Group> GetCurrentGroups()
        {
            var groups = new List<Group>();

            foreach (var name in GroupNames)
            {
                var group = Obj.Groups.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.Ordinal));

                if (group == default)
                {
                    group = new Group(name);
                    Obj.Groups.Add(group);
                }

                groups.Add(group);
            }

            if (groups.Count == 0)
            {
                groups.Add(Obj.DefaultGroup);
            }

            return groups;
        }
    }
}