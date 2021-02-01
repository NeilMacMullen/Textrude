Push-Location
$pub ="publish"

Remove-Item $pub -recurse -Force

#build the Textrude Windows CLI 
dotnet publish Textrude\Textrude.csproj /p:PublishProfile=Textrude\Properties\PublishProfiles\WinX64.pubxml
#build the Textrude Linux CLI 
dotnet publish Textrude\Textrude.csproj /p:PublishProfile=Textrude\Properties\PublishProfiles\LinuxX64.pubxml
#build the TextrudeInteractive tool (windows only)
dotnet publish TextrudeInteractive\TextrudeInteractive.csproj /p:PublishProfile=TextrudeInteractive\Properties\PublishProfiles\WinX64.pubxml

Set-Location $pub
Remove-Item *.pdb  -Recurse
Remove-Item arm64  -Recurse
Remove-Item x86  -Recurse
Remove-Item x64  -Recurse
Move-Item linux/Textrude Textrude_linux
Remove-Item linux  -Recurse
mkdir examples
Pop-Location

Copy-Item Examples\* $pub\examples


