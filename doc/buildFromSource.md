# Building Textrude from source

**IMPORTANT - if you have  *downloaded* rather than *cloned* the source, refer to the section below describing git-less builds**

If you want to build the *bleeding-edge* checkout *main*

`git checkout main`

If you want to build one of the stable releases, checkout one of the release tags

`git checkout tags/v1.5.0`

## Visual Studio

Open the *Textrude.sln* solution file and just rebuild all

If you want single-file artefacts you can  the right-click on the Textrude or TextrudeInteractive projects and choose "Publish".  This will create executables in the "Publish" folder and also copy the standard script library.

## From the command line Windows/Linux

Textrude builds using [Cake](https://cakebuild.net/) (there is no need to install this).  Use either the `build.ps1` or `build.sh` scripts to trigger a build. The easiest way to build the executables is to run 

``` 
build.ps1 -t package  
```
This will create single-file executables in the Publish folder as well as copying the lib scripts and examples alongside.  A Zip file is also created to make it easy to copy the set of files to another location. 

## Example projects

There are a number of example projects in the *examples* folder - this is a good place to start if you want to get an idea of what Textrude can do. 

# Advanced modification and extending Textrude

## Git-less builds

Textrude uses *gitversion* so that it can search for updates.  If you want to build from the source *without* a git checkout you can do so by setting 

`DisableGitVersionTask=true`


## Updating Scriban syntax support

Edit scriban.js in TextrudeInteractive/Resources.

*Note - you must rebuild the entire solution for the resource to be reloaded*

## Updating the version of Monaco

Monaco is the WebView based editor which is developed [here](https://github.com/Microsoft/monaco-editor)  You get get the latest download from [here](https://microsoft.github.io/monaco-editor/index.html)

To incorporate this in the build you must follow a few steps....
- remove all the .map files from the downloaded zip (otherwise the image will be huge!)
- modify vs/editor/editor.main.js to include sections for "scriban".  The easiest way to ensure this is to search for "scriban" in the existing file and copy these sections to the new editor.main.js.
- Add the new zip as a resource to TextrudeInteractive and modify MonacoResourceFetcher to point to this.



