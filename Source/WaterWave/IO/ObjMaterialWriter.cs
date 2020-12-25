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
using System.Globalization;
using System.IO;
using WaterWave.Enums;
using WaterWave.IO.Enums;

namespace WaterWave.IO
{
    public class ObjMaterialWriter
    {
        public virtual void Write(MaterialCollection collection, StreamWriter writer)
        {
            if (collection == default) { throw new ArgumentNullException(nameof(collection)); }
            if (writer == default) { throw new ArgumentNullException(nameof(writer)); }

            Header(collection, writer);

            foreach (var material in collection.Materials)
            {
                Material(material, writer);
            }
        }

        protected virtual void Color(MaterialColor color, MaterialToken token, StreamWriter writer)
        {
            writer.Write(token.ToString().ToLowerInvariant());

            if (color.IsSpectral)
            {
                writer.Write(" {0} {1}", MaterialColorToken.Spectral.ToString().ToLowerInvariant(), color.SpectralFileName);

                if (color.SpectralFactor != 1.0f)
                {
                    writer.Write(" {0}", color.SpectralFactor.ToString("F6", CultureInfo.InvariantCulture));
                }

                writer.WriteLine();
            }
            else
            {
                if (color.UseXYZColorSpace)
                {
                    writer.Write(" {0}", MaterialColorToken.XYZ.ToString().ToLowerInvariant());
                }

                writer.WriteLine(" {0} {1} {2}", color.Color.X.ToString("F6", CultureInfo.InvariantCulture), color.Color.Y.ToString("F6", CultureInfo.InvariantCulture), color.Color.Z.ToString("F6", CultureInfo.InvariantCulture));
            }
        }

        protected virtual void Header(MaterialCollection material, StreamWriter writer)
        {
            if (string.IsNullOrEmpty(material.Header)) { return; }

            foreach (var line in material.Header.Split('\n'))
            {
                writer.WriteLine("#{0}", line);
            }
        }

        protected virtual void Map(MaterialMap map, string token, StreamWriter writer)
        {
            writer.Write(token);

            if (!map.IsHorizontalBlendingEnabled)
            {
                writer.Write(" -blenu {0}", OnOff.Off.ToString().ToLowerInvariant());
            }

            if (!map.IsVerticalBlendingEnabled)
            {
                writer.Write(" -blenv {0}", OnOff.Off.ToString().ToLowerInvariant());
            }

            if (map.BumpMultiplier != 0.0f)
            {
                writer.Write(" -bm ");
                writer.Write(map.BumpMultiplier.ToString("F6", CultureInfo.InvariantCulture));
            }

            if (map.Boost != 0.0f)
            {
                writer.Write(" -boost ");
                writer.Write(map.Boost.ToString("F6", CultureInfo.InvariantCulture));
            }

            if (map.IsColorCorrectionEnabled)
            {
                writer.Write(" -cc {0}", OnOff.On.ToString().ToLowerInvariant());
            }

            if (map.IsClampingEnabled)
            {
                writer.Write(" -clamp {0}", OnOff.On.ToString().ToLowerInvariant());
            }

            if (map.ScalarChannel != MapChannel.Luminance)
            {
                writer.Write(" -imfchan ");

                switch (map.ScalarChannel)
                {
                    case MapChannel.Red:
                        {
                            writer.Write(IMFChanToken.R.ToString().ToLowerInvariant());
                            break;
                        }
                    case MapChannel.Green:
                        {
                            writer.Write(IMFChanToken.G.ToString().ToLowerInvariant());
                            break;
                        }
                    case MapChannel.Blue:
                        {
                            writer.Write(IMFChanToken.B.ToString().ToLowerInvariant());
                            break;
                        }
                    case MapChannel.Matte:
                        {
                            writer.Write(IMFChanToken.M.ToString().ToLowerInvariant());
                            break;
                        }
                    case MapChannel.Depth:
                        {
                            writer.Write(IMFChanToken.Z.ToString().ToLowerInvariant());
                            break;
                        }
                    default:
                        {
                            writer.Write(IMFChanToken.L.ToString().ToLowerInvariant());
                            break;
                        }
                }
            }

            if (map.ModifierBase != 0.0f || map.ModifierGain != 1.0f)
            {
                writer.Write(" -mm {0} {1}", map.ModifierBase.ToString("F6", CultureInfo.InvariantCulture), map.ModifierGain.ToString("F6", CultureInfo.InvariantCulture));
            }

            if (map.Offset.X != 0.0f || map.Offset.Y != 0.0f || map.Offset.Z != 0.0f)
            {
                writer.Write(" -o {0} {1} {2}", map.Offset.X.ToString("F6", CultureInfo.InvariantCulture), map.Offset.Y.ToString("F6", CultureInfo.InvariantCulture), map.Offset.Z.ToString("F6", CultureInfo.InvariantCulture));
            }

            if (map.Scale.X != 1.0f || map.Scale.Y != 1.0f || map.Scale.Z != 1.0f)
            {
                writer.Write(" -s {0} {1} {2}", map.Scale.X.ToString("F6", CultureInfo.InvariantCulture), map.Scale.Y.ToString("F6", CultureInfo.InvariantCulture), map.Scale.Z.ToString("F6", CultureInfo.InvariantCulture));
            }

            if (map.Turbulence.X != 0.0f || map.Turbulence.Y != 0.0f || map.Turbulence.Z != 0.0f)
            {
                writer.Write(" -t {0} {1} {2}", map.Turbulence.X.ToString("F6", CultureInfo.InvariantCulture), map.Turbulence.Y.ToString("F6", CultureInfo.InvariantCulture), map.Turbulence.Z.ToString("F6", CultureInfo.InvariantCulture));
            }

            if (map.TextureResolution != 0)
            {
                writer.Write(" -texres {0}", map.TextureResolution);
            }

            writer.WriteLine(" {0}", map.FileName);
        }

