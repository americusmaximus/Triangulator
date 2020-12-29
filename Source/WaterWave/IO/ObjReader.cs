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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using WaterWave.Approximations;
using WaterWave.Enums;
using WaterWave.IO.Attributes;
using WaterWave.IO.Enums;

namespace WaterWave.IO
{
    public class ObjReader : IObjReader
    {
        public ObjReader()
        {
            Initialize();
        }

        protected virtual Dictionary<ObjToken, Action<IObjReaderState, ObjToken, string[]>> Executors { get; set; }

        public virtual Obj Read(Stream stream)
        {
            var obj = new Obj();
            var reader = new LineReader();
            var state = new ObjReaderState(obj, reader);

            foreach (var values in reader.Read(stream))
            {
                if (values.Length == 0) { continue; }

                if (!Enum.TryParse<ObjToken>(values[0], true, out var token)) { throw new InvalidDataException(string.Format("Unable to parse token <{0}>. Line: {1}.", token, state.LineNumber)); }

                if (!Executors.TryGetValue(token, out var executor)) { throw new InvalidOperationException(string.Format("Unable to find an executor for token <{0}>.", token)); }

                executor.Invoke(state, token, values);
            }

            obj.Header = state.Header;

            return obj;
        }

        protected virtual IApproximationTechnique ApproximationTechnique(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a technique. Line: {1}.", token, state.LineNumber)); }

            if(!Enum.TryParse<ApproximationTechniqueToken>(values[1], true, out var approximationToken)) { throw new InvalidDataException(string.Format("The <{0}> statement contains an unknown technique <{1}>.", token, values[1])); }

            switch (approximationToken)
            {
                case ApproximationTechniqueToken.CParm:
                    {
                        if (values.Length < 3) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement must specify a value. Line: {2}.", token, approximationToken, state.LineNumber)); }
                        if (values.Length != 3) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement has too many values. Line: {2}.", token, approximationToken, state.LineNumber)); }

                        return new ConstantParametricSubDivisionTechnique(float.Parse(values[2], CultureInfo.InvariantCulture));
                    }
                case ApproximationTechniqueToken.CParmA:
                    {
                        if (values.Length < 4) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement must specify a value. Line: {2}.", token, approximationToken, state.LineNumber)); }
                        if (values.Length != 4) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement has too many values. Line: {2}.", token, approximationToken, state.LineNumber)); }

                        return new ConstantParametricSubDivisionTechnique(float.Parse(values[2], CultureInfo.InvariantCulture), float.Parse(values[3], CultureInfo.InvariantCulture));
                    }
                case ApproximationTechniqueToken.CParmB:
                    {
                        if (values.Length < 3) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement must specify a value. Line: {2}.", token, approximationToken, state.LineNumber)); }
                        if (values.Length != 3) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement has too many values. Line: {2}.", token, approximationToken, state.LineNumber)); }

                        return new ConstantParametricSubDivisionTechnique(float.Parse(values[2], CultureInfo.InvariantCulture));
                    }
                case ApproximationTechniqueToken.CSpace:
                    {
                        if (values.Length < 3) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement must specify a value. Line: {2}.", token, approximationToken, state.LineNumber)); }
                        if (values.Length != 3) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement has too many values. Line: {2}.", token, approximationToken, state.LineNumber)); }

                        return new ConstantSpatialSubDivisionTechnique(float.Parse(values[2], CultureInfo.InvariantCulture));
                    }
                case ApproximationTechniqueToken.Curv:
                    {
                        if (values.Length < 4) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement must specify a value. Line: {2}.", token, approximationToken, state.LineNumber)); }
                        if (values.Length != 4) { throw new InvalidDataException(string.Format("The <{0}> <{1}> statement has too many values. Line: {2}.", token, approximationToken, state.LineNumber)); }

                        return new CurvatureDependentSubDivisionTechnique(float.Parse(values[2], CultureInfo.InvariantCulture), float.Parse(values[3], CultureInfo.InvariantCulture));
                    }
            }

            throw new InvalidDataException(string.Format("The <{0}> statement contains an unknown technique <{1}>.", token, values[1]));
        }

