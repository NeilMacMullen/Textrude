# Building Textrude from source

If you want to build the *bleeding-edge* checkout *main*

`git checkout main`

If you want to build one of the stable releases, checkout one of the release tags

`git checkout tags/v1.2.0`

## Visual Studio

Open the *Textrude.sln* solution file and just rebuild all

If you want single-file artefacts you can  the right-click on the Textrude or TextrudeInteractive projects and choose "Publish".  This will create executables in the "Publish" folder and also copy the standard script library.

## From the command line Windows/Linux

``` 
#build the Textrude Windows CLI 
dotnet publish Textrude\Textrude.csproj /p:PublishProfile=Textrude\Properties\PublishProfiles\WinX64.pubxml

#build the Textrude Linux CLI 
dotnet publish Textrude\Textrude.csproj /p:PublishProfile=Textrude\Properties\PublishProfiles\LinuxX64.pubxml

#build the TextrudeInteractive tool (windows only)
dotnet publish TextrudeInteractive\TextrudeInteractive.csproj /p:PublishProfile=TextrudeInteractive\Properties\PublishProfiles\WinX64.pubxml
```
Or just use the powershell script in the Textrude folder
```
make_release.ps1
```

## Example projects

There are a number of example projects in the *examples* folder - this is a good place to start if you want to get an idea of what Textrude can do. 

## Git-less builds

Textrude uses *gitversion* so that it can search for updates.  If you want to build from the source *without* a git checkout you can do so by setting 

`DisableGitVersionTask=true`


## Updating Scriban syntax support

Edit scriban.js in TextrudeInteractive/Resources.

*Note - you must rebuild the project for the resource to be reloaded*

## Updating the version of Monaco

Monaco is the WebView based editor which is developed [here](https://github.com/Microsoft/monaco-editor)  You get get the latest downlaod from [here](https://microsoft.github.io/monaco-editor/index.html)

To incorporate this in the build you must follow a few steps....
- remove all the .map files from the downloaded zip (otherwise the image will be huge!)
- modify vs/editor/editor.main.js to include sections for "scriban".  The easiest way to ensure this is to search for "scriban" in the existing file and copy these sections to the new editor.main.js.
- Add the new zip as a resource to TextrudeInteractive and modify MonacoResourceFetcher to point to this.



