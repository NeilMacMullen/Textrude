# ![Textude](img/textrude_logo.png)
[![NuGet](https://img.shields.io/nuget/v/textrude?label=Latest-version)](https://www.nuget.org/packages/textrude/)
[![Coverage Status](https://coveralls.io/repos/github/NeilMacMullen/Textrude/badge.svg?branch=main)](https://coveralls.io/github/NeilMacMullen/Textrude?branch=main) 
[![Join the chat at https://gitter.im/Textrude/community](https://badges.gitter.im/Textrude/community.svg)](https://gitter.im/Textrude/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

#### Downloads
[![GitHub release](https://img.shields.io/github/downloads-pre/NeilMacmullen/textrude/total?label=Github)](https://github.com/NeilMacMullen/textrude/releases) 
[![NuGet](https://img.shields.io/nuget/dt/textrude?label=Nuget)](https://www.nuget.org/packages/textrude/) 
[![Chocolatey](https://img.shields.io/chocolatey/dt/textrude?label=Chocolatey)](https://community.chocolatey.org/packages/textrude)


## Give a Star! :star:

If you like or are using this project please give it a star or leave some feedback in the [discussions](https://github.com/NeilMacMullen/Textrude/discussions/categories/send-a-smile) section. A little feedback goes a long way - thanks!

## What is it?

Textrude is a tool to feed CSV,YAML, JSON or plain-text files into [Scriban](https://github.com/scriban/scriban) templates. That makes it useful for:
 - **Code-generation**. Generate serializers, smart-enums, lookup tables etc from structured data
 - **Quick and dirty data-processing**. Pull a json file from a URL and extract the fields you're interested in or use the [convenience commands](doc/textrude_convenience.md) to process data files from the command line.
 - **Text processing**. Use line mode to filter the contents of large log files to get to the relevant sections 

Textrude comes in 3 flavours:
- **textrude.exe** is a Windows CLI tool for use from the console and within build-systems
- **textrude_linux** as above but for Linux
- **TextrudeInteractive** is a Windows UI tool for rapid prototyping and development of models and templates.

![Screenshot of TextrudeInteractive](img/textrudedemo.gif)

## Why use it?

For **code-generation**, Textrude's strengths are:
- Easy model (data) creation - use CSV for simple lists or YAML/JSON if you need structured data
- Low-ceremony syntax while retaining a fully functional programming language
- Supports multiple input models and multiple output files for a single template
- Easy to inject additional model context via environment variables or user-supplied definitions
- Built-in dependency checking integrates well with your build system and avoids unneccesary rebuilds
- Support for template re-use/libraries
- Real-time prototyping tool  (TextrudeInteractive)

For **text-processing** or **data-processing**
- Scriban is an easy to understand but relatively powerful scripting language
- **TextrudeInteractive** provides *immediate* feedback so you can see how the input is being processed by your script
- You can pipe text into **Textrude** from another command and reuse the templates you developed in **TextrudeInteractive** to shape the output
- **Textrude** can even pull JSON directly from a URL and feed it through a template.

## Download/build

**Textrude requires [.Net 9.0](https://dotnet.microsoft.com/download/dotnet9.0).  If it's not already on your machine you will be prompted to install it.**

**To run TextrudeInteractive v1.3 and up the [WebView2 runtime] is required.  If this is not already installed on your PC you can obtain it from (https://developer.microsoft.com/en-us/microsoft-edge/webview2/)**  (This is *not* required if you just want to run the CLI tool.)

 - If you just want the binaries,  [go here](doc/getBinaries.md).
 - Or download using [Chocolatey](https://community.chocolatey.org/) `choco install textrude` **Important - TextrudeInteractive is not currently correctly installed in Chocolatey release**
 - Alternatively, [building from source](doc/buildFromSource.md) is pretty easy and gives you access to the bleeding-edge! 
 - Or you can create a [Docker image](Docker.md)
 - Textrude is also available on [nuget](https://www.nuget.org/packages/textrude/)

## What's new

### 1.8.1 (source/binary)
- Retarget to Net9 and updated dependencies

### 1.8.0 (source/binary)
- Retarget to Net7
- Fix bug where a StackOverflowException could be thrown if textrude.to_csv/to_json/to_yaml was called on recursive object
- Add [snippet](doc/snippets.md) support for script editor (thanks to Olof Wistrand)

### v1.7.0 (source/binary)
- Chocolatey install is available.
- A number of cli [convenience commands](doc/textrude_convenience.md) have been added to make it easier to process data files.
- Textrude can now emit [serialised representations](doc/format_conversion.md) of object trees
- Textrude can now guess the format of models based on the first few hundred bytes of content.
- Textrude now supports a set of flexible [time recognition and comparison](doc/builtIns.md) functions. 
- More examples added
- TextrudeInteractive is now much better at cancelling in-flight renders
- TextrudeInteractive now configures Webview to use APPDATALOCAL for cached data 


[Full change history](doc/changeHistory.md)

## Documentation

- [Getting started with template generation](doc/gettingStarted.md)
- [Extended Scriban syntax](doc/syntaxExtensions.md)  
- [Built in helpers](doc/builtIns.md)
- [Library functions](doc/lib.md)
- [Environment variables and user-definitions](doc/environmentAndDefinitions.md)
- [Multiple models and/or output files](doc/multiModel.md)
- [Creating and using library functions](doc/userlibrary.md)
- [Using Textrude in a build system](doc/buildSystemIntegration.md)
- [Docker image](Docker.md)
- [Understanding file-linkages](doc/fileLinkage.md)
- [Constructing command lines](doc/exportInvocation.md)
- [command line piping](doc/cmdPipe.md)
- [Using snippets](doc/snippets.md)

## Credits and contributors
Textrude makes heavy use of the following components:
- [Scriban](https://github.com/scriban/scriban) as the template language engine
- [CommandLineParser](https://github.com/commandlineparser/commandline) for command-line parsing
- [CsvHelper](https://github.com/JoshClose/CsvHelper) for CSV deserialisation
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) for YAML deserialisation
- [Json.Net](https://www.newtonsoft.com/json) for Json deserialisation
- [Humanizr](https://github.com/Humanizr/Humanizer) for useful text-processing
- [MaterialDesignToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit),   [MaterialDesignExtensions](https://spiegelp.github.io/MaterialDesignExtensions) and [Ookii Dialogs](https://github.com/augustoproiete/ookii-dialogs-wpf)to make the UI a bit less clunky

Huge thanks to the contributors:
- [Martin Hochstrasser](https://github.com/highstreeto) - Docker support, general build enhancements, integration of the Monaco editor and the fancy logo!
- [Neil MacMullen](https://github.com/NeilMacMullen) - original application concept and implementation

## Help wanted 

If you fancy making Textrude better, I'd be happy to have help! Grab something from the issues list or suggest an idea. Alternatively you can contribute script snippets, improve the documentation or spread the word by writing an article!

## What's with the name 

It's short for Text-extrude but if you can't stop seeing it as Text-Rude you are not alone.

It is unrelated to both the rather cool (but apparently abandoned project) [Textruder](https://github.com/arrogantrobot/textruder) and the plastics company [Tex-Trude](http://www.tex-trude.com/)