        [ObjToken(ObjToken.Bevel)]
        protected virtual void Bevel(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a name. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            if (!Enum.TryParse<OnOff>(values[1], true, out var onOff)) { throw new InvalidDataException(string.Format("The <{0}> statement must specify on or off. Line: {1}.", token, state.LineNumber)); }

            state.IsBevelInterpolationEnabled = onOff == OnOff.On;
        }

        [ObjToken(ObjToken.BMat)]
        protected virtual void BMat(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a direction. Line: {1}.", token, state.LineNumber)); }

            if (!Enum.TryParse<UV>(values[1], true, out var uv)) { throw new InvalidDataException(string.Format("The <{0}> statement has an unknown direction. Line: {1}.", token, state.LineNumber)); }

            var count = (state.DegreeU + 1) * (state.DegreeV + 1);

            if (values.Length != count + 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many or too few values. Line: {1}.", token, state.LineNumber)); }

            var matrix = new float[count];

            for (int i = 0; i < count; i++)
            {
                matrix[i] = float.Parse(values[2 + i], CultureInfo.InvariantCulture);
            }

            switch (uv)
            {
                case UV.U:
                    {
                        state.BasicMatrixU = matrix;
                        break;
                    }
                case UV.V:
                    {
                        state.BasicMatrixV = matrix;
                        break;
                    }
            }
        }

        [ObjToken(ObjToken.Bsp)]
        protected virtual void Bsp(IObjReaderState state, ObjToken token, string[] values)
        {
            throw new NotImplementedException(string.Format("The <{0}> statement have been replaced by free-form geometry statements.", token));
        }

        [ObjToken(ObjToken.Bzp)]
        protected virtual void Bzp(IObjReaderState state, ObjToken token, string[] values)
        {
            throw new NotImplementedException(string.Format("The <{0}> statement have been replaced by free-form geometry statements.", token));
        }

        [ObjToken(ObjToken.Cdc)]
        protected virtual void Cdc(IObjReaderState state, ObjToken token, string[] values)
        {
            throw new NotImplementedException(string.Format("The <{0}> statement have been replaced by free-form geometry statements.", token));
        }

        [ObjToken(ObjToken.Cdp)]
        protected virtual void Cdp(IObjReaderState state, ObjToken token, string[] values)
        {
            throw new NotImplementedException(string.Format("The <{0}> statement have been replaced by free-form geometry statements.", token));
        }

        [ObjToken(ObjToken.C_Interp)]
        protected virtual void CInterp(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a name. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            if (!Enum.TryParse<OnOff>(values[1], true, out var onOff)) { throw new InvalidDataException(string.Format("The <{0}> statement must specify on or off. Line: {1}.", token, state.LineNumber)); }

            state.IsColorInterpolationEnabled = onOff == OnOff.On;
        }

        [ObjToken(ObjToken.Con)]
        protected virtual void Con(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 9) { throw new InvalidDataException(string.Format("The <{0}> statement must specify 8 values. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 9) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            var surface1 = int.Parse(values[1], CultureInfo.InvariantCulture);

            if (surface1 == 0) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a surface index. Line: {1}.", token, state.LineNumber)); }

            if (surface1 < 0)
            {
                surface1 = state.Obj.Surfaces.Count + surface1 + 1;
            }

            if (surface1 <= 0 || surface1 > state.Obj.Surfaces.Count) { throw new IndexOutOfRangeException(); }

            var curve1 = CurveIndex(state, values, 2);

            var surface2 = int.Parse(values[5], CultureInfo.InvariantCulture);

            if (surface2 == 0) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a surface index. Line: {1}.", token, state.LineNumber)); }

            if (surface2 < 0)
            {
                surface2 = state.Obj.Surfaces.Count + surface2 + 1;
            }

            if (surface2 <= 0 || surface2 > state.Obj.Surfaces.Count) { throw new IndexOutOfRangeException(); }

            var curve2 = CurveIndex(state, values, 6);

            state.Obj.SurfaceConnections.Add(new SurfaceConnection
            {
                Surface1 = surface1,
                Curve2D1 = curve1,
                Surface2 = surface2,
                Curve2D2 = curve2
            });
        }

        [ObjToken(ObjToken.CSType)]
        protected virtual void CSType(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a value. Line: {1}.", token, state.LineNumber)); }

            var type = string.Empty;

            if (values.Length == 2)
            {
                state.IsRationalForm = false;
                type = values[1];
            }
            else if (values.Length == 3 && string.Equals(values[1], "rat", StringComparison.OrdinalIgnoreCase))
            {
                state.IsRationalForm = true;
                type = values[2];
            }
            else
            {
                throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber));
            }

            if (!Enum.TryParse<CurveSurfaceToken>(type, true, out var cstype)) { throw new InvalidDataException(string.Format("The <{0}> statement has an unknown type value. Value: {1}. Line: {2}.", token, type, state.LineNumber)); }

            switch (cstype)
            {
                case CurveSurfaceToken.BMatrix:
                    {
                        state.FreeFormType = FreeFormType.BasisMatrix;
                        break;
                    }
                case CurveSurfaceToken.Bezier:
                    {
                        state.FreeFormType = FreeFormType.Bezier;
                        break;
                    }
                case CurveSurfaceToken.Bspline:
                    {
                        state.FreeFormType = FreeFormType.BSpline;
                        break;
                    }
                case CurveSurfaceToken.Cardinal:
                    {
                        state.FreeFormType = FreeFormType.Cardinal;
                        break;
                    }
                case CurveSurfaceToken.Taylor:
                    {
                        state.FreeFormType = FreeFormType.Taylor;
                        break;
                    }
            }
        }

        [ObjToken(ObjToken.CTech)]
        protected virtual void CTech(IObjReaderState state, ObjToken token, string[] values)
        {
            state.CurveApproximationTechnique = ApproximationTechnique(state, token, values);
        }

        [ObjToken(ObjToken.Curv)]
        protected virtual void Curv(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 5) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 4 values. Line: {1}.", token, state.LineNumber)); }

            var curve = new Curve()
            {
                Start = float.Parse(values[1], CultureInfo.InvariantCulture),
                End = float.Parse(values[2], CultureInfo.InvariantCulture)
            };

            for (int i = 3; i < values.Length; i++)
            {
                var v = int.Parse(values[i], CultureInfo.InvariantCulture);

                if (v == 0) { throw new InvalidDataException(string.Format("The <{0}> statement contains an invalid vertex index. Line: {1}.", token, state.LineNumber)); }

                if (v < 0)
                {
                    v = state.Obj.Vertices.Count + v + 1;
                }

                if (v <= 0 || v > state.Obj.Vertices.Count) { throw new IndexOutOfRangeException(); }

                curve.Vertices.Add(v);
            }

            state.ApplyAttributesToElement(curve);
            state.ApplyAttributesToFreeFormElement(curve);
            state.FreeFormElement = curve;

            state.Obj.Curves.Add(curve);

            foreach (var group in state.GetCurrentGroups())
            {
                group.Curves.Add(curve);
            }

        }

