# Textrude

## Give a Star! :star:

If you like or are using this project please give it a star or leave some feedback in the [discussions](https://github.com/NeilMacMullen/Textrude/discussions/categories/send-a-smile) section. Thanks!

## What is it?

Textrude is a cross-platform general-purpose code-generation tool.  It can easily import data from CSV,YAML, JSON  or plain-text files and apply [Scriban](https://github.com/scriban/scriban) templates to quickly scaffold output files. 

Templates and models can quickly be developed using the bundled TextrudeInteractive tool.

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

## Download

Get pre-built binaries from the [Releases](https://github.com/NeilMacMullen/Textrude/releases) area or build yourself from source.


##Known issues...
- YAML and CSV deserialisers will always attempt to force strings that look like numbers or booleans into that format rather than leaving them as strings.  Most of the time this does not matter but please raise an issue if this causes particular problems
- TextrudeInteractive does not warn when closing if project is dirty.
- Textrude.exe is untested on Linux - please raise an issue if you run into problems

## Documentation
- [Getting started with template generation](doc/gettingStarted.md) 
- Built in functions and helpers
- Creating your own library of functions and templates
- Using Textrude in a build system
- advanced usage - multiple models and/or output files
- use the exportInvocation

## Credits

Textrude makes heavy use of the following components:
- [Scriban](https://github.com/scriban/scriban) as the template language engine
- [CsvHelper](https://github.com/JoshClose/CsvHelper) for command-line parsing
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) for YAML deserialisation
- [Json.Net](https://www.newtonsoft.com/json) for Json deserialisation

## Contributors guide
Adding new model deserialisers...
Architecture...

"Quick and dirty" - use jsonserialiser for new format types ... BUT can lose type information - particularly with numbers

file -> [Deserialiser] -> JObject -> ScriptObject


[Templatetext]
[modelText] 

[definionsText] -> 
[envs] 

Extensions
--binding custom dotnet functions- build your own version

Help wanted 
TextrudeInteractive has a UI only a developer could love and is very much a "scratch" project.  Help make it better....
Contribute to library methods
Test on linux
Documentation


## What's with the name 
It's short for Text-extrude but if you can't stop seeing it as Text-Rude you are not alone.

It is unrelated to the rather cool (but apparently abandoned project) [Textruder](https://github.com/arrogantrobot/textruder) or to plastics company [Tex-Trude](http://www.tex-trude.com/)