        protected virtual void Material(Material material, StreamWriter writer)
        {
            writer.WriteLine("{0} {1}", MaterialToken.Newmtl.ToString().ToLowerInvariant(), material.Name);

            if (material.AmbientColor != default)
            {
                Color(material.AmbientColor, MaterialToken.KA, writer);
            }

            if (material.DiffuseColor != default)
            {
                Color(material.DiffuseColor, MaterialToken.KD, writer);
            }

            if (material.EmissiveColor != default)
            {
                Color(material.EmissiveColor, MaterialToken.KE, writer);
            }

            if (material.SpecularColor != default)
            {
                Color(material.SpecularColor, MaterialToken.KS, writer);
            }

            if (material.TransmissionColor != default)
            {
                Color(material.TransmissionColor, MaterialToken.TF, writer);
            }

            writer.WriteLine("{0} {1}", MaterialToken.Illum.ToString().ToLowerInvariant(), material.IlluminationModel);
            writer.Write("{0}", MaterialToken.D.ToString().ToLowerInvariant());

            if (material.IsHaloDissolve)
            {
                writer.Write(" -halo");
            }

            writer.WriteLine(" {0}", material.DissolveFactor.ToString("F6", CultureInfo.InvariantCulture));
            writer.WriteLine("Ns {0}", material.SpecularExponent.ToString("F6", CultureInfo.InvariantCulture));
            writer.WriteLine("{0} {1}", MaterialToken.Sharpness.ToString().ToLowerInvariant(), material.Sharpness);
            writer.WriteLine("Ni {0}", material.OpticalDensity.ToString("F6", CultureInfo.InvariantCulture));
            writer.WriteLine("{0} {1}", MaterialToken.Map_AAt.ToString().ToLowerInvariant(), material.IsAntiAliasingEnabled ? OnOff.On.ToString().ToLowerInvariant() : OnOff.Off.ToString().ToLowerInvariant());

            if (material.AmbientMap != default)
            {
                Map(material.AmbientMap, MaterialToken.Map_KA.ToString().ToLowerInvariant(), writer);
            }

            if (material.DiffuseMap != default)
            {
                Map(material.DiffuseMap, MaterialToken.Map_KD.ToString().ToLowerInvariant(), writer);
            }

            if (material.EmissiveMap != default)
            {
                Map(material.EmissiveMap, MaterialToken.Map_KE.ToString().ToLowerInvariant(), writer);
            }

            if (material.SpecularMap != default)
            {
                Map(material.SpecularMap, MaterialToken.Map_KS.ToString().ToLowerInvariant(), writer);
            }

            if (material.SpecularExponentMap != default)
            {
                Map(material.SpecularExponentMap, MaterialToken.Map_NS.ToString().ToLowerInvariant(), writer);
            }

            if (material.DissolveMap != default)
            {
                Map(material.DissolveMap, MaterialToken.Map_D.ToString().ToLowerInvariant(), writer);
            }

            if (material.DecalMap != default)
            {
                Map(material.DecalMap, MaterialToken.Decal.ToString().ToLowerInvariant(), writer);
            }

            if (material.DispMap != default)
            {
                Map(material.DispMap, MaterialToken.Disp.ToString().ToLowerInvariant(), writer);
            }

            if (material.BumpMap != default)
            {
                Map(material.BumpMap, MaterialToken.Bump.ToString().ToLowerInvariant(), writer);
            }

            if (material.ReflectionMap.Sphere != default)
            {
                Map(material.ReflectionMap.Sphere, "refl -type sphere", writer);
            }

            if (material.ReflectionMap.CubeTop != default)
            {
                Map(material.ReflectionMap.CubeTop, "refl -type cube_top", writer);
            }

            if (material.ReflectionMap.CubeBottom != default)
            {
                Map(material.ReflectionMap.CubeBottom, "refl -type cube_bottom", writer);
            }

            if (material.ReflectionMap.CubeFront != default)
            {
                Map(material.ReflectionMap.CubeFront, "refl -type cube_front", writer);
            }

            if (material.ReflectionMap.CubeBack != default)
            {
                Map(material.ReflectionMap.CubeBack, "refl -type cube_back", writer);
            }

            if (material.ReflectionMap.CubeLeft != default)
            {
                Map(material.ReflectionMap.CubeLeft, "refl -type cube_left", writer);
            }

            if (material.ReflectionMap.CubeRight != default)
            {
                Map(material.ReflectionMap.CubeRight, "refl -type cube_right", writer);
            }
        }
    }
}