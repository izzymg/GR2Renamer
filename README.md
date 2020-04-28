# GR2Renamer

Recurses through a directory of .gr2 files, inspecting their mesh, animation and skeleton data,
and renaming them accordingly.

### Setup

Requires Visual Studio 2017

Place `granny2.dll` & `granny2_x64_ESO.dll` into the root directory, ensure VS copies to output directory.

Link the project to [lslib](https://github.com/Norbyte/lslib/)

### Usage

Set max files to an integer to limit it to a set amount of files before quitting.

./gr2renamer.exe [directory] [maxFiles]?