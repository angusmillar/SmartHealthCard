How to publish to Nuget:

1. Rev the version in the project package dialog and save
2. Right click the project and select Pack
3. Find the .nupkg package spec in the release bin folder e.g : ..SmartHealthCard\SmartHealthCard.Token\bin\Release\SmartHealthCard.Token.0.1.0-alpha.1.nupkg
4. Using the command line in that directory 

dotnet nuget push [PackageFileName.nupkg] --api-key [Nuget API key] --source https://api.nuget.org/v3/index.json

dotnet nuget push SmartHealthCard.Token.5.0.0.nupkg --api-key oy2dzeuqwpmslidoau4a26rb3fc3demetgxxtrreqh4a2e --source https://api.nuget.org/v3/index.json

