# Textrude pre-built binaries

## Installation

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

## Important note about moving the executables

The executables are self-contained and can run anywhere but they do expect to find the *lib* folder in the same directory.o 

