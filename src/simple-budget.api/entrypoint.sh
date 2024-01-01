#!/bin/sh

cd src
cd simple-budget.api
dotnet restore ./simple-budget.api.csproj
dotnet build ./simple-budget.api.csproj
dotnet watch ./bin/Debug/net8.0/simple-budget.api.dll