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

namespace WaterWave.IO.Enums
{
    public enum ObjToken
    {
        /// <summary>
        /// Bevel Interpolation
        /// </summary>
        Bevel,

        /// <summary>
        /// Basis Matrix
        /// </summary>
        BMat,

        /// <summary>
        /// B-spline
        /// </summary>
        /// <remarks>
        /// In the 3.0 release, the following keywords have been superseded: bsp, bzp, cdc, cdp, res.
        /// </remarks>
        Bsp,

        /// <summary>
        ///  Bezier Patch
        /// </summary>
        /// <remarks>
        /// In the 3.0 release, the following keywords have been superseded: bsp, bzp, cdc, cdp, res.
        /// </remarks>
        Bzp,

        /// <summary>
        /// Color Interpolation
        /// </summary>
        C_Interp,

        /// <summary>
        /// Cardinal Curve
        /// </summary>
        /// <remarks>
        /// In the 3.0 release, the following keywords have been superseded: bsp, bzp, cdc, cdp, res.
        /// </remarks>
        Cdc,

        /// <summary>
        /// Cardinal Patch
        /// </summary>
        /// <remarks>
        /// In the 3.0 release, the following keywords have been superseded: bsp, bzp, cdc, cdp, res.
        /// </remarks>
        Cdp,

        /// <summary>
        /// Connect
        /// </summary>
        Con,

        /// <summary>
        /// Rational or non-rational forms of curve or surface type: Basis Matrix, Bezier, B-spline, Cardinal, Taylor
        /// </summary>
        CSType,

        /// <summary>
        /// Curve Approximation Technique
        /// </summary>
        CTech,

        /// <summary>
        /// Curve
        /// </summary>
        Curv,

        /// <summary>
        /// 2D Curve
        /// </summary>
        Curv2,

        /// <summary>
        /// Dissolve Interpolation
        /// </summary>
        D_Interp,

        /// <summary>
        /// Degree
        /// </summary>
        Deg,

        /// <summary>
        /// End Statement
        /// </summary>
        End,

        /// <summary>
        /// Face
        /// </summary>
        F,

        /// <summary>
        /// Face Outline
        /// </summary>
        /// <remarks>
        /// Any references to fo (face outline) are no longer valid as of version 2.11. You can use f (face) to get the same results.
        /// </remarks>
        FO,

        /// <summary>
        /// Group Name
        /// </summary>
        G,

        /// <summary>
        /// Inner Trimming Loop
        /// </summary>
        Hole,

        /// <summary>
        /// Line
        /// </summary>
        L,

        /// <summary>
        /// Level Of Detail
        /// </summary>
        LOD,

        /// <summary>
        /// Map Library
        /// </summary>
        MapLib,

        /// <summary>
        /// Merging Group
        /// </summary>
        MG,

        /// <summary>
        /// Material Library
        /// </summary>
        MtlLib,

        /// <summary>
        /// Object Name
        /// </summary>
        O,

        /// <summary>
        /// Point
        /// </summary>
        P,

        /// <summary>
        /// Parameter Values
        /// </summary>
        Parm,

        /// <summary>
        /// Reference and display statement
        /// </summary>
        /// <remarks>
        /// In the 3.0 release, the following keywords have been superseded: bsp, bzp, cdc, cdp, res.
        /// </remarks>
        Res,

        /// <summary>
        /// Smoothing Group
        /// </summary>
        S,

        /// <summary>
        /// Special Curve
        /// </summary>
        Scrv,

        /// <summary>
        /// Shadow Casting
        /// </summary>
        Shadow_Obj,

        /// <summary>
        /// Special Point
        /// </summary>
        Sp,

        /// <summary>
        /// Surface Approximation Technique
        /// </summary>
        STech,

        /// <summary>
        /// Step Size
        /// </summary>
        Step,

        /// <summary>
        /// Surface
        /// </summary>
        Surf,

        /// <summary>
        /// Ray Tracing
        /// </summary>
        Trace_Obj,

        /// <summary>
        /// Outer Trimming Loop
        /// </summary>
        Trim,

        /// <summary>
        /// Texture Map Name
        /// </summary>
        UseMap,

        /// <summary>
        /// Material Name
        /// </summary>
        UseMtl,

        /// <summary>
        /// Geometric Vertices
        /// </summary>
        V,

        /// <summary>
        /// Vertex Normals
        /// </summary>
        VN,

        /// <summary>
        /// Parameter Space Vertices
        /// </summary>
        VP,

        /// <summary>
        /// Texture Vertices
        /// </summary>
        VT
    }
}
