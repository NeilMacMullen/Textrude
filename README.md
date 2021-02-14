# Textrude

[![Coverage Status](https://coveralls.io/repos/github/NeilMacMullen/Textrude/badge.svg?branch=main)](https://coveralls.io/github/NeilMacMullen/Textrude?branch=main) [![Join the chat at https://gitter.im/Textrude/community](https://badges.gitter.im/Textrude/community.svg)](https://gitter.im/Textrude/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Give a Star! :star:

If you like or are using this project please give it a star or leave some feedback in the [discussions](https://github.com/NeilMacMullen/Textrude/discussions/categories/send-a-smile) section. A little feedback goes a long way - thanks!

## What is it?

Textrude is a cross-platform general-purpose code-generation tool.  It can easily import data from CSV,YAML, JSON  or plain-text files and apply [Scriban](https://github.com/scriban/scriban) templates to quickly scaffold output files. 

Templates and models can quickly be developed using the bundled TextrudeInteractive tool. A command-line executable is provided for easy integration with automated build-systems.

![Screenshot of TextrudeInteractive](img/textrudedemo.gif)

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

The current release is **v1.3.0**.

**Textrude requires [.Net 5.0](https://dotnet.microsoft.com/download/dotnet/5.0).  If it's not already on your machine you will be prompted to install it.**

**To run TextrudeInteractive v1.3 and up you must install the [WebView2 runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)**

 - If you just want the binaries,  [go here](doc/getBinaries.md).

 - Alternatively, [building from source](doc/buildFromSource.md) is pretty easy and gives you access to the bleeding-edge! 
  
 - Or you can create a [Docker image](Docker.md)

## What's new

### vNext (source only)
- ~~Note - template intellisense/code-completion is known to be broken in the current source build~~ 
- Update to Monaco 22.3 which supports an extra couple of languages
- Rewire font-size control, line numbers, wordwarp to model/output panes
- remove minimap from model/output panes
- added spinner to monaco panes and made white flash a bit briefer
- whitespace can be made visible in input/output panes
- reuse single Monaco edit pane for multiple models/outputs for cleaner switching & lower resource use
- definitions and includes are now move to main input section and used Monaco editor
- the template is now edited using Monaco and (rudimentary) syntax hightlight is applied
- models and outputs can now be renamed in TextrudeInteractive
- Textrude CLI now supports named models/outputs via "mymodel=d:/model.csv" syntax
- Basic auto-completion restored
- CAKE build support - thanks to [Martin Hochstrasser](https://github.com/highstreeto)
- Updated to latest Scriban for [multi-line pipes (yay!)](https://github.com/scriban/scriban/pull/327)
- Add link to textrude community chat in help menu
- Linked files can be toggled between absolute and relative paths
- Export/Build... menu now brings up a dialog to help build CLI options.

### v1.3.0 (source/binary)
- Models and outputs can be added/removed on a per-project basis
- Syntax highlighting for output panes
- Input/ouput panes can be "linked" to files and load/save are supported
- fontsize, wordwrap and line-number settings are now persisted
- warning dialog is now shown if the current project has unsaved changes
- default rendering throttle reduced to 50ms for better responsiveness
- Taskbar icon now shows jumplist, TextrudeInteractive can be started with name of project as parameter
- TextrudeInteractive now opens last used project when reopened
- TextrudeInteractive now uses the Monaco editor (from VS Code) hugely improving the syntax highlighting abilities.   Massive thanks to [Martin Hochstrasser](https://github.com/highstreeto) for this! 
- upgrade to latest Scriban which supports [captured variables in anonymous functions](https://github.com/scriban/scriban/issues/322) 

[Full change history](doc/changeHistory.md)

## Documentation

- [Getting started with template generation](doc/gettingStarted.md) 
- [Built in helpers](doc/builtIns.md)
- [Library functions](doc/lib.md)
- [Environment variables and user-definitions](doc/environmentAndDefinitions.md)
- [Multiple models and/or output files](doc/multiModel.md)
- [Creating and using library functions](doc/userlibrary.md)
- [Using Textrude in a build system](doc/buildSystemIntegration.md)
- [Docker image](Docker.md)

## Credits and contributors
Textrude makes heavy use of the following components:
- [Scriban](https://github.com/scriban/scriban) as the template language engine
- [CommandLineParser](https://github.com/commandlineparser/commandline) for command-line parsing
- [CsvHelper](https://github.com/JoshClose/CsvHelper) for CSV deserialisation
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) for YAML deserialisation
- [Json.Net](https://www.newtonsoft.com/json) for Json deserialisation
- [Humanizr](https://github.com/Humanizr/Humanizer) for useful text-processing
- [MaterialDesignToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit),   [MaterialDesignExtensions](https://spiegelp.github.io/MaterialDesignExtensions) and [Ookii Dialogs](https://github.com/augustoproiete/ookii-dialogs-wpf)to make the UI a bit less clunky
- [AvalonEdit](http://avalonedit.net/) for text-editing goodness

Huge thanks to the contributors:
- [Martin Hochstrasser](https://github.com/highstreeto) - Docker support, general build enhancements and integration of the Monaco editor
- [Neil MacMullen](https://github.com/NeilMacMullen) - original application concept and implementation


## Help wanted 

If you fancy making Textrude better, I'd be happy to have help! Grab something from the issues list or suggest an idea. Alternatively you can contribute script snippets, improve the documentation or spread the word by writing an article!


## What's with the name 

It's short for Text-extrude but if you can't stop seeing it as Text-Rude you are not alone.

It is unrelated to both the rather cool (but apparently abandoned project) [Textruder](https://github.com/arrogantrobot/textruder) and the plastics company [Tex-Trude](http://www.tex-trude.com/)

