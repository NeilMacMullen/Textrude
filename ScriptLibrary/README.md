# ScriptLibrary

**IMPORTANT** If you have added functions to lib, please rebuild the documentation as described below!

This project has two purposes....

1. Copy all script files in the lib folder to the targets so that they can be deployed.  
This happens automatically on build but you may need to perform a Clean/rebuild all to ensure all changes are picked up.

2. Build the lib.md documentation file based on the doc.yaml and doctemplate.sbn files. To do this:
 - Build the solution in DEBUG configuration.
 - add documentation for any new files/functions you have added in **doc.yaml**
 - Open a powershell terminal in the ScriptLibrary folder 
 - run .\build.ps1 
 - check that docs\lib.md has been modified