        [ObjToken(ObjToken.Curv2)]
        protected virtual void Curv2(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 3) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 2 values. Line: {1}.", token, state.LineNumber)); }

            var curve = new Curve2D();

            for (int i = 1; i < values.Length; i++)
            {
                int vp = int.Parse(values[i], CultureInfo.InvariantCulture);

                if (vp == 0) { throw new InvalidDataException(string.Format("The <{0}> statement contains an invalid parameter space vertex index. Line: {1}.", token, state.LineNumber)); }

                if (vp < 0)
                {
                    vp = state.Obj.SpaceVertices.Count + vp + 1;
                }

                if (vp <= 0 || vp > state.Obj.SpaceVertices.Count) { throw new IndexOutOfRangeException(); }

                curve.SpaceVertices.Add(vp);
            }

            state.ApplyAttributesToElement(curve);
            state.ApplyAttributesToFreeFormElement(curve);
            state.FreeFormElement = curve;

            state.Obj.Curves2D.Add(curve);

            foreach (var group in state.GetCurrentGroups())
            {
                group.Curves2D.Add(curve);
            }
        }

        protected virtual void CurveIndex(IList<CurveIndex> curves, IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 4) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 3 value. Line: {1}.", token, state.LineNumber)); }
            if ((values.Length - 1) % 3 != 0) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            for (int i = 1; i < values.Length; i += 3)
            {
                curves.Add(CurveIndex(state, values, i));
            }
        }

        protected virtual CurveIndex CurveIndex(IObjReaderState state, string[] values, int index)
        {
            var start = float.Parse(values[index], CultureInfo.InvariantCulture);
            var end = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
            var curve2D = int.Parse(values[index + 2], CultureInfo.InvariantCulture);

            if (curve2D == 0) { throw new InvalidDataException(string.Format("The curve index must specify an index. Line: {0}.", state.LineNumber)); }

            if (curve2D < 0)
            {
                curve2D = state.Obj.Curves2D.Count + curve2D + 1;
            }

            if (curve2D <= 0 || curve2D > state.Obj.Curves2D.Count) { throw new IndexOutOfRangeException(); }

            return new CurveIndex(start, end, curve2D);
        }

        [ObjToken(ObjToken.Deg)]
        protected virtual void Deg(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 1 value. Line: {1}.", token, state.LineNumber)); }

            if (values.Length == 2)
            {
                state.DegreeU = int.Parse(values[1], CultureInfo.InvariantCulture);
                state.DegreeV = 0;
            }
            else if (values.Length == 3)
            {
                state.DegreeU = int.Parse(values[1], CultureInfo.InvariantCulture);
                state.DegreeV = int.Parse(values[2], CultureInfo.InvariantCulture);
            }
            else
            {
                throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}", token, state.LineNumber));
            }
        }

        [ObjToken(ObjToken.D_Interp)]
        protected virtual void DInterp(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a name. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            if (!Enum.TryParse<OnOff>(values[1], true, out var onOff)) { throw new InvalidDataException(string.Format("The <{0}> statement must specify on or off. Line: {1}.", token, state.LineNumber)); }

            state.IsDissolveInterpolationEnabled = onOff == OnOff.On;
        }

        [ObjToken(ObjToken.End)]
        protected virtual void End(IObjReaderState state, ObjToken token, string[] values)
        {
            state.FreeFormElement = default;
        }

        [ObjToken(ObjToken.F)]
        [ObjToken(ObjToken.FO)]
        protected virtual void F(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 4) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 3 values. Line: {1}.", token, state.LineNumber)); }

            var face = new Face();

            for (int i = 1; i < values.Length; i++)
            {
                face.Vertices.Add(Triplet(state, token, values[i]));
            }

            state.ApplyAttributesToElement(face);
            state.ApplyAttributesToPolygonalElement(face);

            state.Obj.Faces.Add(face);

            foreach (var group in state.GetCurrentGroups())
            {
                group.Faces.Add(face);
            }
        }

        [ObjToken(ObjToken.G)]
        protected virtual void G(IObjReaderState state, ObjToken token, string[] values)
        {
            state.GroupNames.Clear();

            for (int i = 1; i < values.Length; i++)
            {
                var name = values[i];

                if (!string.Equals(name, "default", StringComparison.OrdinalIgnoreCase))
                {
                    state.GroupNames.Add(name);
                }
            }

            state.GetCurrentGroups();
        }

        [ObjToken(ObjToken.Hole)]
        protected virtual void Hole(IObjReaderState state, ObjToken token, string[] values)
        {
            if (state.FreeFormElement == default) { return; }

            CurveIndex(state.FreeFormElement.InnerTrimmingCurves, state, token, values);
        }

        protected virtual void Initialize()
        {
            Executors = new Dictionary<ObjToken, Action<IObjReaderState, ObjToken, string[]>>(Enum.GetValues(typeof(ObjToken)).Length);

            foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attrs = method.GetCustomAttributes(typeof(ObjTokenAttribute), true).OfType<ObjTokenAttribute>().ToArray();

                foreach (var attr in attrs)
                {
                    if (Executors.ContainsKey(attr.Token)) { throw new InvalidOperationException(string.Format("Illegal attempt to to register method <{0}> as an executor for token <{1}>.", method.Name, attr.Token)); }

                    Executors.Add(attr.Token, (s, t, v) => method.Invoke(this, new object[] { s, t, v }));
                }
            }
        }

        [ObjToken(ObjToken.L)]
        protected virtual void L(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 3) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 2 values. Line: {1}.", token, state.LineNumber)); }

            var line = new Line();

            for (int i = 1; i < values.Length; i++)
            {
                line.Vertices.Add(Triplet(state, token, values[i]));
            }

            state.ApplyAttributesToElement(line);
            state.ApplyAttributesToPolygonalElement(line);

            state.Obj.Lines.Add(line);

            foreach (var group in state.GetCurrentGroups())
            {
                group.Lines.Add(line);
            }
        }

        [ObjToken(ObjToken.LOD)]
        protected void LOD(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a value. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            state.LevelOfDetail = int.Parse(values[1], CultureInfo.InvariantCulture);
        }

        [ObjToken(ObjToken.MapLib)]
        protected virtual void MapLib(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a file name. Line: {1}.", token, state.LineNumber)); }

            for (int i = 1; i < values.Length; i++)
            {
                if (!Path.HasExtension(values[1])) { throw new InvalidDataException(string.Format("A file name must have an extension. Value: <{0}>. Line: {1}.", values[1], state.LineNumber)); }

                state.Obj.MapLibraries.Add(values[i]);
            }
        }

        [ObjToken(ObjToken.MG)]
        protected virtual void MG(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a value. Line: {1}.", token, state.LineNumber)); }

            if (Enum.TryParse<OnOff>(values[1], true, out var onOff))
            {
                if (onOff == OnOff.Off)
                {
                    state.MergingGroupNumber = 0;
                }
            }
            else
            {
                state.MergingGroupNumber = int.Parse(values[1], CultureInfo.InvariantCulture);
            }

            if (state.MergingGroupNumber == 0)
            {
                if (values.Length > 3) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }
            }
            else
            {
                if (values.Length != 3) { throw new InvalidDataException(string.Format("The <{0}> statement has too many or too few values. Line: {1}.", token, state.LineNumber)); }

                var res = float.Parse(values[2], CultureInfo.InvariantCulture);

                state.Obj.MergingGroupResolutions[state.MergingGroupNumber] = res;
            }
        }

        [ObjToken(ObjToken.MtlLib)]
        protected virtual void MtlLib(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a file name. Line: {1}.", token, state.LineNumber)); }

            for (int i = 1; i < values.Length; i++)
            {
                if (!Path.HasExtension(values[1])) { throw new InvalidDataException(string.Format("A file name must have an extension. Value: <{0}>. Line: {1}.", values[1], state.LineNumber)); }

                state.Obj.MaterialLibraries.Add(values[i]);
            }

        }

        [ObjToken(ObjToken.O)]
        protected virtual void O(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length == 1)
            {
                state.ObjectName = default;
                return;
            }

            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            state.ObjectName = values[1];
        }

        [ObjToken(ObjToken.P)]
        protected virtual void P(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 1 value. Line: {1}.", token, state.LineNumber)); }

            var point = new Point();

            for (int i = 1; i < values.Length; i++)
            {
                point.Vertices.Add(Triplet(state, token, values[i]));
            }

            state.ApplyAttributesToElement(point);
            state.ApplyAttributesToPolygonalElement(point);

            state.Obj.Points.Add(point);

            foreach (var group in state.GetCurrentGroups())
            {
                group.Points.Add(point);
            }

        }

        [ObjToken(ObjToken.Parm)]
        protected virtual void Parm(IObjReaderState state, ObjToken token, string[] values)
        {
            if (state.FreeFormElement == default) { return; }

            if (values.Length < 4) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 3 values. Line: {1}.", token, state.LineNumber)); }

            var parameters = default(IList<float>);

            if (!Enum.TryParse<UV>(values[1], true, out var uv)) { throw new InvalidDataException(string.Format("The <{0}> statement has an unknown direction. Line: {1}.", token, state.LineNumber)); }

            switch (uv)
            {
                case UV.U:
                    {
                        parameters = state.FreeFormElement.U;
                        break;
                    }
                case UV.V:
                    {
                        parameters = state.FreeFormElement.V;
                        break;
                    }
            }

            for (int i = 2; i < values.Length; i++)
            {
                parameters.Add(float.Parse(values[i], CultureInfo.InvariantCulture));
            }
        }

        [ObjToken(ObjToken.Res)]
        protected virtual void Res(IObjReaderState state, ObjToken token, string[] values)
        {
            throw new NotImplementedException(string.Format("The <{0}> statement have been replaced by free-form geometry statements.", token));

        }

        [ObjToken(ObjToken.S)]
        protected virtual void S(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a value. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            if (Enum.TryParse<OnOff>(values[1], true, out var onOff))
            {
                if (onOff == OnOff.Off)
                {
                    state.SmoothingGroupNumber = 0;
                    return;
                }
            }

            state.SmoothingGroupNumber = int.Parse(values[1], CultureInfo.InvariantCulture);
        }

        [ObjToken(ObjToken.Scrv)]
        protected virtual void Scrv(IObjReaderState state, ObjToken token, string[] values)
        {
            if (state.FreeFormElement == default) { return; }

            CurveIndex(state.FreeFormElement.SequenceCurves, state, token, values);
        }

        [ObjToken(ObjToken.Shadow_Obj)]
        protected virtual void ShadowObj(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a file name. Line: {1}", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }
            if (!Path.HasExtension(values[1])) { throw new InvalidDataException(string.Format("A file name must have an extension. Value: <{0}>. Line: {1}.", values[1], state.LineNumber)); }

            state.Obj.ShadowObjectFileName = values[1];
        }

        [ObjToken(ObjToken.Sp)]
        protected virtual void SP(IObjReaderState state, ObjToken token, string[] values)
        {
            if (state.FreeFormElement == default) { return; }

            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 1 value. Line: {1}.", token, state.LineNumber)); }

            for (int i = 1; i < values.Length; i++)
            {
                var vp = int.Parse(values[i], CultureInfo.InvariantCulture);

                if (vp == 0) { throw new InvalidDataException(string.Format("The <{0}> statement contains an invalid parameter space vertex index. Line: {1}.", token, state.LineNumber)); }

                if (vp < 0)
                {
                    vp = state.Obj.SpaceVertices.Count + vp + 1;
                }

                if (vp <= 0 || vp > state.Obj.SpaceVertices.Count) { throw new IndexOutOfRangeException(); }

                state.FreeFormElement.SpecialPoints.Add(vp);
            }
        }

        [ObjToken(ObjToken.STech)]
        protected virtual void STech(IObjReaderState state, ObjToken token, string[] values)
        {
            state.SurfaceApproximationTechnique = ApproximationTechnique(state, token, values);
        }

        [ObjToken(ObjToken.Step)]
        protected virtual void Step(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 1 value. Line: {1}.", token, state.LineNumber)); }

            if (values.Length == 2)
            {
                state.StepU = float.Parse(values[1], CultureInfo.InvariantCulture);
                state.StepV = 1.0f;
            }
            else if (values.Length == 3)
            {
                state.StepU = float.Parse(values[1], CultureInfo.InvariantCulture);
                state.StepV = float.Parse(values[2], CultureInfo.InvariantCulture);
            }
            else
            {
                throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber));
            }
        }

        [ObjToken(ObjToken.Surf)]
        protected virtual void Surf(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 6) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 5 values. Line: {1}.", token, state.LineNumber)); }

            var surface = new Surface()
            {

                StartU = float.Parse(values[1], CultureInfo.InvariantCulture),
                EndU = float.Parse(values[2], CultureInfo.InvariantCulture),
                StartV = float.Parse(values[3], CultureInfo.InvariantCulture),
                EndV = float.Parse(values[4], CultureInfo.InvariantCulture)
            };

            for (int i = 5; i < values.Length; i++)
            {
                surface.Vertices.Add(Triplet(state, token, values[i]));
            }

            state.ApplyAttributesToElement(surface);
            state.ApplyAttributesToFreeFormElement(surface);
            state.FreeFormElement = surface;

            state.Obj.Surfaces.Add(surface);

            foreach (var group in state.GetCurrentGroups())
            {
                group.Surfaces.Add(surface);
            }
        }

        [ObjToken(ObjToken.Trace_Obj)]
        protected virtual void TraceObj(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a file name. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }
            if (!Path.HasExtension(values[1])) { throw new InvalidDataException(string.Format("A file name must have an extension. Value: <{0}>. Line: {1}.", values[1], state.LineNumber)); }

            state.Obj.TraceObjectFileName = values[1];
        }

        [ObjToken(ObjToken.Trim)]
        protected virtual void Trim(IObjReaderState state, ObjToken token, string[] values)
        {
            if (state.FreeFormElement == default) { return; }

            CurveIndex(state.FreeFormElement.OuterTrimmingCurves, state, token, values);
        }

        protected virtual Triplet Triplet(IObjReaderState state, ObjToken token, string value)
        {
            var values = value.Split('/');

            if (values.Length > 3) { throw new InvalidDataException(string.Format("The <{0}> has too many values. Line: {1}.", token, state.LineNumber)); }

            var v = !string.IsNullOrEmpty(values[0]) ? int.Parse(values[0], CultureInfo.InvariantCulture) : 0;

            if (v == 0) { throw new InvalidDataException(string.Format("The <{0}> must specify a vertex index. Line: {1}.", token, state.LineNumber)); }

            if (v < 0)
            {
                v = state.Obj.Vertices.Count + v + 1;
            }

            if (v <= 0 || v > state.Obj.Vertices.Count) { throw new IndexOutOfRangeException(); }

            var vt = values.Length > 1 && !string.IsNullOrEmpty(values[1]) ? int.Parse(values[1], CultureInfo.InvariantCulture) : 0;

            if (vt != 0)
            {
                if (vt < 0)
                {
                    vt = state.Obj.TextureVertices.Count + vt + 1;
                }

                if (vt <= 0 || vt > state.Obj.TextureVertices.Count) { throw new IndexOutOfRangeException(); }
            }

            var vn = values.Length > 2 && !string.IsNullOrEmpty(values[2]) ? int.Parse(values[2], CultureInfo.InvariantCulture) : 0;

            if (vn != 0)
            {
                if (vn < 0)
                {
                    vn = state.Obj.VertexNormals.Count + vn + 1;
                }

                if (vn <= 0 || vn > state.Obj.VertexNormals.Count) { throw new IndexOutOfRangeException(); }
            }

            return new Triplet(v, vt, vn);
        }

        [ObjToken(ObjToken.UseMap)]
        protected virtual void UseMap(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a value. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            if (Enum.TryParse<OnOff>(values[1], true, out var onOff))
            {
                if (onOff == OnOff.Off)
                {
                    state.MapName = default;
                    return;
                }
            }

            state.MapName = values[1];
        }

        [ObjToken(ObjToken.UseMtl)]
        protected virtual void UseMtl(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a value. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            if (Enum.TryParse<OnOff>(values[1], true, out var onOff))
            {
                if (onOff == OnOff.Off)
                {
                    state.MaterialName = default;
                    return;
                }
            }

            state.MaterialName = values[1];
        }

        [ObjToken(ObjToken.V)]
        protected virtual void V(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 4) { throw new InvalidDataException(string.Format("The <{0}> statement must have at least 3 values. Line: {1}.", token, state.LineNumber)); }

            var x = float.Parse(values[1], CultureInfo.InvariantCulture);
            var y = float.Parse(values[2], CultureInfo.InvariantCulture);
            var z = float.Parse(values[3], CultureInfo.InvariantCulture);

            var w = 1.0f;
            var hasColor = false;

            var r = 0.0f;
            var g = 0.0f;
            var b = 0.0f;
            var a = 1.0f;

            if (values.Length == 4 || values.Length == 5)
            {
                if (values.Length == 5)
                {
                    w = float.Parse(values[4], CultureInfo.InvariantCulture);
                }
            }
            else if (values.Length == 7 || values.Length == 8)
            {
                hasColor = true;

                r = float.Parse(values[4], CultureInfo.InvariantCulture);
                g = float.Parse(values[5], CultureInfo.InvariantCulture);
                b = float.Parse(values[6], CultureInfo.InvariantCulture);

                if (values.Length == 8)
                {
                    a = float.Parse(values[7], CultureInfo.InvariantCulture);
                }
            }
            else
            {
                throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber));
            }

            state.Obj.Vertices.Add(new Vertex(new XYZW(x, y, z, w))
            {
                Color = hasColor ? new XYZW(r, g, b, a) : default
            });
        }

        [ObjToken(ObjToken.VN)]
        protected virtual void VN(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 4) { throw new InvalidDataException(string.Format("The <{0}> statement must specify 3 values. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 4) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            state.Obj.VertexNormals.Add(new XYZ()
            {
                X = float.Parse(values[1], CultureInfo.InvariantCulture),
                Y = float.Parse(values[2], CultureInfo.InvariantCulture),
                Z = float.Parse(values[3], CultureInfo.InvariantCulture)
            });
        }

        [ObjToken(ObjToken.VP)]
        protected virtual void VP(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 1 value. Line: {1}.", token, state.LineNumber)); }

            var result = new XYZ()
            {
                X = float.Parse(values[1], CultureInfo.InvariantCulture)
            };

            if (values.Length == 2)
            {
                result.Y = 0.0f;
                result.Z = 1.0f;
            }
            else if (values.Length == 3)
            {
                result.Y = float.Parse(values[2], CultureInfo.InvariantCulture);
                result.Z = 1.0f;
            }
            else if (values.Length == 4)
            {
                result.Y = float.Parse(values[2], CultureInfo.InvariantCulture);
                result.Z = float.Parse(values[3], CultureInfo.InvariantCulture);
            }
            else
            {
                throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber));
            }

            state.Obj.SpaceVertices.Add(result);
        }

        [ObjToken(ObjToken.VT)]
        protected virtual void VT(IObjReaderState state, ObjToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify at least 1 value. Line: {1}.", token, state.LineNumber)); }
            if (values.Length > 4) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}", token, state.LineNumber)); }

            state.Obj.TextureVertices.Add(new XYZ()
            {
                X = float.Parse(values[1], CultureInfo.InvariantCulture),
                Y = values.Length > 2 ? float.Parse(values[2], CultureInfo.InvariantCulture) : 0.0f,
                Z = values.Length > 3 ? float.Parse(values[3], CultureInfo.InvariantCulture) : 0.0f
            });
        }
    }
}