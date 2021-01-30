# Textrude

## Give a Star! :star:

If you like or are using this project please give it a star or leave some feedback in the [discussions](https://github.com/NeilMacMullen/Textrude/discussions/categories/send-a-smile) section. A little feedback goes a long way - thanks!

## What is it?

Textrude is a cross-platform general-purpose code-generation tool.  It can easily import data from CSV,YAML, JSON  or plain-text files and apply [Scriban](https://github.com/scriban/scriban) templates to quickly scaffold output files. 

Templates and models can quickly be developed using the bundled TextrudeInteractive tool. A command-line executable is provided for easy integration with automated build-systems.

![Screenshot of TextrudeInteractive](img/ex1.png)

## Why use it?

Let's face it, there are any number of code-generation technologies you might consider.  Textrude's strengths are:

- Easy model (data) creation - use CSV for simple lists or YAML/JSON if you need structured data
- Low-ceremony syntax while retaining a fully functional programming language
- Supports multiple input models and multiple output files for a single template
- Easy to inject additional model context via environment variables or user-supplied definitions
- Built-in dependency checking integrates well with your build system
- Allows template re-use via include mechanism
- Real-time prototyping tool  (TextrudeInteractive)

## Download/build

The current release is **v1.2.0**.

**Textrude requires [.Net 5.0](https://dotnet.microsoft.com/download/dotnet/5.0).  If it's not already on your machine you will need to install it.** 

 - If you just want the binaries,  [go here](doc/getBinaries.md).

 - Alternatively, [building from source](doc/buildFromSource.md) is pretty easy and gives you access to the bleeding-edge! 
  
 - Or you can create a [Docker image](Docker.md)


## What's new

### vNext (for source builds)
- Models and outputs can be added/removed on a per-project basis
- Syntax highlighting for output panes
- Input/ouput panes can be "linked" to files and load/save are supported

### v1.2.0
- Turn StrictVariables back on
- AvalonEdit used for edit boxes in TextrudeInteractive allowing:
  - line numberss
  - text size can be changed
  - CTRL-Z etc
  - basic auto-complete when '.' is typed
- Docker image support - thanks to [Martin Hochstrasser](https://github.com/highstreeto)

### v1.1.0
- first public release

## Documentation

- [Getting started with template generation](doc/gettingStarted.md) 
- [Built in helpers](doc/builtIns.md)
- [Library functions](doc/lib.md)
- [Environment variables and user-definitions](doc/environmentAndDefinitions.md)
- [Multiple models and/or output files](doc/multiModel.md)
- [Creating and using library functions](doc/userlibrary.md)
- [Using Textrude in a build system](doc/buildSystemIntegration.md)
- [Docker image](Docker.md)

## Credits
Textrude makes heavy use of the following components:
- [Scriban](https://github.com/scriban/scriban) as the template language engine
- [CommandLineParser](https://github.com/commandlineparser/commandline) for command-line parsing
- [CsvHelper](https://github.com/JoshClose/CsvHelper) for CSV deserialisation
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) for YAML deserialisation
- [Json.Net](https://www.newtonsoft.com/json) for Json deserialisation
- [Humanizr](https://github.com/Humanizr/Humanizer) for useful text-processing
- [MaterialDesignToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit),   [MaterialDesignExtensions](https://spiegelp.github.io/MaterialDesignExtensions) and [Ookii Dialogs](https://github.com/augustoproiete/ookii-dialogs-wpf)to make the UI a bit less clunky
- [AvalonEdit](http://avalonedit.net/) for text-editing goodness



## Help wanted 

If you fancy making Textrude better, I'd be happy to have help! Grab something from the issues list or suggest an idea. Alternatively you can contribute script snippets, improve the documentation or spread the word by writing an article!


## What's with the name 

It's short for Text-extrude but if you can't stop seeing it as Text-Rude you are not alone.

It is unrelated to both the rather cool (but apparently abandoned project) [Textruder](https://github.com/arrogantrobot/textruder) and the plastics company [Tex-Trude](http://www.tex-trude.com/)

