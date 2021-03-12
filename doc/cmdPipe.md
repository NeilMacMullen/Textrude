# Textrude as a pipeline processor

Textrude.exe provides a useful *pipe* command which minimises the amount of typing required when processing text input from stdin. It takes a single argument which is a template name.  The input is treated as a **lines** model.

For example, if you create a script *lc.sbn* that counts the number of blank lines in the model:

```
{{
  cnt = model 
  |> array.each @string.trim 
  |> array.filter @{($0 | string.size) ==0 }@  
  |> array.size  
  "Blank lines: "+cnt
}}
```

Then invoke it from a powershell session like this:

```
get-content *.cs | textrude.exe pipe lc.sbn
```

