# Built-in functions and helpers


## Debug

*debug.dump* generates a representation of the object.  For example:
```
{{debug.dump model}}
```

## Humanizr

Textrude provides all the methods of the [Humanizr](https://github.com/Humanizr/Humanizer) library via the *humanizr* namespace.  These are particularly useful for enforcing naming-conventions in generated code.  For example

```
The standard C naming for camel-cased strings looks like...

{{"CamelCasedFunctionName" | humanizr.underscore}}
```

*Humanizr* has many other useful functions - see the home page for more information.

See the *humanizr.txtproj* project in the examples folder for more examples


## Misc

*misc.new_guid* generates a new GUID 

See the *misc.txtproj* project in the examples folder for more examples

## TimeComparison

The timecomparison class offers flexible time recognition and comparison operations based on the [Microsoft Recognizers](https://github.com/Microsoft/Recognizers-Text) package.

Examples of recognisable input are:
- "christmas day, 1991"
- "yesterday"
- "18 days ago"
- "3rd Mar"
- "Nov 10, 2010"
- "186353" (interpreted as seconds since the Unix epoch)

The `timecomparison.before` and `timecomparison.after` functions can be used to compare times.  E.g.
` "5 days ago" | timecomparison.after "4 jan 2021"`

See the the `11_timerange" example in the examples folder for further information.




