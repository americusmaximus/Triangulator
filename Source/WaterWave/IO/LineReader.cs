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
using System.IO;

namespace WaterWave.IO
{
    public class LineReader : ILineReader
    {
        protected static readonly char[] Separators = new char[] { ' ', '\t' };

        public LineReader()
        {
            Headers = new List<string>();
        }

        public virtual IList<string> Headers { get; protected set; }

        public virtual string Line { get; protected set; }

        public virtual long LineNumber { get; protected set; }

        public virtual IEnumerable<string[]> Read(Stream stream)
        {
            if (stream == default) { throw new ArgumentNullException(nameof(stream)); }

            using (var reader = new StreamReader(stream))
            {
                var line = string.Empty;
                var isMultiLine = false;
                var isHeader = true;

                Headers = new List<string>();
                LineNumber = 0;

                while (!reader.EndOfStream)
                {
                    var currentLine = reader.ReadLine();

                    LineNumber++;

                    if (string.IsNullOrWhiteSpace(currentLine))
                    {
                        if (isHeader)
                        {
                            Headers.Add(string.Empty);
                        }

                        continue;
                    }

                    if (isMultiLine)
                    {
                        line = line.Substring(0, line.Length - 1) + currentLine;
                    }
                    else
                    {
                        line = currentLine;
                    }

                    line = line.Trim();

                    isMultiLine = line.EndsWith("\\", StringComparison.Ordinal);

                    if (isMultiLine) { continue; }

                    var commentIndex = line.IndexOf('#');

                    if (commentIndex == 0)
                    {
                        if (isHeader)
                        {
                            Headers.Add(line.Substring(1));
                        }

                        continue;
                    }

                    if (isHeader)
                    {
                        isHeader = false;
                    }

                    if (commentIndex > 0)
                    {
                        line = line.Substring(0, commentIndex);
                    }

                    Line = line;

                    yield return line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
    }
}
