# Full change history

### v1.7.0 (source/binary)
- Chocolatey install is available.
- A number of cli [convenience commands](doc/textrude_convenience.md) have been added to make it easier to process data files.
- Textrude can now emit [serialised representations](doc/format_conversion.md) of object trees
- Textrude can now guess the format of models based on the first few hundred bytes of content.
- Textrude now supports a set of flexible [time recognition and comparison](doc/builtIns.md) functions. 
- More examples added
- TextrudeInteractive is now much better at cancelling in-flight renders
- TextrudeInteractive now configures Webview to use APPDATALOCAL for cached data 

### v1.6.0 (source/binary)
- Reduce annoying visual flicker when resizing edit panes
- Integers larger than 32 bits supported in JSON files 
- textrude.exe now provides a convenient [pipe](doc/cmdPipe.md) command
- textrude.exe *render* command now offers a *verbose* option
- textrude.exe *info* command now shows full application path
- textrude.exe now supports [dynamic output](doc/dynamicOutput.md)
- Prototype Grouping methods in textrude namespace
- Improve templates for library autodoc
- Prototype [cpp](doc/lib.md#lib/cpp.sbn) library
- Prototype [C#](doc/lib.md#lib/csharp.sbn) library
- Prototype [text-processing](doc/lib.md#lib/line.sbn) library

### v1.5.0 
- Textrude now provides some simple [syntax extensions](doc/syntaxExtensions.md) over *classic* Scriban 
- A [create_library](doc/userLibrary.md) built-in method is now provided to make it easier to create libraries
- LoopLimit now removed and cancellation of long-running in-flight renders is supported.  This makes it easier to process large text files.
- **Textrude** can now read models from STDIN or from a URL, making it useful for processing the output of other commands.
- **Textrude** model/output specifiers can now include explicit format declarations.

### v1.4.0
- The Monaco text editor is now used for all edit panes including the template editor, definitions and include paths.
- A single Monaco edit pane is now used for multiple models/outputs for cleaner switching & improved responsiveness
- The view menu allows visible-whitespace to be toggled on and off
- Rudimentary syntax highlighting and intellisense are provided for the template editor
- Models and outputs can now be assigned names
- Help menu now includes a link to gitter-chat
- Model, template and output panes now support linking to files.
- Export/Build... menu now brings up a dialog to help build CLI options.
- Updated to latest Scriban for [multi-line pipes](https://github.com/scriban/scriban/pull/327)
- Special thanks to [Martin Hochstrasser](https://github.com/highstreeto) for
  - CAKE build support 
  - The fancy new logo! 

### v1.3.0 
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