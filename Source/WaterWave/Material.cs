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

namespace WaterWave
{
    public class Material
    {
        public Material()
        {
            DissolveFactor = 1.0f;
            IlluminationModel = 2;
            OpticalDensity = 1.0f;
            ReflectionMap = new MaterialReflectionMap();
            Sharpness = 60;
        }

        public Material(string name) : this()
        {
            Name = name;
        }

        public virtual MaterialColor AmbientColor { get; set; }

        public virtual MaterialMap AmbientMap { get; set; }

        public virtual MaterialMap BumpMap { get; set; }

        public virtual MaterialMap DecalMap { get; set; }

        public virtual MaterialColor DiffuseColor { get; set; }

        public virtual MaterialMap DiffuseMap { get; set; }

        public virtual MaterialMap DispMap { get; set; }

        public virtual float DissolveFactor { get; set; }

        public virtual MaterialMap DissolveMap { get; set; }

        public virtual MaterialColor EmissiveColor { get; set; }

        public virtual MaterialMap EmissiveMap { get; set; }

        public virtual int IlluminationModel { get; set; }

        public virtual bool IsAntiAliasingEnabled { get; set; }

        public virtual bool IsHaloDissolve { get; set; }

        public virtual string Name { get; set; }

        public virtual float OpticalDensity { get; set; }

        public virtual MaterialReflectionMap ReflectionMap { get; protected set; }

        public virtual int Sharpness { get; set; }

        public virtual MaterialColor SpecularColor { get; set; }

        public virtual float SpecularExponent { get; set; }

        public virtual MaterialMap SpecularExponentMap { get; set; }

        public virtual MaterialMap SpecularMap { get; set; }

        public virtual MaterialColor TransmissionColor { get; set; }
    }
}