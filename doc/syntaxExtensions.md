# Syntax extensions

Textrude offers some non-standard syntax extensions.  It is important to understand that these are applied by pre-processing the text of the template.  Line numbering is correctly preserved but column numbers in error messages may be misleading as a result.

Syntax extensions can be temporarily disabled for a section of the template using a Textrude *directive*, i.e. a comment that instructs the pre-processor to take action.

Textrude directives **must** be placed on a line that starts with a double-brace.

## Pipe hoisting

Standard Scriban syntax supports multi-line pipes, but only if the pipe is the last token on a line.  The extended syntax allows a `|>` token at the begining of a line to be *hoisted* to the end of the line above.

``` 
   model
   |> operation1
   |> operation2
```
is transformed to 

``` 
   model |>
 operation1 |>
 operation2
```

Control
```
{{# textrude push nopipehoist}}
 |>  text in this section will be   <|
 |>  emitted with normal formatting <|  
{{# textrude pop nopipehoist}}


## Automatic whitespace snarfing for functions

Standard Scriban syntax is to treat the whitespace between function declarations as literal.  This can be annoying when declaring lots of functions to use later.  The extended syntax automatically transforms `{{func` into `{{-func` so that blank lines before a function declaration are consumed.

Control
```
{{# textrude push nofuncsnarf}}
these blank  lines will be preserved..


{{func f
end}}
{{# textrude pop nofuncsnarf}}


## Terse anonymous function syntax

Standard Scriban syntax requires anonymous functions to be written as `@(do;ret ... ; end)`.  The extended syntax allows anonymous functions to be surrounded with `@{` and `}@`


``` 
  model | array.each @{ $0 *2 }@ 
```
is transformed to 

``` 
  model | array.each @(do; ret ($0 *2);end) 
```

Control
```
{{# textrude push noterselambda}}
{{# textrude pop noterselambda}}


