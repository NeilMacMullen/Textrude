# Model format conversion

Textrude is normally used to output text but when processing structured files such as Json or Yaml, it's sometimes useful to be able to write them back out in that format.  

The *Textrude* namespace offers some useful methods for this:

- *to_json* - convert an object tree to a JSON string representation
- *to_yaml* - convert an object tree to a YAML string representation
- *to_csv*  - convert an object tree to a CSV string representation
- *to_line* - convert an object tree to a set of lines


Source format  | to_json | to_yaml | to_csv | to_line
-------- | ------|----------|--|-- 
json | full support| full support| arrays of flat objects | arrays of primitives  
yaml |full support|full support| arrays of flat objects |arrays of primitives
csv|  full support | full support|full support| no support
line|full support|full support|no support | full support

## Example
Input
```
{{
    object1 = { a :"value of a1", b:"value of b1"}
    object2 = { a :"value of a2", b:"value of b2"}

    objectArray =[object1,object2]

"JSON:
"
    objectArray | textrude.to_json
" 
-----------------------------------
YAML:
"
    objectArray | textrude.to_yaml
" 
-----------------------------------
CSV:
"
    objectArray | textrude.to_csv
}}

```

Output
```
JSON:
[
  {
    "a": "value of a1",
    "b": "value of b1"
  },
  {
    "a": "value of a2",
    "b": "value of b2"
  }
] 
-----------------------------------
YAML:
- a: value of a1
  b: value of b1
- a: value of a2
  b: value of b2
 
-----------------------------------
CSV:
a,b
value of a1,value of b1
value of a2,value of b2

```