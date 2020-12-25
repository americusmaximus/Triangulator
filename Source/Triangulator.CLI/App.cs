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

using ImageBox.Flipping;
using ImageBox.Splitting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WaterWave;

namespace Triangulator.CLI
{
    public static class App
    {
        static readonly Dictionary<string, string> Help = new Dictionary<string, string>()
        {
            { "angle",              "A floating point angle for the image rotation.\n                    Default value is \"0\" (zero)." },
            { "color",              "A color to fill empty pixels (if any) after the image\n                    rotation. Color can be specified as a name, ARGB integer,\n                    or a HEX value.\n                    Example: \"red\", \"-65536\", \"#00ff0000\".\n                    Default value is \"Transparent\"."},
            { "config",             "A path to a JSON file with the rasterization configuration.\n                    The config file can be used to set all values, or to set\n                    base configuration while overriding some of the parameters\n                    by providing additional command line parameters."},
            { "flip",               "An axis for the image flip.\n                    Possible values are \"None\", \"Horizontal\", \"Vertical\",\n                    and \"Both\".\n                    Default value is \"None\"." },
            { "ignoretransparent",  "A boolean value indication if the transparent pixels must\n                    be skipped from triangulation.\n                    Default value is \"TRUE\"." },
            { "image",              "A path to the input image file.\n                    Image is a required parameter.\n                    Supported image formats are BMP, EMF, EXIF, GIF, ICON,\n                    JPEG, PNG, TIFF, and WMF." },
            { "maxheight",          "A floating point value of maximum possible height for the\n                    triangulated mesh. Maximum value will be applied to\n                    white pixels (if any).\n                    Default value is \"100\" meters." },
            { "minheight",          "A floating point value of minimum possible height for the\n                    triangulated mesh. Maximum value will be applied to\n                    black pixels (if any).\n                    Default value is \"0\" meters." },
            { "offsetx",            "A floating point value of an offset by X-axis for the\n                    triangulated mesh.\n                    Default value is \"0\" (zero) meters." },
            { "offsetz",            "A floating point value of an offset by Z-axis for the\n                    triangulated mesh.\n                    Default value is \"0\" (zero) meters." },
            { "output",             "A path to the output file. If there is going to be\n                    muplitple outut files, the name will be automatically\n                    modified to fit the template."},
            { "scalex",             "A positive floating point value of a pixel size on X-axis\n                    in the output mesh.\n                    Default value is \"1\" meter." },
            { "scalez",             "A positive floating point value of a pixel size on Z-axis\n                    in the output mesh.\n                    Default value is \"1\" meter." },
            { "split",              "An image split type.\n                    Possible values are \"Pixel\" and \"Piece\".\n                    Default value is \"Pixel\"."},
            { "splitx",             "A positive integer number of horizontal units for\n                    image splitting.\n                    The value is required for the \"Split\" mode."},
            { "splity",             "A positive integer number of vertical units for\n                    image splitting.\n                    The value is required for the \"Split\" mode."}
        };

        public static string GetHelpString(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter)) { return "No help available for <blank> parameter."; }

            if (parameter == "c") { parameter = "config"; }

            if (Help.TryGetValue(parameter, out var result))
            {
                return result;
            }

            return string.Format("No help available for <{0}> parameter.", parameter);
        }

        public static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine(string.Format("Triangulator [Version {0}]", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            Console.WriteLine("Copyright © 2020 Americus Maximus.");
            Console.WriteLine();

            if (args == default || args.Length == 0)
            {
                Console.WriteLine("Usage: Triangulator.CLI [options|parameters].");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine(" c=[path]|config=[path]           Rasterize font with specified configuration.");
                Console.WriteLine(" h|help                           Display help.");
                Console.WriteLine(" h=[parameter]|help=[parameter]   Display help for a specified parameter.");
                Console.WriteLine(" v|version                        Display version.");
                Console.WriteLine(" [parameters]                     Rasterize a font with specified parameters.");

                return 0;
            }

            var parameters = args.ToArray();

            // Version takes priority over anything else
            if (parameters.Any(a => a.ToLowerInvariant() == "v") || parameters.Any(a => a.ToLowerInvariant() == "version"))
            {
                Console.WriteLine(string.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
				
                return 0;
            }

            // Help is a second highest priority
            if (parameters.Any(a => a.ToLowerInvariant() == "h") || parameters.Any(a => a.ToLowerInvariant() == "help"))
            {
                Console.WriteLine("Help:");

                foreach (var x in Help.OrderBy(h => h.Key))
                {
                    Console.Write(x.Key.PadRight(20));
                    Console.WriteLine(x.Value);
                    Console.WriteLine();
                }

                return 0;
            }

            if (parameters.Any(a => a.ToLowerInvariant().StartsWith("h=")) || parameters.Any(a => a.ToLowerInvariant().StartsWith("help=")))
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("h=") || a.ToLowerInvariant().StartsWith("help=")).OrderBy(a => a).ToArray();

                Console.WriteLine("Help:");

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("h=")) { ars[x] = ars[x].Substring(2, ars[x].Length - 2); }
                    if (ars[x].StartsWith("help=")) { ars[x] = ars[x].Substring(5, ars[x].Length - 5); }

                    Console.Write(ars[x].PadRight(20));
                    Console.WriteLine(GetHelpString(ars[x]));
                    Console.WriteLine();
                }

                return 0;
            }

            // Load configuration file, if applicable
            var config = default(TriangulatorRequest);

            if (parameters.Any(a => a.ToLowerInvariant().StartsWith("c=")) || parameters.Any(a => a.ToLowerInvariant().StartsWith("config=")))
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("c=") || a.ToLowerInvariant().StartsWith("config=")).OrderBy(a => a).ToArray();

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("c=")) { ars[x] = ars[x].Substring(2, ars[x].Length - 2); }
                    if (ars[x].StartsWith("config=")) { ars[x] = ars[x].Substring(7, ars[x].Length - 7); }
                }

                if (ars.Length != 1)
                {
                    Console.WriteLine(string.Format("There can be only one configuration file specified. Found <{0}> files:\n{1}", ars.Length, string.Join(Environment.NewLine, ars)));

                    return -1;
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(ars[0]))
                    {
                        Console.WriteLine("Configuration file path cannot be empty.");

                        return -1;
                    }

                    var normalizedPath = NormalizeFileName(ars[0]);

                    if (!File.Exists(normalizedPath))
                    {
                        Console.WriteLine(string.Format("Configuration file <0> not found.", normalizedPath));

                        return -1;
                    }

                    var json = File.ReadAllText(normalizedPath, Encoding.UTF8);
                    config = JsonConvert.DeserializeObject<TriangulatorRequest>(json);

                    if (config == default)
                    {
                        Console.WriteLine(string.Format("Unable to deserialize the content of the configuration file <{0}>. Unknown reason.", normalizedPath));

                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to deserialize the content of the configuration file. Please see the error below.");
                    Console.WriteLine(ex.ToString());

                    return -1;
                }
            }

            // Create default configuration, if needed
            if (config == default)
            {
                config = new TriangulatorRequest();
            }

            // Image
            var image = default(Image);
            string imageFileName = string.Empty;

            if (!parameters.Any(a => a.ToLowerInvariant().StartsWith("image=")))
            {
                Console.WriteLine("Image parameter is required. Please see help for details.");

                return -1;
            }
            else
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("image=")).OrderBy(a => a).ToArray();

                if (ars.Length != 1)
                {
                    Console.WriteLine("There can be only one image parameter.");

                    return -1;
                }

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("image=")) { ars[x] = ars[x].Substring(6, ars[x].Length - 6); }
                }

                if (string.IsNullOrWhiteSpace(ars[0]))
                {
                    Console.WriteLine("Image file path cannot be empty.");

                    return -1;
                }

                var normalizedPath = NormalizeFileName(ars[0]);

                if (!File.Exists(normalizedPath))
                {
                    Console.WriteLine(string.Format("Image file <{0}> not found.", normalizedPath));

                    return -1;
                }

                try
                {
                    image = Image.FromFile(normalizedPath);
                    imageFileName = normalizedPath;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Unable to load image <{0}>.", normalizedPath));
                    Console.WriteLine(ex.ToString());

                    return -1;
                }
            }

            // Output
            var output = string.Empty;
            if (parameters.Any(a => a.ToLowerInvariant().StartsWith("output=")))
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("output=")).OrderBy(a => a).ToArray();

                if (ars.Length != 1)
                {
                    Console.WriteLine("There can be only one output parameter.");

                    return -1;
                }

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("output=")) { ars[x] = ars[x].Substring(7, ars[x].Length - 7); }
                }

                if (string.IsNullOrWhiteSpace(ars[0]))
                {
                    Console.WriteLine("Output path cannot be empty.");

                    return -1;
                }

                output = NormalizeFileName(ars[0]);
            }

            // Process parameters
            foreach (var p in parameters)
            {
                if (!p.Contains("="))
                {
                    Console.WriteLine(string.Format("Unable to parse <{0}> parameter. Skipping it.", p));
                    continue;
                }

                var key = p.Substring(0, p.IndexOf("=")).ToLowerInvariant();

                if (key == "c" || key == "config" || key == "t" || key == "image" || key == "output") { continue; }

                var pms = parameters.Where(ar => ar.ToLowerInvariant().StartsWith(key + "=")).ToArray();

                if (pms.Length != 1)
                {
                    Console.WriteLine(string.Format("There can be only one parameter <{0}>", key));

                    return -1;
                }

                var value = p.Substring(key.Length + 1, p.Length - key.Length - 1);

                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine(string.Format("Empty value for <{0}> is not allowed.", key));

                    return -1;
                }

                if (key == "angle")
                {
                    if(float.TryParse(value, out var floatValue))
                    {
                        config.Angle = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse <{0}> as a floating point number. Using default value of <{1}>.", value, config.Angle));
                    }
                }
                else if (key == "color")
                {
                    if (TryParseColor(value, out var color))
                    {
                        config.Color = color;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a color. Using default value of <{1}>.", value, config.Color));
                    }
                }
                else if (key == "flip")
                {
                    if (Enum.TryParse<FlipType>(value, true, out var flipValue))
                    {
                        config.FlipType = flipValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a flip value. Using default value of <{1}>.", value, config.FlipType));
                    }
                }
                else if (key == "ignoretransparent")
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        config.IgnoreTransparent = boolValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a boolean. Using default value of <{1}>.", value, config.IgnoreTransparent));
                    }
                }
                else if (key == "maxheight")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        config.MaximumHeight = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating point number. Using default value of <{1}>.", value, config.MaximumHeight));
                    }
                }
                else if (key == "minheight")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        config.MinimumHeight = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating point number. Using default value of <{1}>.", value, config.MinimumHeight));
                    }
                }
                else if (key == "offsetx")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        config.OffsetX = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating point number. Using default value of <{1}>.", value, config.OffsetX));
                    }
                }
                else if (key == "offsetz")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        config.OffsetZ = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating point number. Using default value of <{1}>.", value, config.OffsetZ));
                    }
                }
                else if (key == "scalex")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        if (floatValue < float.Epsilon)
                        {
                            Console.WriteLine(string.Format("Minimum value of a Scale X is <{0}>, current value is <{1}>.", float.Epsilon, value));

                            return -1;
                        }

                        config.ScaleX = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating point number. Using default value of <{1}>.", value, config.ScaleX));
                    }
                }
                else if (key == "scalez")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        if (floatValue < float.Epsilon)
                        {
                            Console.WriteLine(string.Format("Minimum value of a Scale Z is <{0}>, current value is <{1}>.", float.Epsilon, value));

                            return -1;
                        }

                        config.ScaleZ = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating point number. Using default value of <{1}>.", value, config.ScaleZ));
                    }
                }
                else if (key == "split")
                {
                    if (Enum.TryParse<SplitType>(value, true, out var splitValue))
                    {
                        config.SplitType = splitValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a split value. Using default value of <{1}>.", value, config.SplitType));
                    }
                }
                else if (key == "splitx")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        if (intValue < 1)
                        {
                            Console.WriteLine(string.Format("Minimum value of a Split X is <{0}>, current value is <{1}>.", 1, value));

                            return -1;
                        }

                        config.SplitX = intValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as an integer number. Using default value of <{1}>.", value, config.SplitX));
                    }
                }
                else if (key == "splity")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        if (intValue < 1)
                        {
                            Console.WriteLine(string.Format("Minimum value of a Split Y is <{0}>, current value is <{1}>.", 1, value));

                            return -1;
                        }

                        config.SplitX = intValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as an integer number. Using default value of <{1}>.", value, config.SplitY));
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Unknown parameter <{0}>. Skipping it.", key));
                }
            }

            // Execution
            return Execute(image, config, output);
        }

        public static string NormalizeFileName(string fileName)
        {
            return string.IsNullOrWhiteSpace(Path.GetDirectoryName(fileName)) ? Path.Combine(Environment.CurrentDirectory, fileName) : fileName;
        }

        public static bool TryParseColor(string value, out Color color)
        {
            if (value.StartsWith("#"))
            {
                // Example: #00ff0000
                if (int.TryParse(value.Substring(1, value.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var intHexValue))
                {
                    color = Color.FromArgb(intHexValue);

                    return true;
                }
            }

            // Example: -65536
            if (int.TryParse(value, out var intValue))
            {
                color = Color.FromArgb(intValue);

                return true;
            }

            // Example: Red
            var property = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(p => p.Name.ToLowerInvariant() == value);

            color = property == default ? Color.Transparent : (Color)property.GetValue(default, default);

            return property != default;
        }

        private static int Execute(Image image, TriangulatorRequest config, string output)
        {
            try
            {
                Console.WriteLine(string.Format("Triangulating <{0}> pixels.", image.Width * image.Height));

                var objs = new Triangulator().Triangulate(image, config);

                // Single
                if (objs.Length == 1 && objs[0].Length == 1)
                {
                    ObjFile.Write(objs[0][0], output);

                    Console.WriteLine(string.Format("Obj file is saved as <{0}>.", output));

                    return 0;
                }

                // Multiple
                var fileNameDirectory = Path.GetDirectoryName(output);
                var fileNameTemplate = Path.GetFileNameWithoutExtension(output);

                for (var x = 0; x < objs.Length; x++)
                {
                    var line = objs[x];

                    for (var y = 0; y < line.Length; y++)
                    {
                        ObjFile.Write(line[y], Path.Combine(fileNameDirectory, string.Format("{0}.{1:D4}.{2:D4}.obj", fileNameTemplate, y, x)));
                    }
                }

                Console.WriteLine(string.Format("Successfully saved obj files to <{0}>.", output));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Unable to save file(s) to <{0}>. Please see the error below.", output));
                Console.WriteLine(ex.ToString());

                return -1;
            }

            return 0;
        }
    }
}
