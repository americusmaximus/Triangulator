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
using System.IO;
using WaterWave.IO;

namespace WaterWave
{
    public class MaterialFile
    {
        public static MaterialCollection Read(string path)
        {
            if (path == default) { throw new ArgumentNullException(nameof(path)); }
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentException(string.Format("{0} cannot be empty.", nameof(path)), nameof(path)); }

            if (!File.Exists(path)) { throw new FileNotFoundException("File not found!", path); };

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return new MaterialReader().Read(stream);
            }
        }

        public static MaterialCollection Read(Stream stream)
        {
            if (stream == default) { throw new ArgumentNullException(nameof(stream)); }

            return new MaterialReader().Read(stream);
        }

        public void Write(MaterialCollection collection, string path)
        {
            if (path == default) { throw new ArgumentNullException(nameof(path)); }
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentException(string.Format("{0} cannot be empty.", nameof(path)), nameof(path)); }

            using (var writer = new StreamWriter(path))
            {
                new ObjMaterialWriter().Write(collection, writer);
            }
        }

        public void Write(MaterialCollection collection, Stream stream)
        {
            if (stream == default) { throw new ArgumentNullException(nameof(stream)); }

            using (var writer = new StreamWriter(stream))
            {
                new ObjMaterialWriter().Write(collection, writer);
            }
        }
    }
}