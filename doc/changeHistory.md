# Full change history

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