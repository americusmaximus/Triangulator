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
using WaterWave.Approximations;
using WaterWave.Enums;

namespace WaterWave.IO
{
    public class ObjWriter
    {
        public virtual void Write(Obj obj, StreamWriter writer)
        {
            if (obj == default) { throw new ArgumentNullException(nameof(obj)); }
            if (writer == default) { throw new ArgumentNullException(nameof(writer)); }

            var state = new ObjWriterState(obj);

            Header(state, writer);

            ShadowObjectFileName(state, writer);
            TraceObjectFileName(state, writer);

            MapLibraries(state, writer);
            MaterialLibraries(state, writer);

            Vertices(state, writer);
            ParameterSpaceVertices(state, writer);
            VertexNormals(state, writer);
            TextureVertices(state, writer);

            Points(state, writer);
            Lines(state, writer);
            Faces(state, writer);

            Curves(state, writer);
            Curves2D(state, writer);
            Surfaces(state, writer);

            SurfaceConnection(state, writer);
        }

        protected virtual void AttributesOfElement(IObjWriterState state, Element element, StreamWriter writer)
        {
            if (element.ObjectName != state.ObjectName)
            {
                state.ObjectName = element.ObjectName;
                writer.WriteLine(string.IsNullOrEmpty(state.ObjectName) ? "o" : string.Format("o {0}", state.ObjectName));
            }

            if (element.LevelOfDetail != state.LevelOfDetail)
            {
                state.LevelOfDetail = element.LevelOfDetail;
                writer.WriteLine("lod {0}", state.LevelOfDetail);
            }

            if (element.MapName != state.MapName)
            {
                state.MapName = element.MapName;
                writer.WriteLine(string.IsNullOrEmpty(state.MapName) ? "usemap off" : string.Format("usemap {0}", state.MapName));
            }

            if (element.MaterialName != state.MaterialName)
            {
                state.MaterialName = element.MaterialName;
                writer.WriteLine(string.IsNullOrEmpty(state.MaterialName) ? "usemtl off" : string.Format("usemtl {0}", state.MaterialName));
            }
        }

