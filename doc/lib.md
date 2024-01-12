











line 1 |>
line2


<!---

/*
   WARNING 
   -------
   This file was auto-generated.
   Do not modify by hand - your changes will be lost .
    

   Built:  03:15:10 PM on Friday, 12 Jan 2024
   Machine: NPM-OFFICE
   User:  User

*/ 

--->


# Library functions

This document describes the standard functions distributed with Textrude in the `lib` folder.


- [`cpp`](#lib/cpp.sbn)
- [`csharp`](#lib/csharp.sbn)
- [`line`](#lib/line.sbn)
- [`misc`](#lib/misc.sbn)
- [`warnings`](#lib/warnings.sbn)




## `lib/cpp.sbn`


- [`indent`](#cpp/indent)

- [`linefeed`](#cpp/linefeed)

- [`pad`](#cpp/pad)

- [`block_comment`](#cpp/block_comment)

- [`forall`](#cpp/forall)



## `lib/csharp.sbn`


- [`indent`](#csharp/indent)

- [`linefeed`](#csharp/linefeed)

- [`summary_comment`](#csharp/summary_comment)

- [`pad`](#csharp/pad)

- [`forall`](#csharp/forall)



## `lib/line.sbn`


- [`i`](#line/i)

- [`literal`](#line/literal)

- [`line_match`](#line/line_match)

- [`must_contain`](#line/must_contain)

- [`extract`](#line/extract)

- [`is_not_empty`](#line/is_not_empty)

- [`take`](#line/take)

- [`skip`](#line/skip)

- [`take_until`](#line/take_until)

- [`take_after`](#line/take_after)

- [`remove_text`](#line/remove_text)

- [`chop_left`](#line/chop_left)

- [`display`](#line/display)

- [`show_count`](#line/show_count)

- [`group_by`](#line/group_by)

- [`group_do`](#line/group_do)



## `lib/misc.sbn`


- [`apply`](#misc/apply)



## `lib/warnings.sbn`


- [`autogenwarning`](#warnings/autogenwarning)






### `cpp/indent`
```
indent <n>
```

#### Description
Sets the default indent level

#### Examples
> **input**
```scriban-html
include "lib/cpp.sbn"
cpp.indent 5
cpp.pad
"padded text"
```
> **output**
```html
      padded text
```



### `cpp/linefeed`
```
linefeed 
```

#### Description
Inserts a linefeed

#### Examples
> **input**
```scriban-html
include "lib/cpp.sbn"
"line1"
cpp.linefeed
"line2"
```
> **output**
```html
line1
line2
```



### `cpp/pad`
```
pad 
```

#### Description
Inserts a pad at the current indent level

#### Examples
> **input**
```scriban-html
include "lib/cpp.sbn"
cpp.indent 5
cpp.pad
"padded text"
```
> **output**
```html
      padded text
```



### `cpp/block_comment`
```
block_comment <text>
```

#### Description
Creates a brief-style block comment

#### Examples
> **input**
```scriban-html
include "lib/cpp.sbn"
cpp.block_comment "this is a summary"
```
> **output**
```html
/*!
   @brief this is a summary
*/

```



### `cpp/forall`
```
forall <items> <function>
```

#### Description
Applies a function to all items.
All items are separated by a linefeed and comma
except for the last

#### Examples
> **input**
```scriban-html
include "lib/cpp.sbn"
["abc","def"]
|> cpp.forall @{$0}@
```
> **output**
```html
      abc,
      def

```






### `csharp/indent`
```
indent <n>
```

#### Description
Sets the default indent level

#### Examples
> **input**
```scriban-html
include "lib/csharp.sbn"
csharp.indent 5
csharp.pad
"padded text"
```
> **output**
```html
      padded text
```



### `csharp/linefeed`
```
linefeed 
```

#### Description
Inserts a linefeed

#### Examples
> **input**
```scriban-html
include "lib/csharp.sbn"
"line1"
csharp.linefeed
"line2"
```
> **output**
```html
line1
line2
```



### `csharp/summary_comment`
```
summary_comment <text>
```

#### Description
Creates an intellisense summary block

#### Examples
> **input**
```scriban-html
include "lib/csharp.sbn"
csharp.summary_comment "this is a summary"
```
> **output**
```html

/// <summary>
/// this is a summary
/// </summary>

```



### `csharp/pad`
```
pad 
```

#### Description
Inserts a pad at the current indent level

#### Examples
> **input**
```scriban-html
include "lib/csharp.sbn"
csharp.indent 5
csharp.pad
"padded text"
```
> **output**
```html
      padded text
```



### `csharp/forall`
```
forall <items> <function>
```

#### Description
Applies a function to all items.
All items are separated by a linefeed and comma
except for the last

#### Examples
> **input**
```scriban-html
include "lib/csharp.sbn"
["abc","def"]
|> csharp.forall @{$0}@
```
> **output**
```html
      abc,
      def

```






### `line/i`
```
i 
```

#### Description
ensures case-insensitive matching"

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["line1","LINE2"]
|> must_contain "line" i
|> display
```
> **output**
```html
line1
LINE2

```



### `line/literal`
```
literal <expression>
```

#### Description
escapes a regex to make it literal

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
"abc.*" |> literal
```
> **output**
```html
abc\.\*
```



### `line/line_match`
```
line_match <line> <regex> <options>
```

#### Description
tests whether the line matches the regex

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
"abcdefg"
|> line_match "abc..fg"
```
> **output**
```html
true
```



### `line/must_contain`
```
must_contain <line array> <regex> <options>
```

#### Description
returns only lines that contains the supplied regex

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["abcdefg","abc"]
|> must_contain "abc..fg"
|> display
```
> **output**
```html
abcdefg

```



### `line/extract`
```
extract <line array> <regex> <options>
```

#### Description
extracts only the matching portions of each line

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["abcdefg","abc123defg"]
|> extract "abc.."
|> display
```
> **output**
```html
["abcde"]
["abc12"]

```



### `line/is_not_empty`
```
is_not_empty <line array>
```

#### Description
returns only non-empty lines

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["abcdefg","","  ","def "]
|> extract "abc.."
|> display
```
> **output**
```html
["abcde"]
[]
[]
[]

```



### `line/take`
```
take <line array> <n>
```

#### Description
Takes the first n lines in the pipeline

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["1","2","3","4"]
|> take 2
|> display
```
> **output**
```html
1
2

```



### `line/skip`
```
skip <line array> <n>
```

#### Description
Skips the first n lines in the pipeline

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["1","2","3","4"]
|> skip 2
|> display
```
> **output**
```html
3
4

```



### `line/take_until`
```
take_until <line array> <regex> <options>
```

#### Description
Takes lines until the regex is matched

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["1","2","STOP","4"]
|> take_until "STOP"
|> display
```
> **output**
```html
1
2

```



### `line/take_after`
```
take_after <line array> <regex> <options>
```

#### Description
Skips lines until the regex is matched

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["1","2","START","4"]
|> take_until "START"
|> display
```
> **output**
```html
1
2

```



### `line/remove_text`
```
remove_text <line array> <regex> <options>
```

#### Description
Removes text matching the supplied regex from all lines

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["1abcE","abcxyz","12354","ghabcdere"]
|> remove_text "abc"
|> display
```
> **output**
```html
1E
xyz
12354
ghdere

```



### `line/chop_left`
```
chop_left <line array> <n>
```

#### Description
Removes the first n characters from each line in the pipeline

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["12345678","12345678","STOP","4"]
|> chop_left "2"
|> display
```
> **output**
```html
345678
345678
OP


```



### `line/display`
```
display <line array>
```

#### Description
Displays a pipeline with one element per line

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["12345678","12345678","STOP","4"]
|> display
```
> **output**
```html
12345678
12345678
STOP
4

```



### `line/show_count`
```
show_count <line array> <message>
```

#### Description
Displays the number of items in the pipeline at each point

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
["12345678","12345678","STOP","4"]
|> show_count "at beginning of pipeline"
|> must_contain "12"
|> show_count "after selecting for '12'"
|> display
```
> **output**
```html
Count: at beginning of pipeline 4
Count: after selecting for '12' 2
12345678
12345678

```



### `line/group_by`
```
group_by <line array> <regex>
```

#### Description
Groups lines by match against regex

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
grp = ["12345678","12345678","STOP","4"]
|> group_by "^."
debug.dump grp
```
> **output**
```html
{}

```



### `line/group_do`
```
group_do <group> <function>
```

#### Description
Applies a function to a group

#### Examples
> **input**
```scriban-html
include "lib/line.sbn"
import line
grp = ["12345678","12345678","STOP","4"]
|> group_by "^."
|> group_do @{""+$0 +"->" +($1| array.size)+" values" }@
|> group.flatten
|> display
```
> **output**
```html

```






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
    

   Built:  03:15:10 PM on Friday, 12 Jan 2024
   Machine: NPM-OFFICE
   User:  User

*/ 

```






