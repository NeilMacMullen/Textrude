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

*Humanizr* has manyother useful functions - see the home page for more information.

See the *humanizr.txtproj* project in the examples folder for more examples


## Misc

*misc.new_guid* generates a new GUID 

See the *misc.txtproj* project in the examples folder for more examples

