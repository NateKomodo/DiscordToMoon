dotnet publish -r ubuntu.20.04-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
dotnet publish -r win10-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained

