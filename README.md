# Triangulator
Triangulator is a C# height map image to Wavefront .obj Converter library with a wide range of options for user to use. Triangulator is a library, as well as CLI and GUI interfaces for it.

Triangulator is a library that runs on .Net Framework 4.0, 4.5, 4.7, 4.8, .Net Core 3.1, as well as .Net 5. Please see Build and Usage sections below for details.

## Example
The following example shows conversion of a gray scale height map image conversion to a Wavefront .obj file, and a screen shot of the result in Blender 2.83.

![Height Map](https://github.com/americusmaximus/Triangulator/blob/main/Docs/hm.png)

![Height Map Screen Shot](https://github.com/americusmaximus/Triangulator/blob/main/Docs/screenshot.png)

## Build
### Windows
#### Visual Studio
Open one of the solutions and build it. Please see `<TargetFrameworks>` node in the `.csproj` files to add or remove target frameworks for the build.
#### CLI
To build the solution please use following command:

> dotnet build Triangulator.CLI.sln --configuration Release

To build the solution for only one of the target frameworks please use the following command that shows an example of building for .Net 5.

> dotnet build Triangulator.CLI.sln --framework net50 -- configuration Release

To publish the code you always have to specify the target framework since `dotnet` doesn't support publishing multi-target projects.

> dotnet publish Triangulator.CLI.sln --framework net50 --configuration Release

**Note**: `dotnet` is unable to build the UI for any of the target frameworks.

### Linux
#### CLI
Please see the CLI section of building the code under Windows.

### Dependencies
#### Windows
##### ImageBox
Triangulator depends on [ImageBox](https://github.com/americusmaximus/ImageBox) for image manipulation like rotation, flipping, splitting, etc.

##### WaterWave
Triangulator depends on [WaterWave](https://github.com/americusmaximus/WaterWave) for creation of .obj files.

#### Linux
##### ImageBox
Triangulator depends on [ImageBox](https://github.com/americusmaximus/ImageBox) for image manipulation like rotation, flipping, splitting, etc.

##### WaterWave
Triangulator depends on [WaterWave](https://github.com/americusmaximus/WaterWave) for creation of .obj files.

##### LibGdiPlus
.Net on Linux depends on `libgdiplus` for image manipulation.

In case you see errors mentioning the following:

> The type initializer for 'Gdip' threw an exception.

or

> Unable to load DLL 'libgdiplus': The specified module could not be found.

you have to install libgdiplus library on your computer, which you can do by executing the following command:

> sudo apt install libgdiplus
 
## Use
### Windows
#### CLI
Triangulator CLI on Windows 7

![Triangulator CLI on Windows 7](https://github.com/americusmaximus/Triangulator/blob/main/Docs/Triangulator.CLI.Win.7.png)

Below is the output of running a help command
>Triangulator.CLI.exe h

##### angle
A floating point angle for the image rotation.                     Default value is "0" (zero).

##### color
A color to fill empty pixels (if any) after the image rotation. Color can be specified as a name, ARGB integer, or a HEX value. Example: "red", "-65536", "#00ff0000". Default value is "Transparent".

##### config
A path to a JSON file with the rasterization configuration. The config file can be used to set all values, or to set base configuration while overriding some of the parameters by providing additional command line parameters.

##### flip
An axis for the image flip. Possible values are "None", "Horizontal", "Vertical", and "Both". Default value is "None".

##### ignoretransparent
A boolean value indication if the transparent pixels must be skipped from triangulation. Default value is "TRUE".

##### image
A path to the input image file. Image is a required parameter. Supported image formats are BMP, EMF, EXIF, GIF, ICON, JPEG, PNG, TIFF, and WMF.

##### maxheight
A floating point value of maximum possible height for the triangulated mesh. Maximum value will be applied to white pixels (if any). Default value is "100" meters.

##### minheight
 A floating point value of minimum possible height for the triangulated mesh. Maximum value will be applied to black pixels (if any). Default value is "0" meters.

##### offsetx
A floating point value of an offset by X-axis for the triangulated mesh. Default value is "0" (zero) meters.

##### offsetz
A floating point value of an offset by Z-axis for the triangulated mesh. Default value is "0" (zero) meters. 

##### output
A path to the output file. If there is going to be multiple output files, the name will be automatically modified to fit the template.

##### scalex
A positive floating point value of a pixel size on X-axis in the output mesh. Default value is "1" meter.

##### scalez
A positive floating point value of a pixel size on Z-axis in the output mesh. Default value is "1" meter.

##### split
An image split type. Possible values are "Pixel" and "Piece". Default value is "Piece".

##### splitx
A positive integer number of horizontal units for image splitting. The value is required for the "Split" mode.

##### splity
A positive integer number of vertical units for image splitting. The value is required for the "Split" mode.

#### Example
>Triangulator.CLI.exe image=input.png output=hm.obj

#### UI
Triangulator UI runs on Windows exclusively. It allows for easy and dynamic preview when applicable, as well as ease of use without a need to remember CLI options.

Triangulator on Windows 7

![Triangulator UI on Windows 7](https://github.com/americusmaximus/Triangulator/blob/main/Docs/Triangulator.UI.Win.7.png)

Triangulator on Windows 10

![Triangulator UI on Windows 10](https://github.com/americusmaximus/Triangulator/blob/main/Docs/Triangulator.UI.Win.10.png)

### Linux
#### CLI
Triangulator CLI on xUbuntu 20.04

![Triangulator CLI on xUbuntu 20.04](https://github.com/americusmaximus/Triangulator/blob/main/Docs/Triangulator.CLI.xUbuntu.20.04.png)

Please see detailed description and example of the calls in Windows CLI section. Please note the differences in calling the CLI.

On Linux you have to call dotnet and provide path to the Triangulator.CLI.dll as a first parameter, the Triangulator parameters must follow afterward, please see example below:

>dotnet Triangulator.CLI.dll [parameters]