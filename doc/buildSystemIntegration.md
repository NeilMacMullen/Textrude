# Build system integration

TextrudeInteractive is useful for prototyping and developing models and templates but integration with a build-system requires use of the *textrude.exe* command-line tool.  This version can do everythng that TextrudeInteractive does and has a few extra features:

## Getting help
- use ```textrude.exe --help``` to get a list of command
- use ```textrude.exe help``` *command* to get detailed help

## Basic rendering

```textrude.exe render --models model.yaml --template template.sbn --output out.txt```

If no *--output* is specified, the output will be written to stdout.

## Multiple input/output

```textrude.exe render --models "model0.Yaml" "model1.Json" "model2.Csv" --template "template.sbn" --output "out.txt" "out1.txt"```

## Multiple input/output with named models and outputs

```textrude.exe render --models "m1=model0.Yaml" "m2=model1.Json" "m3=model2.Csv" --template "template.sbn" --output "output.txt" "out2=out1.txt"```


## Definitions

```textrude.exe render --models model.yaml --template template.sbn --output out.txt --definitions "LANGUAGE=ENGLISH" "COUNTRY=UK"
```

## Include-paths

```textrude.exe render --models model.yaml --template template.sbn --output out.txt --include "d:\work\slib" "d:\work\lib2"
```
## Dependency checking

By default, the render pass is always run.  However, if the *--lazy* flag is supplied the Textrude will only render the output files if:
- any of the output files are missing
- any of the model files or the template file is newer than any of the output files.

Textrude is NOT able to perform dependency checking of library files include via the *include* directive.

## Return codes

0 - successful render or render skipped due to **lazy** flag 
1 - fault

## RenderFromFile

Sometimes it can be useful to be able to provide arguments from a file. The **renderFromFile** command allows the argument set to be passed in as a YAML or JSON structure.

```textrude.exe renderFromFile --arguments args.yaml```

You can easily generate example argument files by using the *project->export as invocation* option from TextrudeInteractive.









