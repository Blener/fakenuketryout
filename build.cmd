@echo off
cls
dotnet tool restore
dotnet paket install
dotnet fake -v run build.fsx