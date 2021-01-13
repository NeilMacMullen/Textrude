# Creating a library of functions

Textrude supports reuse of code and templates via the SCRIBAN *include* function.
You can set up additional include paths in the *other->include paths* section of the UI.

**IMPORTANT** 
You should use unix-style slashes for path separators

Example:

```
{{include "customlib/myfunctions.sbn"}}
```



