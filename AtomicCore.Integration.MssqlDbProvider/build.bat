echo off

set pro_dir=%~dp0
set release=bin\Release

cd %~dp0%release%
del *.nupkg
pause

cd %pro_dir%
cmd start /k "dotnet build -c Release"