        protected virtual void AttributesOfFreeFormElement(IObjWriterState state, FreeFormElement element, StreamWriter writer)
        {
            if (element.MergingGroupNumber != state.MergingGroupNumber)
            {
                state.MergingGroupNumber = element.MergingGroupNumber;
                writer.WriteLine(state.MergingGroupNumber == 0 ? "mg off" : string.Format("mg {0} {1}", state.MergingGroupNumber, state.Obj.MergingGroupResolutions[state.MergingGroupNumber].ToString("F6", CultureInfo.InvariantCulture)));
            }

            switch (element.FreeFormType)
            {
                case FreeFormType.BasisMatrix:
                    {
                        writer.Write("cstype");

                        if (element.IsRationalForm)
                        {
                            writer.Write(" rat");
                        }

                        writer.WriteLine(" bmatrix");
                        break;
                    }
                case FreeFormType.Bezier:
                    {
                        writer.Write("cstype");

                        if (element.IsRationalForm)
                        {
                            writer.Write(" rat");
                        }

                        writer.WriteLine(" bezier");
                        break;
                    }
                case FreeFormType.BSpline:
                    {
                        writer.Write("cstype");

                        if (element.IsRationalForm)
                        {
                            writer.Write(" rat");
                        }

                        writer.WriteLine(" bspline");
                        break;
                    }
                case FreeFormType.Cardinal:
                    {
                        writer.Write("cstype");

                        if (element.IsRationalForm)
                        {
                            writer.Write(" rat");
                        }

                        writer.WriteLine(" cardinal");
                        break;
                    }
                case FreeFormType.Taylor:
                    {
                        writer.Write("cstype");

                        if (element.IsRationalForm)
                        {
                            writer.Write(" rat");
                        }

                        writer.WriteLine(" taylor");
                        break;
                    }
            }

            writer.WriteLine(element.DegreeV == 0
                ? string.Format("deg {0}", element.DegreeU)
                : string.Format("deg {0} {1}", element.DegreeU, element.DegreeV));

            if (element.BasicMatrixU != default)
            {
                writer.Write("bmat u");

                foreach (var value in element.BasicMatrixU)
                {
                    writer.Write(" {0}", value.ToString("F6", CultureInfo.InvariantCulture));
                }

                writer.WriteLine();
            }

            if (element.BasicMatrixV != default)
            {
                writer.Write("bmat v");

                foreach (var value in element.BasicMatrixV)
                {
                    writer.Write(" {0}", value.ToString("F6", CultureInfo.InvariantCulture));
                }

                writer.WriteLine();
            }

            writer.WriteLine(element.StepV == 1.0f
                ? string.Format("step {0}", element.StepU.ToString("F6", CultureInfo.InvariantCulture))
                : string.Format("step {0} {1}", element.StepU.ToString("F6", CultureInfo.InvariantCulture), element.StepV.ToString("F6", CultureInfo.InvariantCulture)));

            if (element.CurveApproximationTechnique != default)
            {
                if (element.CurveApproximationTechnique is ConstantParametricSubDivisionTechnique)
                {
                    var technique = (ConstantParametricSubDivisionTechnique)element.CurveApproximationTechnique;
                    writer.WriteLine("ctech cparm {0}", technique.ResolutionU.ToString("F6", CultureInfo.InvariantCulture));
                }
                else if (element.CurveApproximationTechnique is ConstantSpatialSubDivisionTechnique)
                {
                    var technique = (ConstantSpatialSubDivisionTechnique)element.CurveApproximationTechnique;
                    writer.WriteLine("ctech cspace {0}", technique.MaximumLength.ToString("F6", CultureInfo.InvariantCulture));
                }
                else if (element.CurveApproximationTechnique is CurvatureDependentSubDivisionTechnique)
                {
                    var technique = (CurvatureDependentSubDivisionTechnique)element.CurveApproximationTechnique;
                    writer.WriteLine("ctech curv {0} {1}", technique.MaximumDistance.ToString("F6", CultureInfo.InvariantCulture), technique.MaximumAngle.ToString("F6", CultureInfo.InvariantCulture));
                }
            }

            if (element.SurfaceApproximationTechnique != default)
            {
                if (element.SurfaceApproximationTechnique is ConstantParametricSubDivisionTechnique)
                {
                    var technique = (ConstantParametricSubDivisionTechnique)element.SurfaceApproximationTechnique;

                    if (technique.ResolutionU == technique.ResolutionV)
                    {
                        writer.WriteLine("stech cparmb {0}", technique.ResolutionU.ToString("F6", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        writer.WriteLine("stech cparma {0} {1}", technique.ResolutionU.ToString("F6", CultureInfo.InvariantCulture), technique.ResolutionV.ToString("F6", CultureInfo.InvariantCulture));
                    }
                }
                else if (element.SurfaceApproximationTechnique is ConstantSpatialSubDivisionTechnique)
                {
                    var technique = (ConstantSpatialSubDivisionTechnique)element.SurfaceApproximationTechnique;
                    writer.WriteLine("stech cspace {0}", technique.MaximumLength.ToString("F6", CultureInfo.InvariantCulture));
                }
                else if (element.SurfaceApproximationTechnique is CurvatureDependentSubDivisionTechnique)
                {
                    var technique = (CurvatureDependentSubDivisionTechnique)element.SurfaceApproximationTechnique;
                    writer.WriteLine("stech curv {0} {1}", technique.MaximumDistance.ToString("F6", CultureInfo.InvariantCulture), technique.MaximumAngle.ToString("F6", CultureInfo.InvariantCulture));
                }
            }
        }

        protected virtual void AttributesOfPolygonalElement(IObjWriterState state, PolygonalElement element, StreamWriter writer)
        {
            if (element.SmoothingGroupNumber != state.SmoothingGroupNumber)
            {
                state.SmoothingGroupNumber = element.SmoothingGroupNumber;
                writer.WriteLine(state.SmoothingGroupNumber == 0 ? "s off" : string.Format("s {0}", state.SmoothingGroupNumber));
            }

            if (element.IsBevelInterpolationEnabled != state.IsBevelInterpolationEnabled)
            {
                state.IsBevelInterpolationEnabled = element.IsBevelInterpolationEnabled;
                writer.WriteLine(state.IsBevelInterpolationEnabled ? "bevel on": "bevel off");
            }

            if (element.IsColorInterpolationEnabled != state.IsColorInterpolationEnabled)
            {
                state.IsColorInterpolationEnabled = element.IsColorInterpolationEnabled;
                writer.WriteLine(state.IsColorInterpolationEnabled ? "c_interp on" : "c_interp off");
            }

            if (element.IsDissolveInterpolationEnabled != state.IsDissolveInterpolationEnabled)
            {
                state.IsDissolveInterpolationEnabled = element.IsDissolveInterpolationEnabled;
                writer.WriteLine(state.IsDissolveInterpolationEnabled ? "d_interp on" : "d_interp off");
            }
        }

        protected virtual void Curves(IObjWriterState state, StreamWriter writer)
        {
            foreach (var curve in state.Obj.Curves)
            {
                GroupNames(state, writer, curve, g => g.Curves);
                AttributesOfElement(state, curve, writer);
                AttributesOfFreeFormElement(state, curve, writer);

                writer.Write("curv {0} {1}", curve.Start.ToString("F6", CultureInfo.InvariantCulture), curve.End.ToString("F6", CultureInfo.InvariantCulture));

                foreach (var vertex in curve.Vertices)
                {
                    writer.Write(" {0}", vertex);
                }

                writer.WriteLine();

                FreeFormElement(state, curve, writer);
            }
        }

        protected virtual void Curves2D(IObjWriterState state, StreamWriter writer)
        {
            foreach (var curve in state.Obj.Curves2D)
            {
                GroupNames(state, writer, curve, g => g.Curves2D);
                AttributesOfElement(state, curve, writer);
                AttributesOfFreeFormElement(state, curve, writer);

                writer.Write("curv2");

                foreach (var vertex in curve.SpaceVertices)
                {
                    writer.Write(" {0}", vertex);
                }

                writer.WriteLine();

                FreeFormElement(state, curve, writer);
            }
        }

        protected virtual void Faces(IObjWriterState state, StreamWriter writer)
        {
            foreach (var face in state.Obj.Faces)
            {
                GroupNames(state, writer, face, g => g.Faces);
                AttributesOfElement(state, face, writer);
                AttributesOfPolygonalElement(state, face, writer);

                writer.Write("f");

                foreach (var vertex in face.Vertices)
                {
                    writer.Write(" {0}", vertex);
                }

                writer.WriteLine();
            }
        }

        protected virtual void FreeFormElement(IObjWriterState state, FreeFormElement element, StreamWriter writer)
        {
            var writeEnd = false;

            if (element.U.Count != 0)
            {
                writeEnd = true;
                writer.Write("parm u");

                foreach (var value in element.U)
                {
                    writer.Write(" {0}", value.ToString("F6", CultureInfo.InvariantCulture));
                }

                writer.WriteLine();
            }

            if (element.V.Count != 0)
            {
                writeEnd = true;
                writer.Write("parm v");

                foreach (var value in element.V)
                {
                    writer.Write(" {0}", value.ToString("F6", CultureInfo.InvariantCulture));
                }

                writer.WriteLine();
            }

            if (element.OuterTrimmingCurves.Count != 0)
            {
                writeEnd = true;
                writer.Write("trim");

                foreach (var index in element.OuterTrimmingCurves)
                {
                    writer.Write(" {0}", index);
                }

                writer.WriteLine();
            }

            if (element.InnerTrimmingCurves.Count != 0)
            {
                writeEnd = true;
                writer.Write("hole");

                foreach (var index in element.InnerTrimmingCurves)
                {
                    writer.Write(" {0}", index);
                }

                writer.WriteLine();
            }

            if (element.SequenceCurves.Count != 0)
            {
                writeEnd = true;
                writer.Write("scrv");

                foreach (var index in element.SequenceCurves)
                {
                    writer.Write(" {0}", index);
                }

                writer.WriteLine();
            }

            if (element.SpecialPoints.Count != 0)
            {
                writeEnd = true;
                writer.Write("sp");

                foreach (var point in element.SpecialPoints)
                {
                    writer.Write(" {0}", point);
                }

                writer.WriteLine();
            }

            if (writeEnd)
            {
                writer.WriteLine("end");
            }
        }

        protected virtual string GetGroupNames<T>(IObjWriterState state, T element, Func<Group, IList<T>> func) where T : Element
        {
            var result = new List<string>();

            foreach (var group in state.Obj.Groups)
            {
                if (func(group).Contains(element))
                {
                    result.Add(group.Name);
                }
            }

            return string.Join(" ", result);

        }

        protected virtual void GroupNames<T>(IObjWriterState state, StreamWriter writer, T element, Func<Group, IList<T>> func) where T : Element
        {
            var groupNames = GetGroupNames(state, element, func);

            if (groupNames != state.GroupNames)
            {
                state.GroupNames = groupNames;
                writer.WriteLine(string.IsNullOrEmpty(groupNames) ? "g default" : string.Format("g {0}", groupNames));
            }
        }

        protected virtual void Header(IObjWriterState stater, StreamWriter writer)
        {
            if (string.IsNullOrEmpty(stater.Obj.Header)) { return; }

            foreach (var line in stater.Obj.Header.Split('\n'))
            {
                writer.WriteLine("#{0}", line);
            }
        }

        protected virtual void Lines(IObjWriterState state, StreamWriter writer)
        {
            foreach (var line in state.Obj.Lines)
            {
                GroupNames(state, writer, line, g => g.Lines);
                AttributesOfElement(state, line, writer);
                AttributesOfPolygonalElement(state, line, writer);

                writer.Write("l");

                foreach (var vertex in line.Vertices)
                {
                    writer.Write(" {0}", vertex);
                }

                writer.WriteLine();
            }
        }

        protected virtual void MapLibraries(IObjWriterState state, StreamWriter writer)
        {
            if (state.Obj.MapLibraries.Count == 0) { return; }

            writer.Write("maplib");

            foreach (var map in state.Obj.MapLibraries)
            {
                writer.Write(" {0}", map);
            }

            writer.WriteLine();
        }

        protected virtual void MaterialLibraries(IObjWriterState state, StreamWriter writer)
        {
            if (state.Obj.MaterialLibraries.Count == 0) { return; }

            writer.Write("mtllib");

            foreach (var map in state.Obj.MaterialLibraries)
            {
                writer.Write(" {0}", map);
            }

            writer.WriteLine();
        }

        protected virtual void ParameterSpaceVertices(IObjWriterState state, StreamWriter writer)
        {
            foreach (var vertex in state.Obj.SpaceVertices)
            {
                if (vertex.Z == 1.0f)
                {
                    if (vertex.Y == 0.0f)
                    {
                        writer.WriteLine("vp {0}", vertex.X.ToString("F6", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        writer.WriteLine("vp {0} {1}", vertex.X.ToString("F6", CultureInfo.InvariantCulture), vertex.Y.ToString("F6", CultureInfo.InvariantCulture));
                    }
                }
                else
                {
                    writer.WriteLine("vp {0} {1} {2}", vertex.X.ToString("F6", CultureInfo.InvariantCulture), vertex.Y.ToString("F6", CultureInfo.InvariantCulture), vertex.Z.ToString("F6", CultureInfo.InvariantCulture));
                }
            }
        }

        protected virtual void Points(IObjWriterState state, StreamWriter writer)
        {
            foreach (var point in state.Obj.Points)
            {
                GroupNames(state, writer, point, g => g.Points);
                AttributesOfElement(state, point, writer);
                AttributesOfPolygonalElement(state, point, writer);

                writer.Write("p");

                foreach (var vertex in point.Vertices)
                {
                    writer.Write(" {0}", vertex);
                }

                writer.WriteLine();
            }
        }

        protected virtual void ShadowObjectFileName(IObjWriterState state, StreamWriter writer)
        {
            if (!string.IsNullOrEmpty(state.Obj.ShadowObjectFileName))
            {
                writer.Write(string.Format(CultureInfo.InvariantCulture, "shadow_obj {0}\n", state.Obj.ShadowObjectFileName));
            }
        }

        protected virtual void SurfaceConnection(IObjWriterState state, StreamWriter writer)
        {
            foreach (var con in state.Obj.SurfaceConnections)
            {
                writer.WriteLine("con {0} {1} {2} {3}", con.Surface1.ToString(CultureInfo.InvariantCulture), con.Curve2D1.ToString(), con.Surface2.ToString(CultureInfo.InvariantCulture), con.Curve2D2.ToString());
            }
        }

        protected virtual void Surfaces(IObjWriterState state, StreamWriter writer)
        {
            foreach (var surface in state.Obj.Surfaces)
            {
                GroupNames(state, writer, surface, g => g.Surfaces);
                AttributesOfElement(state, surface, writer);
                AttributesOfFreeFormElement(state, surface, writer);

                writer.Write("surf {0} {1} {2} {3}", surface.StartU.ToString("F6", CultureInfo.InvariantCulture), surface.EndU.ToString("F6", CultureInfo.InvariantCulture), surface.StartV.ToString("F6", CultureInfo.InvariantCulture), surface.EndV.ToString("F6", CultureInfo.InvariantCulture));

                foreach (var vertex in surface.Vertices)
                {
                    writer.Write(" {0}", vertex);
                }

                writer.WriteLine();

                FreeFormElement(state, surface, writer);
            }
        }

        protected virtual void TextureVertices(IObjWriterState state, StreamWriter writer)
        {
            foreach (var vertex in state.Obj.TextureVertices)
            {
                writer.WriteLine("vt {0} {1} {2}", vertex.X.ToString("F6", CultureInfo.InvariantCulture), vertex.Y.ToString("F6", CultureInfo.InvariantCulture), vertex.Z.ToString("F6", CultureInfo.InvariantCulture));
            }
        }

        protected virtual void TraceObjectFileName(IObjWriterState state, StreamWriter writer)
        {
            if (!string.IsNullOrEmpty(state.Obj.TraceObjectFileName))
            {
                writer.Write(string.Format(CultureInfo.InvariantCulture, "trace_obj {0}\n", state.Obj.TraceObjectFileName));
            }
        }

        protected virtual void VertexNormals(IObjWriterState state, StreamWriter writer)
        {
            foreach (var vertex in state.Obj.VertexNormals)
            {
                writer.WriteLine("vn {0} {1} {2}", vertex.X.ToString("F6", CultureInfo.InvariantCulture), vertex.Y.ToString("F6", CultureInfo.InvariantCulture), vertex.Z.ToString("F6", CultureInfo.InvariantCulture)); ;
            }
        }

        protected virtual void Vertices(IObjWriterState state, StreamWriter writer)
        {
            foreach (var vertex in state.Obj.Vertices)
            {
                var position = vertex.Position;

                if (vertex.Color != default)
                {
                    var color = vertex.Color;

                    if (color.W == 1.0f)
                    {
                        writer.WriteLine("v {0} {1} {2} {3} {4} {5}", position.X.ToString("F6", CultureInfo.InvariantCulture), position.Y.ToString("F6", CultureInfo.InvariantCulture), position.Z.ToString("F6", CultureInfo.InvariantCulture), color.X.ToString("F6", CultureInfo.InvariantCulture), color.Y.ToString("F6", CultureInfo.InvariantCulture), color.Z.ToString("F6", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        writer.WriteLine("v {0} {1} {2} {3} {4} {5} {6}", position.X.ToString("F6", CultureInfo.InvariantCulture), position.Y.ToString("F6", CultureInfo.InvariantCulture), position.Z.ToString("F6", CultureInfo.InvariantCulture), color.X.ToString("F6", CultureInfo.InvariantCulture), color.Y.ToString("F6", CultureInfo.InvariantCulture), color.Z.ToString("F6", CultureInfo.InvariantCulture), color.W.ToString("F6", CultureInfo.InvariantCulture));
                    }
                }
                else
                {
                    if (position.W == 1.0f)
                    {
                        writer.WriteLine("v {0} {1} {2}", position.X.ToString("F6", CultureInfo.InvariantCulture), position.Y.ToString("F6", CultureInfo.InvariantCulture), position.Z.ToString("F6", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        writer.WriteLine("v {0} {1} {2} {3}", position.X.ToString("F6", CultureInfo.InvariantCulture), position.Y.ToString("F6", CultureInfo.InvariantCulture), position.Z.ToString("F6", CultureInfo.InvariantCulture), position.W.ToString("F6", CultureInfo.InvariantCulture));
                    }
                }
            }
        }
    }
}