# VERIFICATION 

## Chocolatey

*Verification is intended to assist the Chocolatey moderators and community in verifying that this package's contents are trustworthy.*

This package is published by [Neil MacMullen](https://github.com/NeilMacMullen) the author of Textrude.   

The pre-built binaries are also available from the project page https://github.com/NeilMacMullen/Textrude in the releases section.  You may prefer to use these. 

If you are unsure about the provenance of this package the *safest* way to install Textrude via chocolately is to:

- checkout the source code
- examine it to ensure it is not doing anything nefarious
- open a powershell prompt in the top Textrude folder
- use `build.ps1 -t choco` to build the chocolatey package from source
- install from the locally generated package....
  `choco install -s .\chocolatey textrude`





