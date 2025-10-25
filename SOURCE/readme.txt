To compile, make sure you have the latest .NET 9.0 (or above) SDK version:
https://dotnet.microsoft.com/en-us/download/dotnet/9.0

Confirm it by running Powershell anywhere:
"dotnet --version"

Then go to the diretory you dropped the .cs files and gFirebaseDeployer.csproj and run:
"dotnet restore"

You can build the project with:
"dotnet build -c Release"

An .EXE file should be built in the output directory.
Simple as that :D




