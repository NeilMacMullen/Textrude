I welcome all contributions.  

If you're not sure about what you're changing or are planning large scale changes it's probably a good idea to drop me a line via issues or discussions.

Generally I'm happy to take "rough" contributions and tidy them up into "house style" later.  A few guidelines....

For C#:

- No nulls please - I'm allergic!  Except where it's really not possible (e.g. in UI facing code) please don't introduce nulls or nullable variables into the codebase.  Prefer "empty" instances which provide sensible default behaviour. (See ModelPath for an example)   
- Prefer immutability.  I haven't adhered to this everywhere myself but where possible prefer objects with no mutable state.
- unit tests for new functionality would be great (ok, I'm not perfect here either!)

For scripts:
- no particular thoughts
 
For documentation:
- no particular thoughts- more is better!



