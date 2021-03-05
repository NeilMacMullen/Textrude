# Creating a library of functions

Textrude supports reuse of code and templates via the SCRIBAN *include* function.
You can set up additional include paths in the *inc* section of the UI.

**IMPORTANT** 
You should use unix-style slashes for path separators

Example:

```
{{include "customlib/myfunctions.sbn"}}
```

## Using namespaces

If you are creating a reusable library, you may wish to expose the functions and objects via a namespace to avoid clashes with other libraries.  For example:

```
mylib.f1 
mylib.f2 
```
You can do this by using the built-in helper function *textrude.create_library* This accepts two arguments; the first is the special Scriban *this* variable, the second is the name of the library you want to create.  All functions or objects that are prefixed with __<library-name>_ will be imported into an object of the library name.  For example:

```
{{
  func __mylib_f1
  "hello from my library"
  end 

  textrude.create_library this "mylib"

  # the output of this is "hello from my library"
  mylib.f1 

}}
```






