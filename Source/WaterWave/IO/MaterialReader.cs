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
using WaterWave.Enums;
using WaterWave.IO.Attributes;
using WaterWave.IO.Enums;

namespace WaterWave.IO
{
    public class MaterialReader : IMaterialReader
    {
        public MaterialReader()
        {
            Initialize();
        }

        protected virtual Dictionary<MaterialToken, Action<IMaterialReaderState, MaterialToken, string[]>> Executors { get; set; }

        protected virtual Dictionary<MaterialMapToken, Action<IMaterialMapState, MaterialMapToken, string[]>> MapExecutors { get; set; }

        public MaterialCollection Read(Stream stream)
        {
            if (stream == default) { throw new ArgumentNullException(nameof(stream)); }

            var reader = new LineReader();
            var collection = new MaterialCollection();
            var state = new MaterialReaderState(collection, reader);

            foreach (var values in reader.Read(stream))
            {
                if (values.Length == 0) { continue; }

                if (!Enum.TryParse<MaterialToken>(values[0], true, out var token)) { throw new InvalidDataException(string.Format("Unable to parse token <{0}>. Line: {1}.", token, state.LineNumber)); }

                if (!Executors.TryGetValue(token, out var executor)) { throw new InvalidOperationException(string.Format("Unable to find an executor for token <{0}>.", token)); }

                executor.Invoke(state, token, values);
            }

            collection.Header = state.Header;

            return collection;
        }

        [MaterialToken(MaterialToken.Bump)]
        [MaterialToken(MaterialToken.Map_Bump)]
        protected virtual void Bump(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.BumpMap = Map(state, token, values);
        }

        protected virtual MaterialColor Color(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a color. Line: {1}.", token, state.LineNumber)); }

            var color = new MaterialColor();

            int index = 1;

            if (!Enum.TryParse<MaterialColorToken>(values[1], true, out var tokenType))
            {
                tokenType = MaterialColorToken.RGB;
            }

            switch (tokenType)
            {
                case MaterialColorToken.Spectral:
                    {
                        index++;

                        if (values.Length - index < 1) { throw new InvalidDataException(string.Format("The <{0}> spectral statement must specify a file name. Line: {1}.", token, state.LineNumber)); }

                        if (!Path.HasExtension(values[index])) { throw new InvalidDataException(string.Format("A filename must have an extension. Value: <{0}>. Line: {1}.", values[index], state.LineNumber)); }

                        color.SpectralFileName = values[index];
                        index++;

                        if (values.Length > index)
                        {
                            color.SpectralFactor = float.Parse(values[index], CultureInfo.InvariantCulture);
                            index++;
                        }

                        break;
                    }
                case MaterialColorToken.XYZ:
                    {
                        index++;

                        if (values.Length - index < 1) { throw new InvalidDataException(string.Format("The <{0}> xyz statement must specify a color. Line: {1}.", token, state.LineNumber)); }

                        color.UseXYZColorSpace = true;

                        var xyz = new XYZ()
                        {
                            X = float.Parse(values[index], CultureInfo.InvariantCulture)
                        };

                        index++;

                        if (values.Length > index)
                        {
                            if (values.Length - index < 2) { throw new InvalidDataException(string.Format("The <{0}> xyz statement must specify a XYZ color. Line: {1}.", token, state.LineNumber)); }

                            xyz.Y = float.Parse(values[index], CultureInfo.InvariantCulture);
                            xyz.Z = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
                            index += 2;
                        }
                        else
                        {
                            xyz.Y = xyz.X;
                            xyz.Z = xyz.X;
                        }

                        color.Color = xyz;
                        break;
                    }
                default:
                    {
                        var rgb = new XYZ()
                        {
                            X = float.Parse(values[index], CultureInfo.InvariantCulture)
                        };

                        index++;

                        if (values.Length > index)
                        {
                            if (values.Length - index < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a RGB color. Line: {1}.", token, state.LineNumber)); }

                            rgb.Y = float.Parse(values[index], CultureInfo.InvariantCulture);
                            rgb.Z = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
                            index += 2;
                        }
                        else
                        {
                            rgb.Y = rgb.X;
                            rgb.Z = rgb.X;
                        }

                        color.Color = rgb;
                        break;
                    }
            }

            if (index != values.Length) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            return color;
        }

        protected virtual void D(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a factor. Line: {1}.", token, state.LineNumber)); }

            if (string.Equals(values[1], "-halo", StringComparison.OrdinalIgnoreCase))
            {
                if (values.Length < 3) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a factor. Line: {1}.", token, state.LineNumber)); }
                if (values.Length != 3) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

                state.Material.IsHaloDissolve = true;
                state.Material.DissolveFactor = float.Parse(values[2], CultureInfo.InvariantCulture);
            }
            else
            {
                if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

                state.Material.DissolveFactor = float.Parse(values[1], CultureInfo.InvariantCulture);
            }

        }

        [MaterialToken(MaterialToken.Decal)]
        [MaterialToken(MaterialToken.Map_Decal)]
        protected virtual void Decal(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.DecalMap = Map(state, token, values);
        }

        [MaterialToken(MaterialToken.Disp)]
        [MaterialToken(MaterialToken.Map_Disp)]
        protected virtual void Disp(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.DispMap = Map(state, token, values);
        }

        [MaterialToken(MaterialToken.Illum)]
        protected virtual void Illum(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}.", state.LineNumber)); }
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify an illumination model. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            state.Material.IlluminationModel = int.Parse(values[1], CultureInfo.InvariantCulture);
        }

