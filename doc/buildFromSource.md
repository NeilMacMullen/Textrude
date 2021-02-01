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

