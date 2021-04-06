# Textrude pre-built binaries

## Installation

First, ensure that you have the .Net 5 **desktop** runtime installed.  This is available [from Microsoft](https://dotnet.microsoft.com/download/dotnet/5.0)   (*If you are only planning to use the CLI tool on Linux then you can get away with the standard runtime rather than the desktop version*)

Next, if you are intending to use the *TextrudeInteractive* GUI tool, install the [WebView2 runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703). 

Go to the [releases](https://github.com/NeilMacMullen/Textrude/releases) section, look for the latest release (at the top!) and download the *textrude_vxxxx.zip* asset. Unzip the contents to a folder called Textrude (for example)

## What do you get?

After you've extract the files you should see something like

```
C:\TEXTRUDE
│   Textrude.exe                 <-- This is the Windows command line version of Textrude  
│   TextrudeInteractive.exe      <-- This is the Windows UI tool 
|   Textrude_linux               <-- This is the Linux version of the command line tool
│
├───lib                          <-- This folder contains "standard" scripts shipped with Textrude 
│       misc.sbn
│       ... more
│
└───examples                     <-- This folder contains some example projects.  You're encouraged to
        intro.texproj            <-- load some up and look around 
        ... more
```

You can run the executables directly from this folder; no further installation steps are required though if you are planning to use the CLI tools you may want to edit your path to include the folder.

## Important note about moving the executables

The executables are self-contained and can run anywhere but they do expect to find the *lib* folder in the same directory. Later versions also required the *WebView2Loader.dll* to be available in the same folder as the exes.