        protected virtual void Initialize()
        {
            Executors = new Dictionary<MaterialToken, Action<IMaterialReaderState, MaterialToken, string[]>>(Enum.GetValues(typeof(MaterialToken)).Length);

            var executorMethods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in executorMethods)
            {
                var attrs = method.GetCustomAttributes(typeof(MaterialTokenAttribute), true).OfType<MaterialTokenAttribute>().ToArray();

                foreach (var attr in attrs)
                {
                    if (Executors.ContainsKey(attr.Token)) { throw new InvalidOperationException(string.Format("Illegal attempt to to register method <{0}> as an executor for token <{1}>.", method.Name, attr.Token)); }

                    Executors.Add(attr.Token, (s, t, v) => method.Invoke(this, new object[] { s, t, v }));
                }
            }

            MapExecutors = new Dictionary<MaterialMapToken, Action<IMaterialMapState, MaterialMapToken, string[]>>(Enum.GetValues(typeof(MaterialMapToken)).Length);
            
            var mapExecutorMethods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in mapExecutorMethods)
            {
                var attrs = method.GetCustomAttributes(typeof(MaterialMapTokenAttribute), true).OfType<MaterialMapTokenAttribute>().ToArray();

                foreach (var attr in attrs)
                {
                    if (MapExecutors.ContainsKey(attr.Token)) { throw new InvalidOperationException(string.Format("Illegal attempt to to register method <{0}> as an executor for token <{1}>.", method.Name, attr.Token)); }

                    MapExecutors.Add(attr.Token, (s, t, v) => method.Invoke(this, new object[] { s, t, v }));
                }
            }
        }

        [MaterialToken(MaterialToken.KA)]
        protected virtual void KA(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.AmbientColor = Color(state, token, values);
        }

        [MaterialToken(MaterialToken.KD)]
        protected virtual void KD(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.DiffuseColor = Color(state, token, values);
        }

        [MaterialToken(MaterialToken.KE)]
        protected virtual void KE(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.EmissiveColor = Color(state, token, values);
        }

        [MaterialToken(MaterialToken.KS)]
        protected virtual void KS(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.SpecularColor = Color(state, token, values);
        }

        protected virtual MaterialMap Map(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            var mapState = new MaterialMapState()
            {
                Index = 0,
                LineNumber = state.LineNumber,
                Map = new MaterialMap(),
                Token = token
            };

            while (mapState.Index < values.Length)
            {
                mapState.Index++;

                if (values.Length - mapState.Index < 1) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a filename. Line: {1}.", token, state.LineNumber)); }

                var optionString = values[mapState.Index].ToLowerInvariant().Trim(new[] { '-' });

                if (!Enum.TryParse<MaterialMapToken>(optionString, true, out var option))
                {
                    if (!Path.HasExtension(values[mapState.Index])) { throw new InvalidDataException(string.Format("A filename must have an extension. Value: <{0}>. Line: {1}.", values[mapState.Index], state.LineNumber)); }

                    mapState.Map.FileName = values[mapState.Index];
                    mapState.Index++;

                    if (mapState.Index != values.Length) { throw new InvalidDataException(string.Format("The <{0}> has too many values. Line: {1}.", token, state.LineNumber)); }

                    continue;
                }


                if (!MapExecutors.TryGetValue(option, out var executor)) { throw new InvalidOperationException(string.Format("Unable to find an executor for token <{0}>.", option)); }

                executor.Invoke(mapState, option, values);
            }

            return mapState.Map;
        }

        [MaterialToken(MaterialToken.Map_AAt)]
        protected virtual void MapAAt(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a value. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            if (!Enum.TryParse<OnOff>(values[1], true, out var onOffValue)) { throw new InvalidDataException(string.Format("The <{0}> statement must specify on or off. Line: {1}.", token, state.LineNumber)); }

            state.Material.IsAntiAliasingEnabled = onOffValue == OnOff.On;
        }

        [MaterialToken(MaterialToken.Map_D)]
        [MaterialToken(MaterialToken.Map_TR)]
        protected virtual void MapD(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.DissolveMap = Map(state, token, values);
        }

        [MaterialToken(MaterialToken.Map_KA)]
        protected virtual void MapKA(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.AmbientMap = Map(state, token, values);
        }

        [MaterialToken(MaterialToken.Map_KD)]
        protected virtual void MapKD(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.DiffuseMap = Map(state, token, values);
        }

        [MaterialToken(MaterialToken.Map_KE)]
        protected virtual void MapKE(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.EmissiveMap = Map(state, token, values);
        }

        [MaterialToken(MaterialToken.Map_KS)]
        protected virtual void MapKS(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.SpecularMap = Map(state, token, values);
        }

        [MaterialToken(MaterialToken.Map_NS)]
        protected virtual void MapNS(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.SpecularExponentMap = Map(state, token, values);
        }

        [MaterialMapToken(MaterialMapToken.BlenU)]
        protected virtual void MapOptionBlenU(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value. Line: {2}.", state.Token, token, state.LineNumber)); }

            if (!Enum.TryParse<OnOff>(values[1], true, out var onOffValue)) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify on or off. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Map.IsHorizontalBlendingEnabled = onOffValue == OnOff.On;

            state.Index++;
        }

        [MaterialMapToken(MaterialMapToken.BlenV)]
        protected virtual void MapOptionBlenV(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value. Line: {2}.", state.Token, token, state.LineNumber)); }

            if (!Enum.TryParse<OnOff>(values[1], true, out var onOffValue)) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify on or off. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Map.IsVerticalBlendingEnabled = onOffValue == OnOff.On;

            state.Index++;
        }

        [MaterialMapToken(MaterialMapToken.BM)]
        protected virtual void MapOptionBM(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Map.BumpMultiplier = float.Parse(values[state.Index + 1], CultureInfo.InvariantCulture);

            state.Index++;
        }

        [MaterialMapToken(MaterialMapToken.Boost)]
        protected virtual void MapOptionBoost(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Map.Boost = float.Parse(values[state.Index + 1], CultureInfo.InvariantCulture);

            state.Index++;
        }

        [MaterialMapToken(MaterialMapToken.CC)]
        protected virtual void MapOptionCC(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value. Line: {2}.", state.Token, token, state.LineNumber)); }

            if (!Enum.TryParse<OnOff>(values[1], true, out var onOffValue)) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify on or off. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Map.IsColorCorrectionEnabled = onOffValue == OnOff.On;

            state.Index++;
        }

        [MaterialMapToken(MaterialMapToken.Clamp)]
        protected virtual void MapOptionClamp(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value. Line: {2}.", state.Token, token, state.LineNumber)); }

            if (!Enum.TryParse<OnOff>(values[1], true, out var onOffValue)) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify on or off. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Map.IsClampingEnabled = onOffValue == OnOff.On;

            state.Index++;
        }

        [MaterialMapToken(MaterialMapToken.IMFChan)]
        protected virtual void MapOptionImfChan(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value. Line: {2}.", state.Token, token, state.LineNumber)); }

            if (!Enum.TryParse<IMFChanToken>(values[state.Index + 1], true, out var imfChanToken)) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value in ({2}).", state.Token, token, string.Join(", ", Enum.GetValues(typeof(IMFChanToken))))); }

            switch (imfChanToken)
            {
                case IMFChanToken.R:
                    {
                        state.Map.ScalarChannel = MapChannel.Red;
                        break;
                    }
                case IMFChanToken.G:
                    {
                        state.Map.ScalarChannel = MapChannel.Green;
                        break;
                    }
                case IMFChanToken.B:
                    {
                        state.Map.ScalarChannel = MapChannel.Blue;
                        break;
                    }
                case IMFChanToken.M:
                    {
                        state.Map.ScalarChannel = MapChannel.Matte;
                        break;
                    }
                case IMFChanToken.L:
                    {
                        state.Map.ScalarChannel = MapChannel.Luminance;
                        break;
                    }
                case IMFChanToken.Z:
                    {
                        state.Map.ScalarChannel = MapChannel.Depth;
                        break;
                    }
            }

            state.Index++;
        }

        [MaterialMapToken(MaterialMapToken.MM)]
        protected virtual void MapOptionMM(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 3) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a base and a gain. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Map.ModifierBase = float.Parse(values[state.Index + 1], CultureInfo.InvariantCulture);
            state.Map.ModifierGain = float.Parse(values[state.Index + 2], CultureInfo.InvariantCulture);

            state.Index += 2;
        }

        [MaterialMapToken(MaterialMapToken.O)]
        protected virtual void MapOptionO(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify at least 2 values. Line: {2}.", state.Token, token, state.LineNumber)); }

            var offset = new XYZ()
            {
                X = float.Parse(values[state.Index + 1], CultureInfo.InvariantCulture)
            };

            if (values.Length - state.Index > 3)
            {
                offset.Y = float.Parse(values[state.Index + 2], CultureInfo.InvariantCulture);

                if (values.Length - state.Index > 4)
                {
                    offset.Z = float.Parse(values[state.Index + 3], CultureInfo.InvariantCulture);
                    state.Index++;
                }

                state.Index++;
            }

            state.Index++;

            state.Map.Offset = offset;
        }

        [MaterialMapToken(MaterialMapToken.S)]
        protected virtual void MapOptionS(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify at least 2 values. Line: {2}.", state.Token, token, state.LineNumber)); }

            var scale = new XYZ(1.0f, 1.0f, 1.0f);

            scale.X = float.Parse(values[state.Index + 1], CultureInfo.InvariantCulture);

            if (values.Length - state.Index > 3)
            {
                scale.Y = float.Parse(values[state.Index + 2], CultureInfo.InvariantCulture);

                if (values.Length - state.Index > 4)
                {
                    scale.Z = float.Parse(values[state.Index + 3], CultureInfo.InvariantCulture);
                    state.Index++;
                }

                state.Index++;
            }

            state.Index++;

            state.Map.Scale = scale;
        }

        [MaterialMapToken(MaterialMapToken.T)]
        protected virtual void MapOptionT(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify at least 2 values. Line: {2}.", state.Token, token, state.LineNumber)); }

            var turbulence = new XYZ()
            {
                X = float.Parse(values[state.Index + 1], CultureInfo.InvariantCulture)
            };

            if (values.Length - state.Index > 3)
            {
                turbulence.Y = float.Parse(values[state.Index + 2], CultureInfo.InvariantCulture);

                if (values.Length - state.Index > 4)
                {
                    turbulence.Z = float.Parse(values[state.Index + 3], CultureInfo.InvariantCulture);
                    state.Index++;
                }

                state.Index++;
            }

            state.Index++;

            state.Map.Turbulence = turbulence;
        }

        [MaterialMapToken(MaterialMapToken.TexRes)]
        protected virtual void MapOptionTexRes(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a values. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Map.TextureResolution = int.Parse(values[state.Index + 1], CultureInfo.InvariantCulture);

            state.Index++;
        }
        [MaterialMapToken(MaterialMapToken.Type)]
        protected virtual void MapOptionType(IMaterialMapState state, MaterialMapToken token, string[] values)
        {
            if (values.Length - state.Index < 2) { throw new InvalidDataException(string.Format("The <{0}> -{1} option must specify a value. Line: {2}.", state.Token, token, state.LineNumber)); }

            state.Index++;
        }

        [MaterialToken(MaterialToken.Newmtl)]
        protected virtual void NewMtl(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a name. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            state.Material = new Material() { Name = values[1] };
            state.Collection.Materials.Add(state.Material);
        }

        [MaterialToken(MaterialToken.NI)]
        protected virtual void NI(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify an optical density. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            state.Material.OpticalDensity = float.Parse(values[1], CultureInfo.InvariantCulture);
        }

        [MaterialToken(MaterialToken.NS)]
        protected virtual void NS(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a specular exponent. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            state.Material.SpecularExponent = float.Parse(values[1], CultureInfo.InvariantCulture);
        }

        [MaterialToken(MaterialToken.Refl)]
        [MaterialToken(MaterialToken.Map_Refl)]
        protected virtual void Refl(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }
            if (values.Length < 4) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a type and a file name. Line: {1}.", token, state.LineNumber)); }
            if (!string.Equals(values[1], "-type", StringComparison.OrdinalIgnoreCase)) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a type. Line: {1}.", token, state.LineNumber)); }

            if (!Enum.TryParse<MaterialReflectionToken>(values[2], true, out var reflectionType)) { throw new InvalidDataException(string.Format("Unable to parse <{0}> token. Line: {1}.", values[2], state.LineNumber)); }

            switch (reflectionType)
            {
                case MaterialReflectionToken.Sphere:
                    {
                        state.Material.ReflectionMap.Sphere = Map(state, token, values);
                        break;
                    }
                case MaterialReflectionToken.Cube_Top:
                    {
                        state.Material.ReflectionMap.CubeTop = Map(state, token, values);
                        break;
                    }
                case MaterialReflectionToken.Cube_Bottom:
                    {
                        state.Material.ReflectionMap.CubeBottom = Map(state, token, values);
                        break;
                    }
                case MaterialReflectionToken.Cube_Front:
                    {
                        state.Material.ReflectionMap.CubeFront = Map(state, token, values);
                        break;
                    }
                case MaterialReflectionToken.Cube_Back:
                    {
                        state.Material.ReflectionMap.CubeBack = Map(state, token, values);
                        break;
                    }
                case MaterialReflectionToken.Cube_Left:
                    {
                        state.Material.ReflectionMap.CubeLeft = Map(state, token, values);
                        break;
                    }
                case MaterialReflectionToken.Cube_Right:
                    {
                        state.Material.ReflectionMap.CubeRight = Map(state, token, values);
                        break;
                    }
            }
        }

        [MaterialToken(MaterialToken.Sharpness)]
        protected virtual void Sharpness(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }
            if (values.Length < 2) { throw new InvalidDataException(string.Format("The <{0}> statement must specify a sharpness value. Line: {1}.", token, state.LineNumber)); }
            if (values.Length != 2) { throw new InvalidDataException(string.Format("The <{0}> statement has too many values. Line: {1}.", token, state.LineNumber)); }

            state.Material.Sharpness = int.Parse(values[1], CultureInfo.InvariantCulture);
        }

        [MaterialToken(MaterialToken.TF)]
        protected virtual void TF(IMaterialReaderState state, MaterialToken token, string[] values)
        {
            if (state.Material == default) { throw new InvalidDataException(string.Format("The material name is not specified. Line: {0}", state.LineNumber)); }

            state.Material.TransmissionColor = Color(state, token, values);
        }
    }
}