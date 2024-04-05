@ECHO off

CLS
SET currentDirectory=%cd%
FOR %%a IN ("%currentDirectory%") DO SET rootDirectory=%%~dpa
SET srcDirectory=%rootDirectory%src
SET generatorFile=%srcDirectory%\Atc.Rest.ApiGenerator.CLI\bin\Debug\net8.0\atc-rest-api-generator.exe

SET projectName=Demo
SET specFile=%currentDirectory%\Demo.ApiDesign\SingleFileVersion\Api.v1.yaml
SET generatedDirectory=%currentDirectory%
SET optionsFile=%currentDirectory%\Demo.ApiDesign\ApiGeneratorOptions.json

%generatorFile% generate server all -p %projectName% -s %specFile% --outputSlnPath %generatedDirectory% --outputSrcPath %generatedDirectory%\src --outputTestPath %generatedDirectory%\test --optionsPath %optionsFile% -v true

