# Build system integration

TextrudeInteractive is useful for prototyping and developing models and templates but integration with a build-system requires use of the *textrude.exe* command-line tool.  This version can do everythng that TextrudeInteractive does and has a few extra features:

- can read model data directly from a web server
- can read model data directly from stdin or Powershell pipe
- can write output to stdout 

## Getting help
- use ```textrude.exe --help``` to get a list of command
- use ```textrude.exe help``` *command* to get detailed help

## Basic rendering

The simplest invocation is simple to render a template file without a model and to display the output to stdout...

```textrude.exe render --template mytemplate.sbn```

Typically though, you will supply at least one model and wish to write the output to a file.  By default, **textrude** assigns the name *model* to the first model, *model1* to the second etc regardless of the file name and infers the model format from the file extension:


```
#render from a single model file and write the output to a file
textrude.exe render --models data.yaml --template mytemplate.sbn --output out.txt
```

```
#render using two models 
textrude.exe render --models data.yaml data2.csv --template mytemplate.sbn --output out.txt
```


## Advanced model specifiers
Model specifiers passed via  *--models* have up to 3 elements:

*format!***name=**path

### Path
The path can be:
- a file-name. Indicates the model will be read from a file
- a URL starting with http or https.  Indicates the model will be read from the URL.
- the '-' character.  Indicates the model will be read from stdin

### Name
The name is the identifier used from within the template. If it is omitted, *modelN* is used.

### Format
One of:
 - json
 - yaml
 - line
 - csv

If the format is omitted, *textrude* will supply a default format depending upon the kind of path.
- For file-names, the extension will be examined.  If the extension is not recognised the *lines* format is used.
- For a URL, *json* is used.
- For *stdin*, *lines* format is used.

## Advanced output specifiers
Output specifiers passed via  *--output* have up to 2 elements:

***name=**path

### Path
The path can be:
- a file-name. Indicates the model will be read from a file
- the '-' character.  Indicates the output will be written to stdout
  
### Name
The name is the identifier used from within the template. If it is omitted, *outputN* is used.

## Definitions
Definitions are supplied using the *--definitions* flag.  Each definition consists of a **key=value** assignment.

## Include-paths
Additional include paths for use by the template are supplied via the *--include* flag.

## Dynamic Output
Use the --dynamicOutput flag to enable dynamic output (i.e. to enable the template to define its own output set)
See [dynamicOutput.md] for further details

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

# Examples
```
#render a YAML model stored in file 'a.model'. The template will refer to it as 'm'.  Output is sent to stdout
textrude.exe render --models yaml!m=a.model --template template.sbn 

#Fetch json data from a url and render it to stdout
textrude.exe render --models http://getdata --template mytemplate.sbn 

#apply a template to a list of filenames and render to stdout
ls *.h | textrude.exe --models files=- --template proc.sbn

# render a template from data in a csv file. Write the main output stream to the result.txt file but show the stats stream in stdout
textrude.exe --models data.csv --template tpl.sbn --output output=result.txt stats=-



```







