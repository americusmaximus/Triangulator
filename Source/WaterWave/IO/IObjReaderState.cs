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
using WaterWave.Approximations;
using WaterWave.Enums;

namespace WaterWave.IO
{
    public interface IObjReaderState
    {
        float[] BasicMatrixU { get; set; }

        float[] BasicMatrixV { get; set; }

        IApproximationTechnique CurveApproximationTechnique { get; set; }

        int DegreeU { get; set; }

        int DegreeV { get; set; }

        FreeFormElement FreeFormElement { get; set; }

        FreeFormType FreeFormType { get; set; }

        IList<string> GroupNames { get; }

        string Header { get; }

        bool IsBevelInterpolationEnabled { get; set; }

        bool IsColorInterpolationEnabled { get; set; }

        bool IsDissolveInterpolationEnabled { get; set; }

        bool IsRationalForm { get; set; }

        int LevelOfDetail { get; set; }

        long LineNumber { get; }

        string MapName { get; set; }

        string MaterialName { get; set; }

        int MergingGroupNumber { get; set; }

        Obj Obj { get; }

        string ObjectName { get; set; }

        int SmoothingGroupNumber { get; set; }

        float StepU { get; set; }

        float StepV { get; set; }

        IApproximationTechnique SurfaceApproximationTechnique { get; set; }

        void ApplyAttributesToElement(Element element);

        void ApplyAttributesToFreeFormElement(FreeFormElement element);

        void ApplyAttributesToPolygonalElement(PolygonalElement element);

        IList<Group> GetCurrentGroups();
    }
}
