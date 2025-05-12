#!/bin/bash
cd /app
dotnet new console -n App -o App --force
cp Main.cs App/Program.cs
cd App
dotnet run < ../input.txt
