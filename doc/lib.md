


<!---

/*
   WARNING 
   -------
   This file was auto-generated.
   Do not modify by hand - your changes will be lost .
    

   Built:  11:38:56 AM on Sunday, 14 Feb 2021
   Machine: DESKTOP-T7KO4MB
   User:  neilm

*/ 

--->


# Library functions

This document describes the standard functions distributed with Textrude in the `lib` folder.


- [`misc`](#lib/misc.sbn)
- [`warnings`](#lib/warnings.sbn)




## `lib/misc.sbn`


- [`apply`](#misc/apply)



## `lib/warnings.sbn`


- [`autogenwarning`](#warnings/autogenwarning)






### `misc/apply`
```
apply <list> <function>
```

#### Description
Applies a function to each element in a list.  Unlike
array.each, this function expects to emit text 'inline'
rather than returning an array

#### Examples
> **input**
```scriban-html
include "lib/misc.sbn"
[1,2,3] | apply @(do; $0*2;end)
```
> **output**
```html
246
```






### `warnings/autogenwarning`
```
autogenwarning 
```

#### Description
Generates a boilerplate warning using C++
block commenting

#### Examples
> **input**
```scriban-html
include "lib/warnings.sbn"
autogenwarning
```
> **output**
```html

/*
   WARNING 
   -------
   This file was auto-generated.
   Do not modify by hand - your changes will be lost .
    

   Built:  11:38:56 AM on Sunday, 14 Feb 2021
   Machine: DESKTOP-T7KO4MB
   User:  neilm

*/ 

```






