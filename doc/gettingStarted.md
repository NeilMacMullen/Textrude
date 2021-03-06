# Getting started

*The easiest way to get started with Textrude and Scriban is to play with TextrudeInteractive but if you want to charge ahead and use the commmand-line tool, refer to the build-system integration documentation*


## Understanding the template flow 
A typical scenario relies on input from:
- one or more *models*.  These are sets of structured data supplied via CSV/YAML/JSON or text files
- environment variables inherited from the current process. 
- a list of user-supplied *definitions*

The input is acted on by a *template* and rendered to one or more output files.

The collection of inputs and outputs is saved as a *project*.

The *examples* folder contains a number of simple projects which demonstrate the main features. If you use the *Project/Load* menu command to load the *04_multiple_models_and_outputs* you should see something similar to the picture below.

![Annotated screenshot](../img/annotated.png)

This project uses:
- 2 models
  - *names* is a *yaml* model that simply provides some names for use in the template
  - *errors* is *csv* model that lists a number of errors that we want to scaffold code for
- definitions in the *defs* tab
- additional include folders in the *inc* tab
- 3 outputs
  - *output* is the default output and is used here to describe the project
  - *cpp* contains some simple C++ code generated by the template
  - *h* contains some generated C++ code which is intended for a header file 
  
TextrudeInteractive renders changes immediately; if you change the value of *enumName* in the *names* model from *errors* to *warnings* you should see both the *h* and *cpp* reflect the change.  Alternatively you can make a change to the template, for example change line 18 to read 
`{{typeName =names.enumName +"_type"}}`


## Model types and access 
A model may be a simple list of objects with the same schema (as with CSV) or a heirarchy of structured objects.  In either case the syntax for accessing an element is similar; you refer to elements within a model by "dotting into"  or using array notation. For example, in the project above we can refer to:

```
names.enumName         # this is the enumName property from the names model
errors[0].description  # this is the description propery of the first row in the errors model
```

### CSV models

CSV models are imported as an array of objects with each column being exposed as a property of the object.

### Structured models (YAML/JSON)

YAML and JSON support arbitrarily deep nested structures.  In this case, the property names can be used to dot into the model

## Template syntax
In common with most templating languagues, Scriban allows you to mix *code* and *text* so it's important to know how to move from one mode to another.

*Code blocks* are indicated using **{{** and **}}**. A quoted string within a code block is evaluated as the value of the string. So the following two blocks are equivalent...

```
This is a text block and will be emitted as-is

{{# This is a single-multiline code-block where quotes are used to emit strings}}
{{for row in model
  "a line of text"
  row.Name;"=";row.value;";\r\n"
end}}

{{# This is a code block where we revert to text-mode internally}}
{{for row in model}}
  a line of text 
  {{row.Name}}={{row.value}};
{{end}}
```
Depending on the relative amount of code to text and your tolerance for braces or quotes you may prefer one style over the other. 

See the [Scriban language reference](https://github.com/scriban/scriban/blob/master/doc/language.md) for more information.

# Linking to existing files

TextrudeInteractive can store all model and template text internally within the project but you can also set up [links to external files](../doc/fileLinkage.md) allowing in-situ editing and prototyping. 

# Re-using projects from the command-line

TextrudeInteractive provides quick prototyping and development of templates and models.  If you want to use these within a build system or command-line environment you can easily export them using the [Export menu](../doc/exportInvocation.md).

