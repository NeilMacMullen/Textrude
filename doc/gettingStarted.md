# Getting started

*Note.  All the examples use screenshots of TextrudeInteractive but it is equally possibly to run them from the command-line using the --project option...*



##Understanding the environment
A typical template relies on:
- model.  This is the data supplied via CSV/YAML/JSON file
- env. Contains a list of environment variables inherited from the current process. 
- def.  Contains a list of user-supplied variables 
- sys. TBD 

Model types
A model may be a simple list of objects with the same schema (as with CSV) or a heirarchy of structured objects.  In either case the syntax for accessing an element is similar...
CSV example...
YAML example


examples/ex1 uses the built-in 

Basic syntax...
Code blocks are indicated using {{ }} A quoted string within a code block is evaluated as the value of the string. So the following two blocks are equivalent...

```
{{# stay in code mode for block}}
{{for row in model
  row.Name;"=";row.value;";\r\n"
end}}

{{# block defaults to text mode}}
{{for row in model}}
  {{row.Name}}={{row.value}};
{{end}}
```
Depending on the relative amount of code to text and your tolerance for braces or quotes you may prefer one style over the other 




For a full description 

