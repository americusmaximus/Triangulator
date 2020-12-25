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
    public enum MaterialToken
    {
        /// <summary>
        ///  Bump texture files are designated by an extension of ".mpb" in the filename. Bump textures modify surface normals
        /// </summary>
        Bump,

        /// <summary>
        /// Material dissolve is multiplied by the texture value
        /// </summary>
        D,

        /// <summary>
        /// Uses a scalar value to deform the surface of an object to  create surface roughness
        /// </summary>
        Decal,

        /// <summary>
        /// Specifies that a scalar texture is used to deform the surface of an object, creating surface roughness
        /// </summary>
        Disp,

        /// <summary>
        /// The statement specifies the illumination model to use in the material. Illumination models are mathematical equations that represent various material lighting and shading effects.
        /// </summary>
        Illum,

        /// <summary>
        /// Material ambient color is multiplied by the texture value
        /// </summary>
        KA,

        /// <summary>
        /// Material diffuse color is multiplied by the texture value
        /// </summary>
        KD,

        /// <summary>
        /// Material emissive color coeficient is multiplied by the texture value
        /// </summary>
        KE,

        /// <summary>
        /// Material specular color is multiplied by the texture value
        /// </summary>
        KS,

        /// <summary>
        /// Turns on/off anti-aliasing of textures in this material without anti-aliasing all textures in the scene
        /// </summary>
        Map_AAt,

        /// <summary>
        /// Some implementations use map_bump instead of bump
        /// </summary>
        Map_Bump,

        /// <summary>
        /// Specifies that a scalar texture file or scalar procedural texture file is linked to the dissolve of the material.  During rendering, the map_d value is multiplied by the d value.
        /// </summary>
        Map_D,

        /// <summary>
        /// Decal map
        /// </summary>
        Map_Decal,

        /// <summary>
        /// Displace map
        /// </summary>
        Map_Disp,

        /// <summary>
        /// Specifies that a color texture file or a color procedural texture file is applied to the ambient reflectivity of the material.
        /// </summary>
        Map_KA,

        /// <summary>
        /// Specifies that a color texture file or color procedural texture file is linked to the diffuse reflectivity of the material.
        /// </summary>
        Map_KD,

        /// <summary>
        /// Specifies that a color texture file or color procedural texture file is linked to the emissive reflectivity of the material.
        /// </summary>
        Map_KE,

        /// <summary>
        /// Specifies that a color texture file or color procedural texture file is linked to the specular reflectivity of the material.
        /// </summary>
        Map_KS,

        /// <summary>
        /// Specifies that a scalar texture file or scalar procedural texture file is linked to the specular exponent of the material.
        /// </summary>
        Map_NS,

        Map_Refl,

        Map_TR,

        /// <summary>
        /// Material name
        /// </summary>
        Newmtl,

        /// <summary>
        /// Optical density / index of refraction
        /// </summary>
        NI,

        /// <summary>
        /// Material specular color exponent is multiplied by the texture value
        /// </summary>
        NS,

        /// <summary>
        /// A reflection map is an environment that simulates reflections in specified objects
        /// </summary>
        Refl,

        /// <summary>
        /// Specifies the sharpness of the reflections from the local reflection map
        /// </summary>
        Sharpness,

        /// <summary>
        /// The Tf statement specifies the transmission filter using RGB values
        /// </summary>
        TF
    }
}
