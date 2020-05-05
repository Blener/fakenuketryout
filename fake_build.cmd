@echo off
cls
dotnet tool restore
dotnet paket install
dotnet fake run fake_build.fsx