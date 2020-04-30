# GR2Renamer

Recurses through a directory of .gr2 files, inspecting their mesh, animation and skeleton data,
and renaming them accordingly.

### Setup

Requires Visual Studio 2017

Link the project to [lslib](https://github.com/Norbyte/lslib/)

### Usage

See LSLib for DLL deps.

Set max files to an integer to limit it to a set amount of files before quitting.

./gr2renamer.exe [directory] [maxFiles]?
