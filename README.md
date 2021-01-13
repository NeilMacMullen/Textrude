# Textrude

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## What is it?

Textrude is a cross-platform general-purpose code-generation tool suitable for integration with build-systems.  It can easily import data from CSV,YAML, JSON  or plain-text files and apply [Scriban](https://github.com/scriban/scriban) templates to generate output files. 

Templates and models can quickly be developed using the bundled TextrudeInteractive tool (windows only).

![Screenshot of TextrudeInteractive](img/ex1.png)

## Why use it?

Let's face it, there are any number of code-generation technologies you might consider.  Textrude's strengths are:

- Easy model (data) creation - use CSV for simple lists or YAML/JSON if you need structured data
- Low-ceremony syntax while retaining a fully functional programming language
- Well suited for systems where it's necessary to generate multiple representations of the same model
- Supports multiple input models and multiple output files for a single template
- Easy to inject context via environment or defines
- Built-in dependency checking integrates well with your build system
- Real-time prototyping tool  (TextrudeInteractive)


## Download

Get the latest binaries from the --link here--- or build yourself from source.

##Known issues...
- YAML and CSV deserialisers don't always interpret numbers and strings correctly
- TextrudeInteractive UI is a bit rudimentary

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

