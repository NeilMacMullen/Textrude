# Textrude convenience commands for working with files

The *render* command provides the most powerful way of applying templates to arbitrary sets of inputs and outputs as described [here](buildSystemIntegration.md) but **Textrude** also supplies some commands that make it easy to apply simple Scriban expressions to files as "one-liners".

All commands have the same basic parameter set:

- *-i, --input* : an input file or *stdin* if omitted
- *-o, --output* : an output file or *stdout* if omitted
- *-e, --expression* : a Scriban expression
- *-f, --format* : specifies the format of the data.  If this is omitted, textrude will try to guess the format, first by looking at the file extension (if a filename was used) and then by examining the first few hundred characters of the input.

All the Scriban [built-in](https://github.com/scriban/scriban/blob/master/doc/builtins.md) methods are imported to allow for terser expressions; ie *contains* may be used instead of *string.contains*.

## 'i' commands

The *i* commands assume that the input is an array of items.  I.e. Json array, Yaml array, CSV file or text file (which is treated as an array of lines)

The *expression* is applied to each item in turn, with the current item being referred to as *i*.

### iFilter

Returns only the items that match the filter.  
Examples: 
```
# perform a search on items
textrude iFilter -e "(i.Population >100) && (contains i.Country 'United')"
```

### iDo

Mutates each item in turn and returns the input set

Examples:
```
# create a new column/property from two others 
textrude iDo -e "i.productivity= i.gdp/i.population"

# remove a column/property
textrude iDo -e "i.unwanted=null"
```

### iMap

Creates a new item in place of the old item returns the mapped set

Examples:
```
#create a new set of records, each with an 'id' and 'name' property derived from the input
textrude iMap -e "id: i.id, name: (i.first+i.last) "
```

## 'm' commands

The *m* commands operate on the model.  In contrast to the *i* commands, they do not automatically return a serialised representation of the model because it's not normally useful.  However you can use a pipe to one of the [format conversion](format_conversion.md) commands if you want. 

### mDo

Pipes the model into the expression and returns the result.

Examples: 
```
# return the number of items in the input.  Note that "array.size" is fully qualifed
# because the string.size method would otherwise take preference
textrude mDo -e "array.size"
```
### Do

Returns the result of the expression.  The input model is referred to as 'm'

Examples 
```
# remove an unwanted property and return the JSON representation
textrude Do -e "m.additional_info=null;to_json"

# print out the size of the Yaml represention vs the size of the JSON representation
textrude Do -e "'Yaml:'+ (m|to_yaml|size) +' Json:'+(to_json|size)"

```


